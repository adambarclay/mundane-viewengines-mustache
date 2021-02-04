using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_MustacheViewEngine
{
	[ExcludeFromCodeCoverage]
	public static class MustacheView_Throws_DependencyNotFound
	{
		[Fact]
		public static async Task When_The_Views_Depencency_Is_Not_Found()
		{
			var responseStreamNoModel = await Assert.ThrowsAnyAsync<DependencyNotFound>(
				async () => await Helper.Run(new Dependencies(), o => o.MustacheView("Index.html")));

			Assert.Equal("No concrete type has been registered for \"MustacheViews\".", responseStreamNoModel.Message);

			var responseStreamModel = await Assert.ThrowsAnyAsync<DependencyNotFound>(
				async () => await Helper.Run(new Dependencies(), o => o.MustacheView("Index.html", new object())));

			Assert.Equal("No concrete type has been registered for \"MustacheViews\".", responseStreamModel.Message);
		}
	}
}
