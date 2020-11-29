using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace Mundane.ViewEngines.Mustache.Engine
{
	internal readonly struct ViewProgram
	{
		private const BindingFlags ValueFlags =
			BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

		private readonly string[][] identifiers;
		private readonly Instruction[] instructions;
		private readonly string[] literals;

		internal ViewProgram(Instruction[] instructions, string[] literals, string[][] identifiers)
		{
			this.instructions = instructions;
			this.literals = literals;
			this.identifiers = identifiers;
		}

		internal async Task Execute<T>(StreamWriter streamWriter, int entryPoint, T viewModel)
		{
			var programStack = new int[128];
			var stackCounter = 0;

			programStack[stackCounter] = entryPoint;

			while (stackCounter >= 0)
			{
				var instruction = this.instructions[programStack[stackCounter]++];

				switch (instruction.InstructionType)
				{
					case InstructionType.Literal:
					{
						await streamWriter.WriteAsync(this.literals[instruction.Parameter]);

						break;
					}

					case InstructionType.OutputValue:
					{
						await streamWriter.WriteAsync(
							WebUtility.HtmlEncode(
								ViewProgram.GetValue(viewModel, this.identifiers[instruction.Parameter])));

						break;
					}

					case InstructionType.Return:
					{
						--stackCounter;

						break;
					}

					default:
					{
						throw new InvalidOperationException(
							$"Instruction \"{instruction.InstructionType}\" not valid.");
					}
				}
			}
		}

		private static object? GetPropertyValue(object viewModel, string propertyName)
		{
			if (viewModel is IDictionary<string, object> dictionaryModel)
			{
				return dictionaryModel.TryGetValue(propertyName, out var output) ? output : null;
			}

			var type = viewModel.GetType();

			var property = type.GetProperty(propertyName, ViewProgram.ValueFlags);

			if (property != null)
			{
				return property.GetValue(viewModel, null);
			}

			var field = type.GetField(propertyName, ViewProgram.ValueFlags);

			return field?.GetValue(viewModel);
		}

		private static string GetValue(object? viewModel, string[] identifiers)
		{
			if (viewModel == null)
			{
				return string.Empty;
			}

			foreach (var identifier in identifiers)
			{
				viewModel = ViewProgram.GetPropertyValue(viewModel, identifier);

				if (viewModel == null)
				{
					return string.Empty;
				}
			}

			return viewModel.ToString() ?? string.Empty;
		}
	}
}
