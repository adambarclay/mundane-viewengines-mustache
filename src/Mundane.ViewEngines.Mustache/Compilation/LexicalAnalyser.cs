using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.FileProviders;

namespace Mundane.ViewEngines.Mustache.Compilation
{
	internal static class LexicalAnalyser
	{
		private static readonly LexerState BraceClose = (tokens, literals, stringBuilder, character) =>
		{
			if (character == '}')
			{
				if (stringBuilder.Length > 0)
				{
					literals.Add(stringBuilder.ToString());
					stringBuilder.Clear();
				}

				tokens.Add(Token.CloseTag);

				return LexicalAnalyser.Text;
			}

			if (stringBuilder.Length == 0)
			{
				tokens.Add(Token.Identifier);
			}

			stringBuilder.Append('}');
			stringBuilder.Append(character);

			return LexicalAnalyser.Identifier;
		};

		private static readonly LexerState BraceOpen = (tokens, literals, stringBuilder, character) =>
		{
			if (character == '{')
			{
				if (stringBuilder.Length > 0)
				{
					literals.Add(stringBuilder.ToString());
					stringBuilder.Clear();
				}

				tokens.Add(Token.OpenTag);

				return LexicalAnalyser.Identifier;
			}

			if (stringBuilder.Length == 0)
			{
				tokens.Add(Token.Text);
			}

			stringBuilder.Append('{');
			stringBuilder.Append(character);

			return LexicalAnalyser.Text;
		};

		private static readonly LexerState Identifier = (tokens, literals, stringBuilder, character) =>
		{
			if (char.IsWhiteSpace(character))
			{
				if (stringBuilder.Length > 0)
				{
					literals.Add(stringBuilder.ToString());
					stringBuilder.Clear();
				}

				return LexicalAnalyser.IdentifierEnd;
			}

			if (character == '}')
			{
				return LexicalAnalyser.BraceClose;
			}

			stringBuilder.Append(character);

			return LexicalAnalyser.Identifier;
		};

		private static readonly LexerState IdentifierEnd = (tokens, literals, stringBuilder, character) =>
		{
			if (char.IsWhiteSpace(character))
			{
				return LexicalAnalyser.IdentifierEnd;
			}

			if (character == '}')
			{
				return LexicalAnalyser.BraceClose;
			}

			tokens.Add(Token.Identifier);
			stringBuilder.Append(character);

			return LexicalAnalyser.Identifier;
		};

		private static readonly LexerState Text = (tokens, literals, stringBuilder, character) =>
		{
			if (character == '{')
			{
				return LexicalAnalyser.BraceOpen;
			}

			if (stringBuilder.Length == 0)
			{
				tokens.Add(Token.Text);
			}

			stringBuilder.Append(character);

			return LexicalAnalyser.Text;
		};

		private delegate LexerState LexerState(
			List<Token> tokens,
			List<string> literals,
			StringBuilder stringBuilder,
			char character);

		internal static (List<Token> Tokens, List<string> Literals) Tokenise(IFileInfo templateFile)
		{
			string template;

			using (var resourceStream = templateFile.CreateReadStream())
			{
				using (var streamReader = new StreamReader(resourceStream, Encoding.UTF8))
				{
					template = streamReader.ReadToEnd();
				}
			}

			var tokens = new List<Token>();
			var literals = new List<string>();

			var offset = 0;

			var stringBuilder = new StringBuilder(1024);

			var lexerState = LexicalAnalyser.Text;

			while (offset < template.Length)
			{
				lexerState = lexerState.Invoke(tokens, literals, stringBuilder, template[offset++]);
			}

			if (stringBuilder.Length > 0)
			{
				literals.Add(stringBuilder.ToString());
			}

			tokens.Add(Token.End);

			return (tokens, literals);
		}
	}
}
