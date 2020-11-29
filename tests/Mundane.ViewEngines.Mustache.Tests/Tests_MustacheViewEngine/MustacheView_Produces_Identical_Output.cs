using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_MustacheViewEngine
{
	[ExcludeFromCodeCoverage]
	public static class MustacheView_Produces_Identical_Output
	{
		[Fact]
		public static async Task When_The_File_Name_Has_A_Leading_Slash_And_When_It_Hasnt()
		{
			var views = new MustacheViews(new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates"));

			const string templatePath = "SimpleSubstitutions/SimpleSubstitutions.html";
			const string templatePathSlash = "/SimpleSubstitutions/SimpleSubstitutions.html";

			var viewModel = new
			{
				Title = "Simple Substitutions",
				Nested = new { Value = "Nested Value" }
			};

			Assert.Equal(
				await Helper.Execute(views, o => o.MustacheView(templatePath, viewModel)),
				await Helper.Execute(views, o => o.MustacheView(templatePathSlash, viewModel)));

			Assert.Equal(
				await MustacheViewEngine.MustacheView(views, templatePath, viewModel),
				await MustacheViewEngine.MustacheView(views, templatePathSlash, viewModel));
		}
	}
}
