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
			var replacementPlaceholders = new Dictionary<string, Dictionary<string, int>>(programs.Count);

			var instructionCount = 0;
			var literalCount = 0;
			var identifierCount = 0;
			var replacementCount = 0;

			foreach ((var path, var compiledProgram) in programs)
			{
				entryPoints.Add(path, instructionCount);

				instructionCount += compiledProgram.Instructions.Count;
				literalCount += compiledProgram.Literals.Count;
				identifierCount += compiledProgram.Identifiers.Count;
				replacementCount += compiledProgram.Replacements.Count;

				replacementPlaceholders.Add(path, compiledProgram.ReplacementPlaceholders);
			}

			var programInstructions = new Instruction[instructionCount];
			var programLiterals = new byte[literalCount][];
			var programIdentifiers = new string[identifierCount][];
			var replacements = new Replacement[replacementCount][];

			var instructionOffset = 0;
			var literalOffset = 0;
			var identifierOffset = 0;
			var replacementsOffset = 0;

			foreach ((var currentPath, var compiledProgram) in programs)
			{
				compiledProgram.Instructions.CopyTo(programInstructions, instructionOffset);

				for (var currentInstructionOffset = 0;
					currentInstructionOffset < compiledProgram.Instructions.Count;
					++currentInstructionOffset)
				{
					var instruction = compiledProgram.Instructions[currentInstructionOffset];

					if (instruction.InstructionType == InstructionType.Literal)
					{
						programInstructions[instructionOffset + currentInstructionOffset] = new Instruction(
							instruction.InstructionType,
							instruction.Parameter + literalOffset);
					}
					else if (instruction.InstructionType == InstructionType.OutputValue ||
						instruction.InstructionType == InstructionType.OutputValueRaw ||
						instruction.InstructionType == InstructionType.PushValue ||
						instruction.InstructionType == InstructionType.ResolveUrl)
					{
						programInstructions[instructionOffset + currentInstructionOffset] = new Instruction(
							instruction.InstructionType,
							instruction.Parameter + identifierOffset);
					}
					else if (instruction.InstructionType == InstructionType.BranchIfFalsy ||
						instruction.InstructionType == InstructionType.BranchIfTruthy ||
						instruction.InstructionType == InstructionType.Loop)
					{
						programInstructions[instructionOffset + currentInstructionOffset] = new Instruction(
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

						programInstructions[instructionOffset + currentInstructionOffset] = new Instruction(
							instruction.InstructionType,
							entryPoint);
					}
					else if (instruction.InstructionType == InstructionType.PushReplacements)
					{
						var templatePath =
							compiledProgram.TemplatePaths[compiledProgram.Instructions[currentInstructionOffset + 1]
								.Parameter];

						var templateFileName = Linker.ResolvePath(
							Path.Combine(Path.GetDirectoryName(currentPath)!, templatePath));

						if (!replacementPlaceholders.TryGetValue(templateFileName, out var placeholders))
						{
							throw new TemplateNotFound(templateFileName);
						}

						var replacementsArray = new Replacement[placeholders.Count];

						var currentReplacements = compiledProgram.Replacements[instruction.Parameter];

						foreach ((var replacementName, var replacementOffset) in placeholders)
						{
							var replacementSupplied = currentReplacements.TryGetValue(
								replacementName,
								out var replacementEntryPoint);

							replacementsArray[replacementOffset] = new Replacement(
								replacementSupplied,
								instructionOffset + replacementEntryPoint);
						}

						programInstructions[instructionOffset + currentInstructionOffset] = new Instruction(
							instruction.InstructionType,
							instruction.Parameter + replacementsOffset);

						replacements[replacementsOffset++] = replacementsArray;
					}
					else
					{
						programInstructions[instructionOffset + currentInstructionOffset] =
							compiledProgram.Instructions[currentInstructionOffset];
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

			return (new ViewProgram(programInstructions, programLiterals, programIdentifiers, replacements),
				new ReadOnlyDictionary<string, int>(entryPoints));
		}

		private static string ResolvePath(string path)
		{
			return Path.GetFullPath(path)[Linker.PathRoot.Length..]!.Replace('\\', '/');
		}
	}
}
