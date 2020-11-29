namespace Mundane.ViewEngines.Mustache.Engine
{
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
}
