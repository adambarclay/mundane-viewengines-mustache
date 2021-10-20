using System;
using System.Collections.Generic;

namespace Mundane.ViewEngines.Mustache.Compilation
{
	internal static class Parser
	{
		/*
			 1. P ::= ε
			 2. P ::= text P
			 3. P ::= {{ id }} P
			 4. P ::= {{& id }} P
			 5. P ::= {{> id }} P
			 6. P ::= {{~ id }} P
			 7. P ::= {{# B
			 8. P ::= {{^ B
			 9. P ::= {{$ B
			10. P ::= {{< id }} R {{/ id }} P
			11. B ::= id }} P {{/ id }} P
			12. R ::= {{$ id }} P {{/ id }} R
			13. R ::= ε
		*/
		private static readonly Dictionary<TokenType, Dictionary<TokenType, int>> ParsingTable =
			new Dictionary<TokenType, Dictionary<TokenType, int>>
			{
				{
					TokenType.Program, new Dictionary<TokenType, int>
					{
						{ TokenType.End, 1 },
						{ TokenType.CloseBlock, 1 },
						{ TokenType.Text, 2 },
						{ TokenType.OpenTag, 3 },
						{ TokenType.Raw, 4 },
						{ TokenType.Partial, 5 },
						{ TokenType.Url, 6 },
						{ TokenType.OpenBlock, 7 },
						{ TokenType.InvertedBlock, 8 },
						{ TokenType.ReplacementBlock, 9 },
						{ TokenType.LayoutBlock, 10 }
					}
				},
				{ TokenType.Block, new Dictionary<TokenType, int> { { TokenType.Identifier, 11 } } },
				{
					TokenType.Replacement, new Dictionary<TokenType, int>
					{
						{ TokenType.ReplacementBlock, 12 },
						{ TokenType.CloseBlock, 1 },
						{ TokenType.Text, 0 } // Ignore whitespace in the layout block
					}
				}
			};

		internal static (bool Invalid, ParserError Error) Parse(
			string filePath,
			List<Token> tokens,
			List<string> literals)
		{
			var blockStack = new Stack<string>();
			var literalOffset = 0;

			var tokenStack = new Stack<TokenType>();

			tokenStack.Push(TokenType.End);
			tokenStack.Push(TokenType.Program);

			var tokenOffset = 0;

			while (tokenStack.Count > 0)
			{
				var nextToken = tokenStack.Peek();

				if (nextToken == tokens[tokenOffset].TokenType)
				{
					var token = tokenStack.Pop();

					if (token == TokenType.Text)
					{
						++literalOffset;
					}
					else if (token == TokenType.Identifier)
					{
						var previousToken = tokens[tokenOffset - 1].TokenType;

						if (previousToken == TokenType.OpenBlock ||
							previousToken == TokenType.InvertedBlock ||
							previousToken == TokenType.ReplacementBlock ||
							previousToken == TokenType.LayoutBlock)
						{
							blockStack.Push(literals[literalOffset]);
						}
						else if (previousToken == TokenType.CloseBlock)
						{
							var expectedIdentifier = blockStack.Pop();

							if (expectedIdentifier != literals[literalOffset])
							{
								var message = "Block closing tag {{/" +
									literals[literalOffset] +
									"}} does not correspond to opening tag {{" +
									expectedIdentifier +
									"}}.";

								return (true, new ParserError(filePath, tokens[tokenOffset], message));
							}
						}

						++literalOffset;
					}

					++tokenOffset;
				}
				else if (Parser.ParsingTable.TryGetValue(nextToken, out var ruleTable) &&
					ruleTable.TryGetValue(tokens[tokenOffset].TokenType, out var rule))
				{
					switch (rule)
					{
						case 0:
						{
							var text = literals[literalOffset];

							if (text.AsSpan().TrimStart().Length != 0)
							{
								return (true,
									new ParserError(
										filePath,
										Parser.NonWhitespaceTextErrorToken(tokens[tokenOffset], text),
										nextToken));
							}

							tokens.RemoveAt(tokenOffset);
							literals.RemoveAt(literalOffset);

							break;
						}

						case 1:
						{
							tokenStack.Pop();

							break;
						}

						case 2:
						{
							tokenStack.Push(TokenType.Text);

							break;
						}

						case 3:
						{
							tokenStack.Push(TokenType.CloseTag);
							tokenStack.Push(TokenType.Identifier);
							tokenStack.Push(TokenType.OpenTag);

							break;
						}

						case 4:
						{
							tokenStack.Push(TokenType.CloseTag);
							tokenStack.Push(TokenType.Identifier);
							tokenStack.Push(TokenType.Raw);

							break;
						}

						case 5:
						{
							tokenStack.Push(TokenType.CloseTag);
							tokenStack.Push(TokenType.Identifier);
							tokenStack.Push(TokenType.Partial);

							break;
						}

						case 6:
						{
							tokenStack.Push(TokenType.CloseTag);
							tokenStack.Push(TokenType.Identifier);
							tokenStack.Push(TokenType.Url);

							break;
						}

						case 7:
						{
							tokenStack.Push(TokenType.Block);
							tokenStack.Push(TokenType.OpenBlock);

							break;
						}

						case 8:
						{
							tokenStack.Push(TokenType.Block);
							tokenStack.Push(TokenType.InvertedBlock);

							break;
						}

						case 9:
						{
							tokenStack.Push(TokenType.Block);
							tokenStack.Push(TokenType.ReplacementBlock);

							break;
						}

						case 10:
						{
							tokenStack.Push(TokenType.CloseTag);
							tokenStack.Push(TokenType.Identifier);
							tokenStack.Push(TokenType.CloseBlock);
							tokenStack.Push(TokenType.Replacement);
							tokenStack.Push(TokenType.CloseTag);
							tokenStack.Push(TokenType.Identifier);
							tokenStack.Push(TokenType.LayoutBlock);

							break;
						}

						case 11:
						{
							tokenStack.Pop();

							tokenStack.Push(TokenType.CloseTag);
							tokenStack.Push(TokenType.Identifier);
							tokenStack.Push(TokenType.CloseBlock);
							tokenStack.Push(TokenType.Program);
							tokenStack.Push(TokenType.CloseTag);
							tokenStack.Push(TokenType.Identifier);

							break;
						}

						case 12:
						{
							tokenStack.Push(TokenType.CloseTag);
							tokenStack.Push(TokenType.Identifier);
							tokenStack.Push(TokenType.CloseBlock);
							tokenStack.Push(TokenType.Program);
							tokenStack.Push(TokenType.CloseTag);
							tokenStack.Push(TokenType.Identifier);
							tokenStack.Push(TokenType.ReplacementBlock);

							break;
						}
					}
				}
				else
				{
					return (true, new ParserError(filePath, tokens[tokenOffset], nextToken));
				}
			}

			return (false, default);
		}

		private static Token NonWhitespaceTextErrorToken(Token token, string text)
		{
			var lineCounter = new LineCounter(token.Line, token.Character);

			var count = 0;

			while (count < text.Length)
			{
				var character = text[count++];

				if (char.IsWhiteSpace(character))
				{
					lineCounter.Advance(character);
				}
				else
				{
					count = text.Length;
				}
			}

			return new Token(token.TokenType, lineCounter);
		}
	}
}
