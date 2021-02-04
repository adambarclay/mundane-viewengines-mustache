using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_MustacheViewEngine
{
	[ExcludeFromCodeCoverage]
	public static class MustacheView_Produces_Identical_Output
	{
		[Theory]
		[ClassData(typeof(MustacheViewWithModelTheoryData))]
		public static async Task When_The_File_Name_Has_A_Leading_Slash_And_When_It_Hasnt(
			MustacheViewWithModel entryPoint)
		{
			var views = new MustacheViews(
				new ManifestEmbeddedFileProvider(
					typeof(Helper).Assembly,
					"/Templates/SimpleSubstitutions/WhitespaceVariations"));

			var viewModel = new
			{
				Title = "Simple Substitutions",
				Nested = new { Value = "Nested Value" }
			};

			Assert.Equal(
				await entryPoint(views, "WhitespaceVariations.html", viewModel),
				await entryPoint(views, "/WhitespaceVariations.html", viewModel));
		}
	}
}
