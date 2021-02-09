namespace Mundane.ViewEngines.Mustache.Engine
{
	internal enum InstructionType
	{
		BranchIfFalse,
		Falsiness,
		Literal,
		OutputValue,
		Return,
		Truthiness
	}
}
