using System.Diagnostics;

namespace Mundane.ViewEngines.Mustache.Engine;

[DebuggerDisplay("{" + nameof(Instruction.InstructionType) + "}: {" + nameof(Instruction.Parameter) + "}")]
internal readonly struct Instruction
{
	internal readonly InstructionType InstructionType;
	internal readonly int Parameter;

	internal Instruction(InstructionType instructionType, int parameter)
	{
		this.InstructionType = instructionType;
		this.Parameter = parameter;
	}
}
