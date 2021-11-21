namespace Mundane.ViewEngines.Mustache.Compilation;

internal enum TokenType
{
	Block,
	CloseBlock,
	CloseTag,
	End,
	Identifier,
	InvertedBlock,
	LayoutBlock,
	OpenBlock,
	OpenTag,
	Partial,
	Program,
	Raw,
	Replacement,
	ReplacementBlock,
	Section,
	Text,
	Url
}
