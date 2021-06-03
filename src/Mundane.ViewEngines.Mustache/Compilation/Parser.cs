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
			6. P ::= {{# B
			7. P ::= {{^ B
			8. B ::= id }} P {{/ id }} P
		*/
		private static readonly Dictionary<TokenType, Dictionary<TokenType, int>> ParsingTable =
			new Dictionary<TokenType, Dictionary<TokenType, int>>
			{
				{
					TokenType.Program, new Dictionary<TokenType, int>
					{
						{ TokenType.Epsilon, 1 },
						{ TokenType.Text, 2 },
						{ TokenType.OpenTag, 3 },
						{ TokenType.Raw, 4 },
						{ TokenType.Partial, 5 },
						{ TokenType.OpenBlock, 6 },
						{ TokenType.InvertedBlock, 7 }
					}
				},
				{ TokenType.Block, new Dictionary<TokenType, int> { { TokenType.Identifier, 8 } } }
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

						if (previousToken == TokenType.OpenBlock || previousToken == TokenType.InvertedBlock)
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
					(ruleTable.TryGetValue(tokens[tokenOffset].TokenType, out var rule) ||
						ruleTable.TryGetValue(TokenType.Epsilon, out rule)))
				{
					switch (rule)
					{
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
							tokenStack.Push(TokenType.Block);
							tokenStack.Push(TokenType.OpenBlock);

							break;
						}

						case 7:
						{
							tokenStack.Push(TokenType.Block);
							tokenStack.Push(TokenType.InvertedBlock);

							break;
						}

						case 8:
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
					}
				}
				else
				{
					return (true, new ParserError(filePath, tokens[tokenOffset], nextToken));
				}
			}

			return (false, default);
		}
	}
}
