using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_MustacheViewEngine
{
	[ExcludeFromCodeCoverage]
	public static class MustacheView_Writes_Template_Unchanged
	{
		[Fact]
		public static async Task When_The_Template_Contains_No_Substitution_Parameters()
		{
			var views = new MustacheViews(new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates"));

			const string templatePath = "NoSubstitutions.html";

			Assert.Equal(
				await Helper.Results(templatePath),
				await Helper.Execute(views, o => o.MustacheView(templatePath)));

			Assert.Equal(
				await Helper.Results(templatePath),
				await Helper.Execute(views, o => o.MustacheView(templatePath, new object())));

			Assert.Equal(
				await Helper.Results(templatePath),
				await MustacheViewEngine.MustacheView(views, templatePath));

			Assert.Equal(
				await Helper.Results(templatePath),
				await MustacheViewEngine.MustacheView(views, templatePath, new object()));
		}
	}
}
