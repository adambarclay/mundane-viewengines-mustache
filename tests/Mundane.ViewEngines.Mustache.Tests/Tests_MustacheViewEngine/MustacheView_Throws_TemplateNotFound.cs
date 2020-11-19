using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_MustacheViewEngine
{
	[ExcludeFromCodeCoverage]
	public static class MustacheView_Throws_TemplateNotFound
	{
		[Fact]
		public static async Task When_The_Template_Is_Not_Found()
		{
			const string template = "Index.html";
			const string expected = "Template \"" + template + "\" was not found.";

			var views = Helper.NotFound(template);

			var exceptionStreamNoViewModel = await Assert.ThrowsAnyAsync<TemplateNotFound>(
				async () => await Helper.Execute(views, o => o.MustacheView(template)));

			Assert.Equal(expected, exceptionStreamNoViewModel.Message);

			var exceptionStreamViewModel = await Assert.ThrowsAnyAsync<TemplateNotFound>(
				async () => await Helper.Execute(views, o => o.MustacheView(template, new object())));

			Assert.Equal(expected, exceptionStreamViewModel.Message);

			var exceptionStringNoViewModel = await Assert.ThrowsAnyAsync<TemplateNotFound>(
				async () => await MustacheViewEngine.MustacheView(views, template));

			Assert.Equal(expected, exceptionStringNoViewModel.Message);

			var exceptionStringViewModel = await Assert.ThrowsAnyAsync<TemplateNotFound>(
				async () => await MustacheViewEngine.MustacheView(views, template, new object()));

			Assert.Equal(expected, exceptionStringViewModel.Message);
		}
	}
}
