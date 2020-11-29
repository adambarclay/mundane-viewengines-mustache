using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_MustacheViewEngine
{
	[ExcludeFromCodeCoverage]
	public static class MustacheView_Throws_DependencyNotFound
	{
		[Fact]
		public static async Task When_Dependency_Returns_Null()
		{
			var dependencyFinder = new Mock<DependencyFinder>(MockBehavior.Strict);

			dependencyFinder.Setup(o => o.Find<MustacheViews>(It.IsAny<Request>())).Returns((MustacheViews)null!);

			var exceptionStreamNoViewModel = await Assert.ThrowsAnyAsync<DependencyNotFound>(
				async () => await Helper.Execute(
					dependencyFinder.Object,
					o => o.MustacheView(Guid.NewGuid().ToString())));

			Assert.Equal(
				"No concrete type has been registered for \"MustacheViews\".",
				exceptionStreamNoViewModel.Message);

			var exceptionStreamViewModel = await Assert.ThrowsAnyAsync<DependencyNotFound>(
				async () => await Helper.Execute(
					dependencyFinder.Object,
					o => o.MustacheView(Guid.NewGuid().ToString(), new object())));

			Assert.Equal(
				"No concrete type has been registered for \"MustacheViews\".",
				exceptionStreamViewModel.Message);
		}
	}
}
