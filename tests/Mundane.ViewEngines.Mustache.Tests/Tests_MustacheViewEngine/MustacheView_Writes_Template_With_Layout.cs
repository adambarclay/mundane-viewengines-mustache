using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_MustacheViewEngine;

[ExcludeFromCodeCoverage]
public static class MustacheView_Writes_Template_With_Layout
{
	[Theory]
	[ClassData(typeof(MustacheViewNoModelTheoryData))]
	public static async Task When_The_Template_Contains_A_Nested_Layout(MustacheViewNoModel entryPoint)
	{
		var views = new MustacheViews(new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/Layout"));

		Assert.Equal(
			await Helper.Results("Layout/LayoutWithNestedLayout.html"),
			await entryPoint(views, "LayoutWithNestedLayout.html"));
	}

	[Theory]
	[ClassData(typeof(MustacheViewNoModelTheoryData))]
	public static async Task When_The_Template_Contains_A_Nested_Layout_With_Default_Values(
		MustacheViewNoModel entryPoint)
	{
		var views = new MustacheViews(new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/Layout"));

		var a = await Helper.Results("Layout/LayoutWithNestedLayoutDefaults.html");
		var b = await entryPoint(views, "LayoutWithNestedLayoutDefaults.html");

		Assert.Equal(a, b);
	}

	[Theory]
	[ClassData(typeof(MustacheViewWithModelTheoryData))]
	public static async Task When_The_Template_Contains_An_Enumerator(MustacheViewWithModel entryPoint)
	{
		var views = new MustacheViews(new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/Layout"));

		var viewModel = new { Items = new[] { 1, 2, 3 } };

		Assert.Equal(
			await Helper.Results("Layout/LayoutWithEnumerator.html"),
			await entryPoint(views, "LayoutWithEnumerator.html", viewModel));
	}

	[Theory]
	[ClassData(typeof(MustacheViewNoModelTheoryData))]
	public static async Task When_The_Template_Contains_Extra_Text_Outside_Of_The_Layout(MustacheViewNoModel entryPoint)
	{
		var views = new MustacheViews(new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/Layout"));

		Assert.Equal(
			await Helper.Results("Layout/LayoutWithExtraText.html"),
			await entryPoint(views, "LayoutWithExtraText.html"));
	}

	[Theory]
	[ClassData(typeof(MustacheViewNoModelTheoryData))]
	public static async Task When_The_Template_Does_Not_Contain_Replacements(MustacheViewNoModel entryPoint)
	{
		var views = new MustacheViews(new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/Layout"));

		Assert.Equal(
			await Helper.Results("Layout/LayoutWithDefaults.html"),
			await entryPoint(views, "LayoutWithDefaults.html"));
	}

	[Theory]
	[ClassData(typeof(MustacheViewNoModelTheoryData))]
	public static async Task When_The_Template_Replaces_All_Of_The_Values(MustacheViewNoModel entryPoint)
	{
		var views = new MustacheViews(new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/Layout"));

		Assert.Equal(
			await Helper.Results("Layout/LayoutWithAllValues.html"),
			await entryPoint(views, "LayoutWithAllValues.html"));
	}

	[Theory]
	[ClassData(typeof(MustacheViewNoModelTheoryData))]
	public static async Task When_The_Template_Replaces_Some_Of_The_Values(MustacheViewNoModel entryPoint)
	{
		var views = new MustacheViews(new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/Layout"));

		Assert.Equal(
			await Helper.Results("Layout/LayoutWithSomeValues.html"),
			await entryPoint(views, "LayoutWithSomeValues.html"));
	}
}
