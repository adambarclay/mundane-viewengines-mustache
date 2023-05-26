using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_MustacheViewEngine;

[ExcludeFromCodeCoverage]
public static class MustacheView_Writes_Contents_Of_Nested_Block
{
	[Theory]
	[ClassData(typeof(MustacheViewWithModelTheoryData))]
	public static async Task When_The_Block_Condition_Is_A_Multi_Item_Enumerable(MustacheViewWithModel entryPoint)
	{
		var views = new MustacheViews(new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/Block"));

		var viewModel = new
		{
			Title = "Nested Block",
			BlockCondition = new object[] { new { Value = 1, Nested = "Hello" }, new { Value = 2, Nested = "5" }, new { Value = 3, Nested = string.Empty }, new { Value = 4, Nested = (string?)null } },
		};

		var a = await Helper.Results("Block/Nested.html");
		var b = await entryPoint(views, "Nested/Nested.html", viewModel);

		Assert.Equal(a, b);
	}
}
