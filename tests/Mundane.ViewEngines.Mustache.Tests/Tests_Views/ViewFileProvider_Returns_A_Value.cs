using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.FileProviders;
using Moq;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_Views
{
	[ExcludeFromCodeCoverage]
	public static class ViewFileProvider_Returns_A_Value
	{
		[Fact]
		public static void Which_Was_Passed_To_The_Constructor()
		{
			var viewFileProvider = new Mock<IFileProvider>(MockBehavior.Strict).Object;

			Assert.Same(viewFileProvider, new Views(viewFileProvider).ViewFileProvider);
		}
	}
}
