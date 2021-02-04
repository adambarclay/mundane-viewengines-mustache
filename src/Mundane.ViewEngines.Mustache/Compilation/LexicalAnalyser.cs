using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.FileProviders;

namespace Mundane.ViewEngines.Mustache.Compilation
{
	internal sealed class LexicalAnalyser
	{
		private static readonly LexerState Text = LexicalAnalyser.TextState;
		private static readonly LexerState BraceClose = LexicalAnalyser.BraceCloseState;
		private static readonly LexerState IdentifierStart = LexicalAnalyser.IdentifierStartState;
		private static readonly LexerState Identifier = LexicalAnalyser.IdentifierState;
		private static readonly LexerState IdentifierEnd = LexicalAnalyser.IdentifierEndState;
		private static readonly LexerState BraceOpen = LexicalAnalyser.BraceOpenState;

		private readonly List<string> literals;
		private readonly StringBuilder stringBuilder;
		private readonly List<Token> tokens;
		private int currentColumn;
		private int currentLine;

		private LexicalAnalyser()
		{
			this.tokens = new List<Token>();
			this.literals = new List<string>();
			this.stringBuilder = new StringBuilder(1024);
			this.currentLine = 0;
			this.currentColumn = 0;
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
			var lastCharacter = '\0';

			var lexerState = LexicalAnalyser.Text;

			while (characterOffset < template.Length)
			{
				var character = template[characterOffset++];

				lexerState = lexerState.Invoke(state, character);

				if (character == '\r')
				{
					++state.currentLine;
					state.currentColumn = 0;
				}
				else if (character == '\n')
				{
					if (lastCharacter != '\r')
					{
						++state.currentLine;
						state.currentColumn = 0;
					}
				}
				else
				{
					++state.currentColumn;
				}

				lastCharacter = character;
			}

			if (state.stringBuilder.Length > 0)
			{
				state.literals.Add(state.stringBuilder.ToString());
			}

			state.tokens.Add(new Token(TokenType.End, state.currentLine, state.currentColumn));

			return (state.tokens, state.literals);
		}

		private static LexerState BraceCloseState(LexicalAnalyser state, char character)
		{
			if (character == '}')
			{
				if (state.stringBuilder.Length > 0)
				{
					state.literals.Add(state.stringBuilder.ToString());
					state.stringBuilder.Clear();
				}

				state.tokens.Add(new Token(TokenType.CloseTag, state.currentLine, state.currentColumn));

				return LexicalAnalyser.Text;
			}

			state.stringBuilder.Append('}');
			state.stringBuilder.Append(character);

			return LexicalAnalyser.Identifier;
		}

		private static LexerState BraceOpenState(LexicalAnalyser state, char character)
		{
			if (character == '{')
			{
				if (state.stringBuilder.Length > 0)
				{
					state.literals.Add(state.stringBuilder.ToString());
					state.stringBuilder.Clear();
				}

				state.tokens.Add(new Token(TokenType.OpenTag, state.currentLine, state.currentColumn));

				return LexicalAnalyser.IdentifierStart;
			}

			if (state.stringBuilder.Length == 0)
			{
				state.tokens.Add(new Token(TokenType.Text, state.currentLine, state.currentColumn));
			}

			state.stringBuilder.Append('{');
			state.stringBuilder.Append(character);

			return LexicalAnalyser.Text;
		}

		private static LexerState IdentifierEndState(LexicalAnalyser state, char character)
		{
			if (char.IsWhiteSpace(character))
			{
				return LexicalAnalyser.IdentifierEnd;
			}

			if (character == '}')
			{
				return LexicalAnalyser.BraceClose;
			}

			state.tokens.Add(new Token(TokenType.Identifier, state.currentLine, state.currentColumn));
			state.stringBuilder.Append(character);

			return LexicalAnalyser.Identifier;
		}

		private static LexerState IdentifierStartState(LexicalAnalyser state, char character)
		{
			if (char.IsWhiteSpace(character))
			{
				return LexicalAnalyser.IdentifierStartState;
			}

			if (character == '}')
			{
				return LexicalAnalyser.BraceClose;
			}

			state.tokens.Add(new Token(TokenType.Identifier, state.currentLine, state.currentColumn));
			state.stringBuilder.Append(character);

			return LexicalAnalyser.Identifier;
		}

		private static LexerState IdentifierState(LexicalAnalyser state, char character)
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

		private static LexerState TextState(LexicalAnalyser state, char character)
		{
			if (character == '{')
			{
				return LexicalAnalyser.BraceOpen;
			}

			if (state.stringBuilder.Length == 0)
			{
				state.tokens.Add(new Token(TokenType.Text, state.currentLine, state.currentColumn));
			}

			state.stringBuilder.Append(character);

			return LexicalAnalyser.Text;
		}
	}
}
