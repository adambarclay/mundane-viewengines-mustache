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

			var scannedLiteralOffset = 0;

			var literalOffset = 0;
			var identifierOffset = 0;

			foreach (var token in tokens)
			{
				switch (token.TokenType)
				{
					case TokenType.Text:
					{
						literals.Add(scannedliterals[scannedLiteralOffset++]);

						instructions.Add(new Instruction(InstructionType.Literal, literalOffset++));

						break;
					}

					case TokenType.Identifier:
					{
						identifiers.Add(scannedliterals[scannedLiteralOffset++].Split('.'));

						instructions.Add(new Instruction(InstructionType.OutputValue, identifierOffset++));

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
