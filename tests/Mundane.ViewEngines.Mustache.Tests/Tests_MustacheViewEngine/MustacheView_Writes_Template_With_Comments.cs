using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_MustacheViewEngine
{
	[ExcludeFromCodeCoverage]
	public static class MustacheView_Writes_Template_With_Comments
	{
		[Theory]
		[ClassData(typeof(MustacheViewWithModelTheoryData))]
		public static async Task When_The_Comment_Contains_A_Single_Close_Brace(MustacheViewWithModel entryPoint)
		{
			var views = new MustacheViews(
				new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/Comments"));

			var viewModel = new { Title = "Title" };

			Assert.Equal(
				await Helper.Results("Comments/Comments.html"),
				await entryPoint(views, "CommentsWithSingleCloseBrace.html", viewModel));
		}

		[Theory]
		[ClassData(typeof(MustacheViewWithModelTheoryData))]
		public static async Task Without_Outputting_The_Comments(MustacheViewWithModel entryPoint)
		{
			var views = new MustacheViews(
				new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/Comments"));

			var viewModel = new { Title = "Title" };

			Assert.Equal(
				await Helper.Results("Comments/Comments.html"),
				await entryPoint(views, "Comments.html", viewModel));
		}
	}
}
