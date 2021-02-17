namespace Mundane.ViewEngines.Mustache.Engine
{
	internal enum InstructionType
	{
		BranchIfFalsy,
		BranchIfTruthy,
		Literal,
		Loop,
		OutputValue,
		PushValue,
		Return
	}
}
