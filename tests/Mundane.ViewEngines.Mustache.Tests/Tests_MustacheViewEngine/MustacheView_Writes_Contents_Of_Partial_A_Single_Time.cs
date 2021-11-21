using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_MustacheViewEngine;

[ExcludeFromCodeCoverage]
public static class MustacheView_Writes_Contents_Of_Partial_A_Single_Time
{
	[Theory]
	[ClassData(typeof(MustacheViewWithModelTheoryData))]
	public static async Task When_The_Partial_Is_In_A_Block(MustacheViewWithModel entryPoint)
	{
		var views = new MustacheViews(new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/Partials"));

		var viewModel = new
		{
			Title = "Simple Block",
			BlockCondition = true,
			Value = "Block Contents"
		};

		Assert.Equal(
			await Helper.Results("Block/Simple.html"),
			await entryPoint(views, "BlockPartial/BlockPartial.html", viewModel));
	}

	[Theory]
	[ClassData(typeof(MustacheViewWithModelTheoryData))]
	public static async Task When_The_Partial_Is_Not_In_A_Block(MustacheViewWithModel entryPoint)
	{
		var views = new MustacheViews(new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/Partials"));

		var viewModel = new
		{
			Title = "Simple Block",
			Value = "Block Contents"
		};

		Assert.Equal(
			await Helper.Results("Block/Simple.html"),
			await entryPoint(views, "SimplePartial/SimplePartial.html", viewModel));
	}
}
