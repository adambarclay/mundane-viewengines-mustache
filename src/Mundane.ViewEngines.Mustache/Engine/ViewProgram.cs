using System;
using System.Collections;
using System.Collections.Generic;
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

			var objectStack = new Stack<object?>();
			var enumeratorStack = new Stack<IEnumerator>();

			programStack[stackCounter] = entryPoint;

			objectStack.Push(viewModel);

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
							ViewProgram.GetValueBytes(objectStack, this.identifiers[instruction.Parameter]));

						break;
					}

					case InstructionType.PushValue:
					{
						objectStack.Push(ViewProgram.GetValue(objectStack, this.identifiers[instruction.Parameter]));

						break;
					}

					case InstructionType.BranchIfFalsy:
					{
						if (ViewProgram.Falsy(objectStack.Peek(), out var enumerator))
						{
							objectStack.Pop();

							programStack[stackCounter] = instruction.Parameter;
						}
						else if (enumerator != null)
						{
							enumeratorStack.Push(enumerator);

							objectStack.Pop();
							objectStack.Push(enumerator.Current);
						}

						break;
					}

					case InstructionType.BranchIfTruthy:
					{
						if (!ViewProgram.Falsy(objectStack.Peek(), out var _))
						{
							objectStack.Pop();

							programStack[stackCounter] = instruction.Parameter;
						}

						break;
					}

					case InstructionType.Loop:
					{
						objectStack.Pop();

						if (enumeratorStack.TryPeek(out var enumerator))
						{
							if (enumerator.MoveNext())
							{
								objectStack.Push(enumerator.Current);

								programStack[stackCounter] = instruction.Parameter;
							}
							else
							{
								enumeratorStack.Pop();
							}
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

		private static bool Falsy(object? value, out IEnumerator? enumerator)
		{
			if (value is null)
			{
				enumerator = null;

				return true;
			}

			if (value is bool boolValue)
			{
				enumerator = null;

				return !boolValue;
			}

			if (value is string stringValue)
			{
				enumerator = null;

				return stringValue.Length == 0;
			}

			if (value is IEnumerable enumerableValue)
			{
				enumerator = enumerableValue.GetEnumerator();

				return !enumerator.MoveNext();
			}

			enumerator = null;

			switch (value)
			{
				case sbyte sbyteValue when sbyteValue == 0:
				case byte byteValue when byteValue == 0:
				case short shortValue when shortValue == 0:
				case ushort ushortValue when ushortValue == 0:
				case int intValue when intValue == 0:
				case uint uintValue when uintValue == 0:
				case long longValue when longValue == 0:
				case ulong ulongValue when ulongValue == 0:
				case float floatValue when floatValue == 0:
				case double doubleValue when doubleValue == 0:
				case decimal decimalValue when decimalValue == 0:
				{
					return true;
				}
			}

			return false;
		}

		private static (bool ValueFound, object? ValueObject) GetObjectValue(object? valueObject, string[] identifiers)
		{
			foreach (var identifier in identifiers)
			{
				if (valueObject == null)
				{
					return (false, null);
				}

				if (valueObject is IDictionary dictionaryModel)
				{
					if (dictionaryModel.Contains(identifier))
					{
						valueObject = dictionaryModel[identifier];
					}
					else
					{
						return (false, null);
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
							return (false, null);
						}
					}
				}
			}

			return (true, valueObject);
		}

		private static object? GetValue(Stack<object?> objectStack, string[] identifiers)
		{
			foreach (var topofStack in objectStack)
			{
				(var valueFound, var valueObject) = ViewProgram.GetObjectValue(topofStack, identifiers);

				if (valueFound)
				{
					return valueObject;
				}
			}

			throw new ViewModelPropertyNotFound(identifiers);
		}

		private static byte[] GetValueBytes(Stack<object?> objectStack, string[] identifiers)
		{
			var valueObject = ViewProgram.GetValue(objectStack, identifiers);

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
