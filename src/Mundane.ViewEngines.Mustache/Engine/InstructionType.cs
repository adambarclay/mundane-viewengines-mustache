namespace Mundane.ViewEngines.Mustache.Engine;

internal enum InstructionType
{
	BranchIfFalsy,
	BranchIfTruthy,
	Call,
	CallReplacement,
	Literal,
	Loop,
	OutputValue,
	OutputValueRaw,
	PopReplacements,
	PushReplacements,
	PushValue,
	ResolveUrl,
	Return
}
