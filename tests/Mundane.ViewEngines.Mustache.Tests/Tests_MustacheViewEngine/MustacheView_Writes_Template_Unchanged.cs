using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_MustacheViewEngine
{
	[ExcludeFromCodeCoverage]
	public static class MustacheView_Writes_Template_Unchanged
	{
		[Theory]
		[ClassData(typeof(MustacheViewNoModelTheoryData))]
		public static async Task When_The_Template_Contains_No_Substitution_Parameters(MustacheViewNoModel entryPoint)
		{
			var views = new MustacheViews(
				new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/NoSubstitutions"));

			const string templatePath = "NoSubstitutions.html";

			var results = await Helper.Results("NoSubstitutions/" + templatePath);

			Assert.Equal(results, await entryPoint(views, templatePath));
		}
	}
}
