using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mundane.ViewEngines.Mustache.Engine
{
	internal readonly struct ViewProgram
	{
		private const BindingFlags ValueFlags =
			BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

		private readonly string[][] identifiers;
		private readonly Instruction[] instructions;
		private readonly byte[][] literals;

		internal ViewProgram(Instruction[] instructions, byte[][] literals, string[][] identifiers)
		{
			this.instructions = instructions;
			this.literals = literals;
			this.identifiers = identifiers;
		}

		internal async ValueTask Execute<T>(Stream outputStream, int entryPoint, T viewModel)
			where T : notnull
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
						await outputStream.WriteAsync(this.literals[instruction.Parameter]);

						break;
					}

					case InstructionType.OutputValue:
					{
						await outputStream.WriteAsync(
							ViewProgram.GetValue(viewModel, this.identifiers[instruction.Parameter]));

						break;
					}

					case InstructionType.Return:
					{
						--stackCounter;

						break;
					}
				}
			}
		}

		private static object? GetPropertyValue(object viewModel, string propertyName, string[] identifiers)
		{
			if (viewModel is IDictionary dictionaryModel)
			{
				if (dictionaryModel.Contains(propertyName))
				{
					return dictionaryModel[propertyName];
				}
			}

			var type = viewModel.GetType();

			var property = type.GetProperty(propertyName, ViewProgram.ValueFlags);

			if (property != null)
			{
				return property.GetValue(viewModel, null);
			}

			var field = type.GetField(propertyName, ViewProgram.ValueFlags);

			if (field != null)
			{
				return field.GetValue(viewModel);
			}

			throw new ViewModelPropertyNotFound(identifiers);
		}

		private static byte[] GetValue(object viewModel, string[] identifiers)
		{
			var valueObject = viewModel;

			foreach (var identifier in identifiers)
			{
				if (valueObject == null)
				{
					throw new ViewModelPropertyNotFound(identifiers);
				}

				valueObject = ViewProgram.GetPropertyValue(valueObject, identifier, identifiers);
			}

			if (valueObject == null)
			{
				return Array.Empty<byte>();
			}

			var value = valueObject.ToString();

			if (value == null)
			{
				return Array.Empty<byte>();
			}

			return Encoding.UTF8.GetBytes(WebUtility.HtmlEncode(value));
		}
	}
}
