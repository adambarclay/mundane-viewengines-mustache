using System;
using System.Collections.Generic;
using Microsoft.Extensions.FileProviders;
using Mundane.ViewEngines.Mustache.Engine;

namespace Mundane.ViewEngines.Mustache.Compilation
{
	internal static class Compiler
	{
		private static readonly Dictionary<Token, Dictionary<Token, int>> Table =
			new Dictionary<Token, Dictionary<Token, int>>
			{
				{
					Token.Program, new Dictionary<Token, int>
					{
						{ Token.End, 1 },
						{ Token.OpenTag, 2 },
						{ Token.Text, 3 }
					}
				}
			};

		internal static
			List<(string Path, (List<Instruction> Instructions, List<string> Literals, List<string[]> Identifiers)
				Program)> Compile(List<(string Path, IFileInfo File)> templateFiles)
		{
			var programs = new List<(string, (List<Instruction>, List<string>, List<string[]>))>();

			foreach ((var path, var file) in templateFiles)
			{
				(var tokens, var literals) = LexicalAnalyser.Tokenise(file);

				Compiler.Parse(tokens);

				programs.Add((path, Compiler.GenerateCode(tokens, literals)));
			}

			return programs;
		}

		private static (List<Instruction> Instructions, List<string> Literals, List<string[]> Identifiers) GenerateCode(
			List<Token> tokens,
			List<string> scannedliterals)
		{
			var instructions = new List<Instruction>();
			var literals = new List<string>();
			var identifiers = new List<string[]>();

			var scannedLiteralOffset = 0;

			var literalOffset = 0;
			var identifierOffset = 0;

			foreach (var token in tokens)
			{
				switch (token)
				{
					case Token.Text:
					{
						literals.Add(scannedliterals[scannedLiteralOffset++]);

						instructions.Add(new Instruction(InstructionType.Literal, literalOffset++));

						break;
					}

					case Token.Identifier:
					{
						identifiers.Add(scannedliterals[scannedLiteralOffset++].Split('.'));

						instructions.Add(new Instruction(InstructionType.OutputValue, identifierOffset++));

						break;
					}

					case Token.End:
					{
						instructions.Add(new Instruction(InstructionType.Return, 0));

						break;
					}
				}
			}

			return (instructions, literals, identifiers);
		}

		private static void Parse(List<Token> tokens)
		{
			var stack = new Stack<Token>();

			stack.Push(Token.End);
			stack.Push(Token.Program);

			var tokenOffset = 0;

			while (stack.Count > 0)
			{
				if (stack.Peek() == tokens[tokenOffset])
				{
					stack.Pop();

					++tokenOffset;
				}
				else if (Compiler.Table.TryGetValue(stack.Peek(), out var ruleTable) &&
					ruleTable.TryGetValue(tokens[tokenOffset], out var rule))
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
							stack.Push(Token.CloseTag);
							stack.Push(Token.Identifier);
							stack.Push(Token.OpenTag);

							break;
						}

						case 3:
						{
							stack.Push(Token.Text);

							break;
						}
					}
				}
				else
				{
					throw new Exception("Parsing Error.");
				}
			}
		}
	}
}
