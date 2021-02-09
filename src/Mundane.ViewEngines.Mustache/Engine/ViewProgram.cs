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

			var truthyRegister = false;

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
							ViewProgram.GetValueBytes(viewModel, this.identifiers[instruction.Parameter]));

						break;
					}

					case InstructionType.Truthiness:
					{
						truthyRegister = ViewProgram.Truthy(
							ViewProgram.GetValue(viewModel, this.identifiers[instruction.Parameter]));

						break;
					}

					case InstructionType.Falsiness:
					{
						truthyRegister = !ViewProgram.Truthy(
							ViewProgram.GetValue(viewModel, this.identifiers[instruction.Parameter]));

						break;
					}

					case InstructionType.BranchIfFalse:
					{
						if (!truthyRegister)
						{
							programStack[stackCounter] = instruction.Parameter;
						}

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

		private static object? GetValue(object viewModel, string[] identifiers)
		{
			var valueObject = viewModel;

			foreach (var identifier in identifiers)
			{
				if (valueObject == null)
				{
					throw new ViewModelPropertyNotFound(identifiers);
				}

				if (valueObject is IDictionary dictionaryModel)
				{
					if (dictionaryModel.Contains(identifier))
					{
						valueObject = dictionaryModel[identifier];
					}
					else
					{
						throw new ViewModelPropertyNotFound(identifiers);
					}
				}
				else
				{
					var type = valueObject.GetType();

					var property = type.GetProperty(identifier, ViewProgram.ValueFlags);

					if (property != null)
					{
						valueObject = property.GetValue(valueObject, null);
					}
					else
					{
						var field = type.GetField(identifier, ViewProgram.ValueFlags);

						if (field != null)
						{
							valueObject = field.GetValue(valueObject);
						}
						else
						{
							throw new ViewModelPropertyNotFound(identifiers);
						}
					}
				}
			}

			return valueObject;
		}

		private static byte[] GetValueBytes(object viewModel, string[] identifiers)
		{
			var valueObject = ViewProgram.GetValue(viewModel, identifiers);

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

		private static bool Truthy(object? value)
		{
			if (value is null)
			{
				return false;
			}

			if (value is bool boolValue)
			{
				return boolValue;
			}

			if (value is string { Length: 0 })
			{
				return false;
			}

			if (value is sbyte sbyteValue && sbyteValue == 0)
			{
				return false;
			}

			if (value is byte byteValue && byteValue == 0)
			{
				return false;
			}

			if (value is short shortValue && shortValue == 0)
			{
				return false;
			}

			if (value is ushort ushortValue && ushortValue == 0)
			{
				return false;
			}

			if (value is int intValue && intValue == 0)
			{
				return false;
			}

			if (value is uint uintValue && uintValue == 0)
			{
				return false;
			}

			if (value is long longValue && longValue == 0)
			{
				return false;
			}

			if (value is ulong ulongValue && ulongValue == 0)
			{
				return false;
			}

			if (value is float floatValue && floatValue == 0)
			{
				return false;
			}

			if (value is double doubleValue && doubleValue == 0)
			{
				return false;
			}

			if (value is decimal decimalValue && decimalValue == 0)
			{
				return false;
			}

			return true;
		}
	}
}
