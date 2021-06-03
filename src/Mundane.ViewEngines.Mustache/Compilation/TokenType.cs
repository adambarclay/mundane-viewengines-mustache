namespace Mundane.ViewEngines.Mustache.Compilation
{
	internal enum TokenType
	{
		Block,
		CloseBlock,
		CloseTag,
		End,
		Epsilon,
		Identifier,
		InvertedBlock,
		OpenBlock,
		OpenTag,
		Partial,
		Program,
		Raw,
		Text
	}
}
