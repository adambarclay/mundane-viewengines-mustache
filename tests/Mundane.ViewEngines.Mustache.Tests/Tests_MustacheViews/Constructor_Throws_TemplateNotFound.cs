using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_MustacheViews;

[ExcludeFromCodeCoverage]
public static class Constructor_Throws_TemplateNotFound
{
	[Fact]
	public static void When_The_Layout_Is_Not_Found()
	{
		var exception = Assert.ThrowsAny<TemplateNotFound>(
			() => new MustacheViews(
				new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Errors/MissingLayout")));

		Assert.Equal("Template \"/Layout.html\" was not found.", exception.Message);
	}

	[Fact]
	public static void When_The_Partial_Is_Not_Found()
	{
		var exception = Assert.ThrowsAny<TemplateNotFound>(
			() => new MustacheViews(
				new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Errors/MissingPartial")));

		Assert.Equal("Template \"/Missing.html\" was not found.", exception.Message);
	}
}
