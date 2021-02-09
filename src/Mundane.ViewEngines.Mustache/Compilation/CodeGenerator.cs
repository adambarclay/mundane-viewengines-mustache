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

			var blockStack = new Stack<int>();

			var scannedLiteralOffset = 0;

			for (var tokenOffset = 0; tokenOffset < tokens.Count; tokenOffset++)
			{
				switch (tokens[tokenOffset].TokenType)
				{
					case TokenType.OpenBlock:
					{
						instructions.Add(new Instruction(InstructionType.Truthiness, identifiers.Count));

						identifiers.Add(scannedliterals[scannedLiteralOffset++].Split('.'));

						blockStack.Push(instructions.Count);

						instructions.Add(new Instruction(InstructionType.BranchIfFalse, 0));

						++tokenOffset;

						break;
					}

					case TokenType.InvertedBlock:
					{
						instructions.Add(new Instruction(InstructionType.Falsiness, identifiers.Count));

						identifiers.Add(scannedliterals[scannedLiteralOffset++].Split('.'));

						blockStack.Push(instructions.Count);

						instructions.Add(new Instruction(InstructionType.BranchIfFalse, 0));

						++tokenOffset;

						break;
					}

					case TokenType.CloseBlock:
					{
						instructions[blockStack.Pop()] = new Instruction(
							InstructionType.BranchIfFalse,
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
						instructions.Add(new Instruction(InstructionType.OutputValue, identifiers.Count));

						identifiers.Add(scannedliterals[scannedLiteralOffset++].Split('.'));

						break;
					}

					case TokenType.End:
					{
						instructions.Add(new Instruction(InstructionType.Return, 0));

						break;
					}
				}
			}

			return new CompiledProgram(instructions, literals, identifiers);
		}
	}
}
