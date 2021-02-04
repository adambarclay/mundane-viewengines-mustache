using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_MustacheViewEngine
{
	[ExcludeFromCodeCoverage]
	public static class MustacheView_Throws_TemplateNotFound
	{
		[Theory]
		[ClassData(typeof(MustacheViewNoModelTheoryData))]
		public static async Task When_The_Template_Is_Not_Found(MustacheViewNoModel entryPoint)
		{
			const string template = "Index.html";

			var exception = await Assert.ThrowsAnyAsync<TemplateNotFound>(
				async () => await entryPoint(new MustacheViews(new NullFileProvider()), template));

			Assert.Equal("Template \"" + template + "\" was not found.", exception.Message);
		}
	}
}
