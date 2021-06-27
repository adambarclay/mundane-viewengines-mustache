using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.FileProviders;

namespace Mundane.ViewEngines.Mustache.Compilation
{
	internal sealed class LexicalAnalyser
	{
		private readonly LineCounter lineCounter;
		private readonly List<string> literals;
		private readonly StringBuilder stringBuilder;
		private readonly List<Token> tokens;

		private LexicalAnalyser()
		{
			this.tokens = new List<Token>();
			this.literals = new List<string>();
			this.stringBuilder = new StringBuilder(1024);
			this.lineCounter = new LineCounter(0, 0);
		}

		private delegate LexerState LexerState(LexicalAnalyser state, char character);

		internal static (List<Token> Tokens, List<string> Literals) Tokenise(IFileInfo templateFile)
		{
			string template;

			using (var resourceStream = templateFile.CreateReadStream()!)
			{
				using (var streamReader = new StreamReader(resourceStream, Encoding.UTF8))
				{
					template = streamReader.ReadToEnd();
				}
			}

			var state = new LexicalAnalyser();

			var characterOffset = 0;

			LexerState lexerState = LexicalAnalyser.Text;

			while (characterOffset < template.Length)
			{
				var character = template[characterOffset++];

				lexerState = lexerState.Invoke(state, character);

				state.lineCounter.Advance(character);
			}

			if (state.stringBuilder.Length > 0)
			{
				state.literals.Add(state.stringBuilder.ToString());
			}

			state.tokens.Add(new Token(TokenType.End, state.lineCounter));

			return (state.tokens, state.literals);
		}

		private static LexerState BraceClose(LexicalAnalyser state, char character)
		{
			if (character == '}')
			{
				if (state.stringBuilder.Length > 0)
				{
					state.literals.Add(state.stringBuilder.ToString());
					state.stringBuilder.Clear();
				}

				state.tokens.Add(new Token(TokenType.CloseTag, state.lineCounter));

				return LexicalAnalyser.Text;
			}

			state.stringBuilder.Append('}');
			state.stringBuilder.Append(character);

			return LexicalAnalyser.Identifier;
		}

		private static LexerState BraceOpen(LexicalAnalyser state, char character)
		{
			if (character == '{')
			{
				if (state.stringBuilder.Length > 0)
				{
					state.literals.Add(state.stringBuilder.ToString());
					state.stringBuilder.Clear();
				}

				return LexicalAnalyser.DoubleBraceOpen;
			}

			if (state.stringBuilder.Length == 0)
			{
				state.tokens.Add(new Token(TokenType.Text, state.lineCounter));
			}

			state.stringBuilder.Append('{');
			state.stringBuilder.Append(character);

			return LexicalAnalyser.Text;
		}

		private static LexerState DoubleBraceOpen(LexicalAnalyser state, char character)
		{
			if (char.IsWhiteSpace(character))
			{
				return LexicalAnalyser.DoubleBraceOpen;
			}

			if (character == '&')
			{
				state.tokens.Add(new Token(TokenType.Raw, state.lineCounter));

				return LexicalAnalyser.IdentifierStart;
			}

			if (character == '>')
			{
				state.tokens.Add(new Token(TokenType.Partial, state.lineCounter));

				return LexicalAnalyser.IdentifierStart;
			}

			if (character == '#')
			{
				state.tokens.Add(new Token(TokenType.OpenBlock, state.lineCounter));

				return LexicalAnalyser.IdentifierStart;
			}

			if (character == '^')
			{
				state.tokens.Add(new Token(TokenType.InvertedBlock, state.lineCounter));

				return LexicalAnalyser.IdentifierStart;
			}

			if (character == '$')
			{
				state.tokens.Add(new Token(TokenType.ReplacementBlock, state.lineCounter));

				return LexicalAnalyser.IdentifierStart;
			}

			if (character == '<')
			{
				state.tokens.Add(new Token(TokenType.LayoutBlock, state.lineCounter));

				return LexicalAnalyser.IdentifierStart;
			}

			if (character == '/')
			{
				state.tokens.Add(new Token(TokenType.CloseBlock, state.lineCounter));

				return LexicalAnalyser.IdentifierStart;
			}

			state.tokens.Add(new Token(TokenType.OpenTag, state.lineCounter));

			return LexicalAnalyser.IdentifierStart(state, character);
		}

		private static LexerState Identifier(LexicalAnalyser state, char character)
		{
			if (char.IsWhiteSpace(character))
			{
				if (state.stringBuilder.Length > 0)
				{
					state.literals.Add(state.stringBuilder.ToString());
					state.stringBuilder.Clear();
				}

				return LexicalAnalyser.IdentifierEnd;
			}

			if (character == '}')
			{
				return LexicalAnalyser.BraceClose;
			}

			state.stringBuilder.Append(character);

			return LexicalAnalyser.Identifier;
		}

		private static LexerState IdentifierEnd(LexicalAnalyser state, char character)
		{
			if (char.IsWhiteSpace(character))
			{
				return LexicalAnalyser.IdentifierEnd;
			}

			if (character == '}')
			{
				return LexicalAnalyser.BraceClose;
			}

			state.tokens.Add(new Token(TokenType.Identifier, state.lineCounter));
			state.stringBuilder.Append(character);

			return LexicalAnalyser.Identifier;
		}

		private static LexerState IdentifierStart(LexicalAnalyser state, char character)
		{
			if (char.IsWhiteSpace(character))
			{
				return LexicalAnalyser.IdentifierStart;
			}

			if (character == '}')
			{
				return LexicalAnalyser.BraceClose;
			}

			state.tokens.Add(new Token(TokenType.Identifier, state.lineCounter));
			state.stringBuilder.Append(character);

			return LexicalAnalyser.Identifier;
		}

		private static LexerState Text(LexicalAnalyser state, char character)
		{
			if (character == '{')
			{
				return LexicalAnalyser.BraceOpen;
			}

			if (state.stringBuilder.Length == 0)
			{
				state.tokens.Add(new Token(TokenType.Text, state.lineCounter));
			}

			state.stringBuilder.Append(character);

			return LexicalAnalyser.Text;
		}
	}
}
