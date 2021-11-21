using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_MustacheViewEngine;

[ExcludeFromCodeCoverage]
public static class MustacheView_Writes_Contents_Of_Block_Multiple_Times
{
	[Theory]
	[ClassData(typeof(MustacheViewWithModelTheoryData))]
	public static async Task When_The_Block_Condition_Is_A_Multi_Item_Enumerable(MustacheViewWithModel entryPoint)
	{
		var views = new MustacheViews(new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/Block"));

		var viewModel = new
		{
			Title = "Simple Block",
			BlockCondition = new[] { new object(), new object(), new object() },
			Value = "Block Contents"
		};

		Assert.Equal(
			await Helper.Results("Block/Multi.html"),
			await entryPoint(views, "Simple/Simple.html", viewModel));
	}
}
