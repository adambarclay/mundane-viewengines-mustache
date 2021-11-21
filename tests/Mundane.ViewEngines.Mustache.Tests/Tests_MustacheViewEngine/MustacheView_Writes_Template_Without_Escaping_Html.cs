using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_MustacheViewEngine;

[ExcludeFromCodeCoverage]
public static class MustacheView_Writes_Template_Without_Escaping_Html
{
	[Theory]
	[ClassData(typeof(MustacheViewWithModelTheoryData))]
	public static async Task When_Using_The_Raw_Tag(MustacheViewWithModel entryPoint)
	{
		var views = new MustacheViews(new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/Raw"));

		var viewModel = new
		{
			Title = "Output Raw Value",
			Value = "<script>alert(\"Hello World!\");</script>"
		};

		var x = await Helper.Results("Raw/Raw.html");
		var y = await entryPoint(views, "Raw.html", viewModel);

		Assert.Equal(x, y);
	}
}
