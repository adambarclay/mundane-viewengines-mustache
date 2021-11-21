using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_MustacheViewEngine;

[ExcludeFromCodeCoverage]
public static class MustacheView_Writes_Current_Iterator_Item_Multiple_Times
{
	[Theory]
	[ClassData(typeof(MustacheViewWithModelTheoryData))]
	public static async Task When_The_Block_Condition_Is_A_Multi_Item_Enumerable(MustacheViewWithModel entryPoint)
	{
		var views = new MustacheViews(new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/Block"));

		var viewModel = new
		{
			Title = "Simple Block",
			BlockCondition = new[] { 1, 2, 3, 4, 5 },
			Value = "Block Contents"
		};

		Assert.Equal(
			await Helper.Results("Block/OutputCurrent.html"),
			await entryPoint(views, "OutputCurrent/OutputCurrent.html", viewModel));
	}
}
