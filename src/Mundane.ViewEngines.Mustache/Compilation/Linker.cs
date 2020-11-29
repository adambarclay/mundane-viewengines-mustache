using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mundane.ViewEngines.Mustache.Engine;

namespace Mundane.ViewEngines.Mustache.Compilation
{
	internal static class Linker
	{
		internal static (ViewProgram ViewProgram, ReadOnlyDictionary<string, int> EntryPoints) Link(
			List<(string Path, (List<Instruction> Instructions, List<string> Literals, List<string[]> Identifiers)
				Program)> programs)
		{
			var entryPoints = new Dictionary<string, int>(programs.Count);

			var instructionCount = 0;
			var literalCount = 0;
			var identifierCount = 0;

			foreach ((var path, (var instructions, var literals, var identifiers)) in programs)
			{
				entryPoints.Add(path, instructionCount);

				instructionCount += instructions.Count;
				literalCount += literals.Count;
				identifierCount += identifiers.Count;
			}

			var programInstructions = new Instruction[instructionCount];
			var programLiterals = new string[literalCount];
			var programIdentifiers = new string[identifierCount][];

			var instructionOffset = 0;
			var literalOffset = 0;
			var identifierOffset = 0;

			foreach ((var _, (var instructions, var literals, var identifiers)) in programs)
			{
				instructions.CopyTo(programInstructions, instructionOffset);

				for (var i = 0; i < instructions.Count; ++i)
				{
					var instruction = instructions[i];

					if (instruction.InstructionType == InstructionType.Literal)
					{
						programInstructions[instructionOffset + i] = new Instruction(
							instruction.InstructionType,
							instruction.Parameter + literalOffset);
					}
					else if (instruction.InstructionType == InstructionType.OutputValue)
					{
						programInstructions[instructionOffset + i] = new Instruction(
							instruction.InstructionType,
							instruction.Parameter + identifierOffset);
					}
					else
					{
						programInstructions[instructionOffset + i] = instructions[i];
					}
				}

				literals.CopyTo(programLiterals, literalOffset);
				identifiers.CopyTo(programIdentifiers, identifierOffset);

				instructionOffset += instructions.Count;
				literalOffset += literals.Count;
				identifierOffset += identifiers.Count;
			}

			return (new ViewProgram(programInstructions, programLiterals, programIdentifiers),
				new ReadOnlyDictionary<string, int>(entryPoints));
		}
	}
}
