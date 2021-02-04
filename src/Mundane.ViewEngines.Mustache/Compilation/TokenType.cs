namespace Mundane.ViewEngines.Mustache.Compilation
{
	internal enum TokenType
	{
		Text,
		OpenTag,
		CloseTag,
		Identifier,
		End,
		Program
	}
}
