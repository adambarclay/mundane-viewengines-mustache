using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_MustacheViews
{
	[ExcludeFromCodeCoverage]
	public static class Constructor_Throws_ArgumentNullException
	{
		[Fact]
		public static void When_The_UrlResolver_Parameter_Is_Null()
		{
			var exception =
				Assert.ThrowsAny<ArgumentNullException>(() => new MustacheViews(new NullFileProvider(), null!));

			Assert.Same("urlResolver", exception.ParamName!);
		}

		[Fact]
		public static void When_The_ViewFileProvider_Parameter_Is_Null()
		{
			var exception1 = Assert.ThrowsAny<ArgumentNullException>(() => new MustacheViews(null!));

			Assert.Same("viewFileProvider", exception1.ParamName!);

			var exception2 = Assert.ThrowsAny<ArgumentNullException>(
				() => new MustacheViews(null!, (_, _) => string.Empty));

			Assert.Same("viewFileProvider", exception2.ParamName!);
		}
	}
}
