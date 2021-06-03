using System.Collections.Generic;
using Mundane.ViewEngines.Mustache.Engine;

namespace Mundane.ViewEngines.Mustache.Compilation
{
	internal static class CodeGenerator
	{
		internal static CompiledProgram GenerateCode(List<Token> tokens, List<string> scannedliterals)
		{
			var instructions = new List<Instruction>();
			var literals = new List<string>();
			var identifiers = new List<string[]>();
			var templatePaths = new List<string>();

			var blockStack = new Stack<(TokenType, int)>();

			var scannedLiteralOffset = 0;

			var partial = false;
			var raw = false;

			for (var tokenOffset = 0; tokenOffset < tokens.Count; tokenOffset++)
			{
				switch (tokens[tokenOffset].TokenType)
				{
					case TokenType.OpenBlock:
					{
						instructions.Add(new Instruction(InstructionType.PushValue, identifiers.Count));

						identifiers.Add(scannedliterals[scannedLiteralOffset++].Split('.'));

						blockStack.Push((TokenType.OpenBlock, instructions.Count));

						instructions.Add(new Instruction(InstructionType.BranchIfFalsy, 0));

						++tokenOffset;

						break;
					}

					case TokenType.InvertedBlock:
					{
						instructions.Add(new Instruction(InstructionType.PushValue, identifiers.Count));

						identifiers.Add(scannedliterals[scannedLiteralOffset++].Split('.'));

						blockStack.Push((TokenType.InvertedBlock, instructions.Count));

						instructions.Add(new Instruction(InstructionType.BranchIfTruthy, 0));

						++tokenOffset;

						break;
					}

					case TokenType.CloseBlock:
					{
						(var blockType, var loopStart) = blockStack.Pop();

						if (blockType == TokenType.OpenBlock)
						{
							instructions.Add(new Instruction(InstructionType.Loop, loopStart + 1));
						}

						instructions[loopStart] = new Instruction(
							instructions[loopStart].InstructionType,
							instructions.Count);

						++scannedLiteralOffset;
						++tokenOffset;

						break;
					}

					case TokenType.Text:
					{
						instructions.Add(new Instruction(InstructionType.Literal, literals.Count));

						literals.Add(scannedliterals[scannedLiteralOffset++]);

						break;
					}

					case TokenType.Identifier:
					{
						if (partial)
						{
							instructions.Add(new Instruction(InstructionType.Call, templatePaths.Count));

							templatePaths.Add(scannedliterals[scannedLiteralOffset++]);

							partial = false;
						}
						else if (raw)
						{
							instructions.Add(new Instruction(InstructionType.OutputValueRaw, identifiers.Count));

							identifiers.Add(scannedliterals[scannedLiteralOffset++].Split('.'));

							raw = false;
						}
						else
						{
							instructions.Add(new Instruction(InstructionType.OutputValue, identifiers.Count));

							identifiers.Add(scannedliterals[scannedLiteralOffset++].Split('.'));
						}

						break;
					}

					case TokenType.Raw:
					{
						raw = true;

						break;
					}

					case TokenType.Partial:
					{
						partial = true;

						break;
					}

					case TokenType.End:
					{
						instructions.Add(new Instruction(InstructionType.Return, 0));

						break;
					}
				}
			}

			return new CompiledProgram(instructions, literals, identifiers, templatePaths);
		}
	}
}
