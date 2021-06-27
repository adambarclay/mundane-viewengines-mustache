using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_MustacheViews
{
	[ExcludeFromCodeCoverage]
	public static class Error_LineNumbers_Are_Reported_Correctly
	{
		[Fact]
		public static void With_Linux_New_Line_Characters()
		{
			var exception = Assert.ThrowsAny<MustacheCompilerError>(
				() => new MustacheViews(
					new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Errors/LineNumberReporting/Linux")));

			Assert.Equal("/Linux.dat Ln 4 Ch 19: }} expected.", exception.Message);
		}

		[Fact]
		public static void With_Mac_New_Line_Characters()
		{
			var exception = Assert.ThrowsAny<MustacheCompilerError>(
				() => new MustacheViews(
					new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Errors/LineNumberReporting/Mac")));

			Assert.Equal("/Mac.dat Ln 4 Ch 19: }} expected.", exception.Message);
		}

		[Fact]
		public static void With_Windows_New_Line_Characters()
		{
			var exception = Assert.ThrowsAny<MustacheCompilerError>(
				() => new MustacheViews(
					new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Errors/LineNumberReporting/Windows")));

			Assert.Equal("/Windows.dat Ln 4 Ch 19: }} expected.", exception.Message);
		}
	}
}
