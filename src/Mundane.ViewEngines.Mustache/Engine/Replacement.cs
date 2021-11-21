namespace Mundane.ViewEngines.Mustache.Engine;

internal readonly struct Replacement
{
	internal readonly int ReplacementEntryPoint;

	internal readonly bool ReplacementSupplied;

	internal Replacement(bool replacementSupplied, int replacementEntryPoint)
	{
		this.ReplacementSupplied = replacementSupplied;
		this.ReplacementEntryPoint = replacementEntryPoint;
	}
}
