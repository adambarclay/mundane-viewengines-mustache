using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_MustacheViewEngine
{
	[ExcludeFromCodeCoverage]
	public static class MustacheView_Writes_Template_With_Simple_Substitutions
	{
		[Fact]
		public static async Task When_The_Template_Simple_Substitution_Parameters()
		{
			var views = new MustacheViews(new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates"));

			const string templatePath = "SimpleSubstitutions/SimpleSubstitutions.html";

			var viewModel = new
			{
				Title = "Simple Substitutions",
				Nested = new { Value = "Nested Value" }
			};

			Assert.Equal(
				await Helper.Results(templatePath),
				await Helper.Execute(views, o => o.MustacheView(templatePath, viewModel)));

			Assert.Equal(
				await Helper.Results(templatePath),
				await MustacheViewEngine.MustacheView(views, templatePath, viewModel));
		}
	}
}
