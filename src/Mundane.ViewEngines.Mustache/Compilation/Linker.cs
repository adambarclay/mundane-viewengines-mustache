using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Mundane.ViewEngines.Mustache.Engine;

namespace Mundane.ViewEngines.Mustache.Compilation
{
	internal static class Linker
	{
		private static readonly string PathRoot = Path.GetPathRoot(Environment.CurrentDirectory)![..^1]!;

		internal static (ViewProgram ViewProgram, ReadOnlyDictionary<string, int> EntryPoints) Link(
			List<(string Path, CompiledProgram Program)> programs)
		{
			var entryPoints = new Dictionary<string, int>(programs.Count);

			var instructionCount = 0;
			var literalCount = 0;
			var identifierCount = 0;

			foreach ((var path, var compiledProgram) in programs)
			{
				entryPoints.Add(path, instructionCount);

				instructionCount += compiledProgram.Instructions.Count;
				literalCount += compiledProgram.Literals.Count;
				identifierCount += compiledProgram.Identifiers.Count;
			}

			var programInstructions = new Instruction[instructionCount];
			var programLiterals = new byte[literalCount][];
			var programIdentifiers = new string[identifierCount][];

			var instructionOffset = 0;
			var literalOffset = 0;
			var identifierOffset = 0;

			foreach ((var currentPath, var compiledProgram) in programs)
			{
				compiledProgram.Instructions.CopyTo(programInstructions, instructionOffset);

				for (var i = 0; i < compiledProgram.Instructions.Count; ++i)
				{
					var instruction = compiledProgram.Instructions[i];

					if (instruction.InstructionType == InstructionType.Literal)
					{
						programInstructions[instructionOffset + i] = new Instruction(
							instruction.InstructionType,
							instruction.Parameter + literalOffset);
					}
					else if (instruction.InstructionType == InstructionType.OutputValue ||
						instruction.InstructionType == InstructionType.PushValue)
					{
						programInstructions[instructionOffset + i] = new Instruction(
							instruction.InstructionType,
							instruction.Parameter + identifierOffset);
					}
					else if (instruction.InstructionType == InstructionType.BranchIfFalsy ||
						instruction.InstructionType == InstructionType.BranchIfTruthy ||
						instruction.InstructionType == InstructionType.Loop)
					{
						programInstructions[instructionOffset + i] = new Instruction(
							instruction.InstructionType,
							instruction.Parameter + instructionOffset);
					}
					else if (instruction.InstructionType == InstructionType.Call)
					{
						var templateFileName = Linker.ResolvePath(
							Path.Combine(
								Path.GetDirectoryName(currentPath)!,
								compiledProgram.TemplatePaths[instruction.Parameter]));

						if (!entryPoints.TryGetValue(templateFileName, out var entryPoint))
						{
							throw new TemplateNotFound(templateFileName);
						}

						programInstructions[instructionOffset + i] = new Instruction(
							instruction.InstructionType,
							entryPoint);
					}
					else
					{
						programInstructions[instructionOffset + i] = compiledProgram.Instructions[i];
					}
				}

				for (var i = 0; i < compiledProgram.Literals.Count; ++i)
				{
					programLiterals[literalOffset + i] = Encoding.UTF8.GetBytes(compiledProgram.Literals[i]);
				}

				compiledProgram.Identifiers.CopyTo(programIdentifiers, identifierOffset);

				instructionOffset += compiledProgram.Instructions.Count;
				literalOffset += compiledProgram.Literals.Count;
				identifierOffset += compiledProgram.Identifiers.Count;
			}

			return (new ViewProgram(programInstructions, programLiterals, programIdentifiers),
				new ReadOnlyDictionary<string, int>(entryPoints));
		}

		private static string ResolvePath(string path)
		{
			return Path.GetFullPath(path)[Linker.PathRoot.Length..]!.Replace('\\', '/');
		}
	}
}
