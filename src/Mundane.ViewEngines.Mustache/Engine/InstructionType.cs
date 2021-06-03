namespace Mundane.ViewEngines.Mustache.Engine
{
	internal enum InstructionType
	{
		BranchIfFalsy,
		BranchIfTruthy,
		Call,
		Literal,
		Loop,
		OutputValue,
		OutputValueRaw,
		PushValue,
		Return
	}
}
