namespace Mundane.ViewEngines.Mustache
{
	/// <summary>Resolves parameters passed to the URL tag e.g. {{~ /some-url }}.</summary>
	/// <param name="pathBase">The application path base.</param>
	/// <param name="url">The URL tag parameter.</param>
	/// <returns>The resolved URL.</returns>
	public delegate string UrlResolver(string pathBase, string url);
}
