using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_Views
{
	[ExcludeFromCodeCoverage]
	public static class Constructor_Throws_ArgumentNullException
	{
		[Fact]
		public static void When_The_ViewFileProvider_Parameter_Is_Null()
		{
			var exception = Assert.ThrowsAny<ArgumentNullException>(() => new MustacheViews(null!));

			Assert.Same("viewFileProvider", exception.ParamName);
		}
	}
}
