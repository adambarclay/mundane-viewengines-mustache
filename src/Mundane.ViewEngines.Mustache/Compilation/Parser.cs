using System.Collections.Generic;

namespace Mundane.ViewEngines.Mustache.Compilation
{
	internal static class Parser
	{
		/*
			1. P ::= ''
			2. P ::= {{ id }} P
			3. P ::= <text> P
		*/
		private static readonly Dictionary<TokenType, Dictionary<TokenType, int>> ParsingTable =
			new Dictionary<TokenType, Dictionary<TokenType, int>>
			{
				{
					TokenType.Program, new Dictionary<TokenType, int>
					{
						{ TokenType.End, 1 },
						{ TokenType.OpenTag, 2 },
						{ TokenType.Text, 3 }
					}
				}
			};

		internal static (bool Invalid, ParserError Error) Parse(string filePath, List<Token> tokens)
		{
			var stack = new Stack<TokenType>();

			stack.Push(TokenType.End);
			stack.Push(TokenType.Program);

			var tokenOffset = 0;

			while (stack.Count > 0)
			{
				if (stack.Peek() == tokens[tokenOffset].TokenType)
				{
					stack.Pop();

					++tokenOffset;
				}
				else if (Parser.ParsingTable.TryGetValue(stack.Peek(), out var ruleTable) &&
					ruleTable.TryGetValue(tokens[tokenOffset].TokenType, out var rule))
				{
					switch (rule)
					{
						case 1:
						{
							stack.Pop();

							break;
						}

						case 2:
						{
							stack.Push(TokenType.CloseTag);
							stack.Push(TokenType.Identifier);
							stack.Push(TokenType.OpenTag);

							break;
						}

						case 3:
						{
							stack.Push(TokenType.Text);

							break;
						}
					}
				}
				else
				{
					return (true, new ParserError(filePath, stack.Peek(), tokens[tokenOffset]));
				}
			}

			return (false, default);
		}
	}
}
