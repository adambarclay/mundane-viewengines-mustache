namespace Mundane.ViewEngines.Mustache.Compilation
{
	internal enum Token
	{
		Text,
		OpenTag,
		CloseTag,
		Identifier,
		End,
		Program
	}
}
