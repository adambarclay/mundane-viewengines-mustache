using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_MustacheViewEngine
{
	[ExcludeFromCodeCoverage]
	public static class MustacheView_Writes_Template_With_Layout
	{
		[Theory]
		[ClassData(typeof(MustacheViewNoModelTheoryData))]
		public static async Task When_The_Template_Does_Not_Contain_Replacements(MustacheViewNoModel entryPoint)
		{
			var views = new MustacheViews(
				new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/Layout"));

			Assert.Equal(
				await Helper.Results("Layout/LayoutWithDefaults.html"),
				await entryPoint(views, "LayoutWithDefaults.html"));
		}

		[Theory]
		[ClassData(typeof(MustacheViewNoModelTheoryData))]
		public static async Task When_The_Template_Replaces_All_Of_The_Values(MustacheViewNoModel entryPoint)
		{
			var views = new MustacheViews(
				new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/Layout"));

			var a = await Helper.Results("Layout/LayoutWithAllValues.html");
			var b = await entryPoint(views, "LayoutWithAllValues.html");

			Assert.Equal(a, b);
		}
	}
}
