using System.Collections.Generic;
using Mundane.ViewEngines.Mustache.Engine;

namespace Mundane.ViewEngines.Mustache.Compilation;

internal static class CodeGenerator
{
	internal static CompiledProgram GenerateCode(List<Token> tokens, List<string> scannedliterals)
	{
		var instructions = new List<List<Instruction>> { new List<Instruction>() };
		var instructionsOffset = 0;

		var literals = new List<string>();
		var identifiers = new List<string[]>();
		var templatePaths = new List<string>();
		var replacementPlaceholders = new Dictionary<string, int>();
		var replacements = new List<Dictionary<string, int>>();

		var blockStack = new Stack<(TokenType TokenType, int Parameter)>();
		var replacementsStack = new Stack<Dictionary<string, int>>();
		var scannedLiteralOffset = 0;
		var identifierType = TokenType.Identifier;

		for (var tokenOffset = 0; tokenOffset < tokens.Count; tokenOffset++)
		{
			switch (tokens[tokenOffset].TokenType)
			{
				case TokenType.OpenBlock:
				{
					instructions[instructionsOffset].Add(new Instruction(InstructionType.PushValue, identifiers.Count));

					identifiers.Add(scannedliterals[scannedLiteralOffset++].Split('.'));

					blockStack.Push((TokenType.OpenBlock, instructions[instructionsOffset].Count));

					instructions[instructionsOffset].Add(new Instruction(InstructionType.BranchIfFalsy, 0));

					++tokenOffset;

					break;
				}

				case TokenType.InvertedBlock:
				{
					instructions[instructionsOffset].Add(new Instruction(InstructionType.PushValue, identifiers.Count));

					identifiers.Add(scannedliterals[scannedLiteralOffset++].Split('.'));

					blockStack.Push((TokenType.InvertedBlock, instructions[instructionsOffset].Count));

					instructions[instructionsOffset].Add(new Instruction(InstructionType.BranchIfTruthy, 0));

					++tokenOffset;

					break;
				}

				case TokenType.LayoutBlock:
				{
					blockStack.Push((TokenType.LayoutBlock, 0));

					instructions[instructionsOffset]
						.Add(new Instruction(InstructionType.PushReplacements, templatePaths.Count));

					instructions[instructionsOffset].Add(new Instruction(InstructionType.Call, templatePaths.Count));

					var newReplacements = new Dictionary<string, int>();

					replacementsStack.Push(newReplacements);
					replacements.Add(newReplacements);

					templatePaths.Add(scannedliterals[scannedLiteralOffset++]);

					++tokenOffset;

					break;
				}

				case TokenType.ReplacementBlock:
				{
					if (blockStack.TryPeek(out var topOfStack) && topOfStack.TokenType == TokenType.LayoutBlock)
					{
						blockStack.Push((TokenType.ReplacementBlock, instructions[instructionsOffset].Count));

						++instructionsOffset;

						if (instructions.Count == instructionsOffset)
						{
							instructions.Add(new List<Instruction>());
						}

						replacementsStack.Peek()
							.Add(scannedliterals[scannedLiteralOffset++], instructions[instructionsOffset].Count);

						++tokenOffset;
					}
					else
					{
						if (!replacementPlaceholders.ContainsKey(scannedliterals[scannedLiteralOffset]))
						{
							replacementPlaceholders.Add(
								scannedliterals[scannedLiteralOffset],
								replacementPlaceholders.Count);
						}

						instructions[instructionsOffset]
							.Add(
								new Instruction(
									InstructionType.CallReplacement,
									replacementPlaceholders[scannedliterals[scannedLiteralOffset]]));

						blockStack.Push((TokenType.ReplacementBlock, instructions[instructionsOffset].Count));

						instructions[instructionsOffset].Add(new Instruction(InstructionType.BranchIfTruthy, 0));

						++scannedLiteralOffset;
						++tokenOffset;
					}

					break;
				}

				case TokenType.CloseBlock:
				{
					(var blockType, var parameter) = blockStack.Pop();

					if (blockType == TokenType.OpenBlock)
					{
						instructions[instructionsOffset].Add(new Instruction(InstructionType.Loop, parameter + 1));
					}

					if (blockType == TokenType.ReplacementBlock &&
						blockStack.TryPeek(out var topOfStack) &&
						topOfStack.TokenType == TokenType.LayoutBlock)
					{
						instructions[instructionsOffset].Add(new Instruction(InstructionType.Return, 0));

						--instructionsOffset;
					}
					else if (blockType == TokenType.LayoutBlock)
					{
						instructions[instructionsOffset].Add(new Instruction(InstructionType.PopReplacements, 0));
						replacementsStack.Pop();
					}
					else
					{
						instructions[instructionsOffset][parameter] = new Instruction(
							instructions[instructionsOffset][parameter].InstructionType,
							instructions[instructionsOffset].Count);
					}

					++scannedLiteralOffset;
					++tokenOffset;

					break;
				}

				case TokenType.Text:
				{
					instructions[instructionsOffset].Add(new Instruction(InstructionType.Literal, literals.Count));

					literals.Add(scannedliterals[scannedLiteralOffset++]);

					break;
				}

				case TokenType.Identifier:
				{
					if (identifierType == TokenType.Partial)
					{
						instructions[instructionsOffset]
							.Add(new Instruction(InstructionType.Call, templatePaths.Count));

						templatePaths.Add(scannedliterals[scannedLiteralOffset++]);
					}
					else if (identifierType == TokenType.Raw)
					{
						instructions[instructionsOffset]
							.Add(new Instruction(InstructionType.OutputValueRaw, identifiers.Count));

						identifiers.Add(scannedliterals[scannedLiteralOffset++].Split('.'));
					}
					else if (identifierType == TokenType.Url)
					{
						instructions[instructionsOffset]
							.Add(new Instruction(InstructionType.ResolveUrl, identifiers.Count));

						identifiers.Add(new[] { scannedliterals[scannedLiteralOffset++] });
					}
					else
					{
						instructions[instructionsOffset]
							.Add(new Instruction(InstructionType.OutputValue, identifiers.Count));

						identifiers.Add(scannedliterals[scannedLiteralOffset++].Split('.'));
					}

					identifierType = TokenType.Identifier;

					break;
				}

				case TokenType.Raw:
				{
					identifierType = TokenType.Raw;

					break;
				}

				case TokenType.Partial:
				{
					identifierType = TokenType.Partial;

					break;
				}

				case TokenType.Url:
				{
					identifierType = TokenType.Url;

					break;
				}

				case TokenType.End:
				{
					instructions[instructionsOffset].Add(new Instruction(InstructionType.Return, 0));

					break;
				}
			}
		}

		var instructionOffset = 0;

		for (var i = 1; i < instructions.Count; ++i)
		{
			var previousOffset = i - 1;

			instructionOffset += instructions[previousOffset].Count;

			foreach (var instruction in instructions[i])
			{
				if (instruction.InstructionType == InstructionType.BranchIfFalsy ||
					instruction.InstructionType == InstructionType.BranchIfTruthy ||
					instruction.InstructionType == InstructionType.Loop)
				{
					instructions[0]
						.Add(new Instruction(instruction.InstructionType, instruction.Parameter + instructionOffset));
				}
				else
				{
					instructions[0].Add(instruction);
				}
			}

			foreach (var key in replacements[previousOffset].Keys)
			{
				replacements[previousOffset][key] += instructionOffset;
			}
		}

		return new CompiledProgram(
			instructions[0],
			literals,
			identifiers,
			templatePaths,
			replacementPlaceholders,
			replacements);
	}
}
