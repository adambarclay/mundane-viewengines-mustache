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
		Program,
		Text
	}
}
