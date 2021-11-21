using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_MustacheViewEngine;

[ExcludeFromCodeCoverage]
public static class MustacheView_Throws_ArgumentNullException
{
	[Fact]
	public static async Task When_The_MustacheViews_Parameter_Is_Null()
	{
		var dependencies = new Dependencies(new Dependency<MustacheViews>(new MustacheViews(new NullFileProvider())));

		var responseStreamNoModel = await Assert.ThrowsAnyAsync<ArgumentNullException>(
			async () => await Helper.Run(dependencies, o => o.MustacheView(null!, "Index.html")));

		Assert.Equal("mustacheViews", responseStreamNoModel.ParamName!);

		var responseStreamModel = await Assert.ThrowsAnyAsync<ArgumentNullException>(
			async () => await Helper.Run(dependencies, o => o.MustacheView(null!, "Index.html", new object())));

		Assert.Equal("mustacheViews", responseStreamModel.ParamName!);

		var streamNoModel = await Assert.ThrowsAnyAsync<ArgumentNullException>(
			async () => await Helper.Run(stream => MustacheViewEngine.MustacheView(stream, null!, "Index.html")));

		Assert.Equal("mustacheViews", streamNoModel.ParamName!);

		var streamModel = await Assert.ThrowsAnyAsync<ArgumentNullException>(
			async () => await Helper.Run(
				stream => MustacheViewEngine.MustacheView(stream, null!, "Index.html", new object())));

		Assert.Equal("mustacheViews", streamModel.ParamName!);

		var stringNoModel = await Assert.ThrowsAnyAsync<ArgumentNullException>(
			async () => await MustacheViewEngine.MustacheView(null!, "Index.html"));

		Assert.Equal("mustacheViews", stringNoModel.ParamName!);

		var stringModel = await Assert.ThrowsAnyAsync<ArgumentNullException>(
			async () => await MustacheViewEngine.MustacheView(null!, "Index.html", new object()));

		Assert.Equal("mustacheViews", stringModel.ParamName!);
	}

	[Fact]
	public static async Task When_The_OutputStream_Parameter_Is_Null()
	{
		var views = new MustacheViews(new NullFileProvider());

		var streamNoModel = await Assert.ThrowsAnyAsync<ArgumentNullException>(
			async () => await Helper.Run(_ => MustacheViewEngine.MustacheView(null!, views, "Index.html")));

		Assert.Equal("outputStream", streamNoModel.ParamName!);

		var streamModel = await Assert.ThrowsAnyAsync<ArgumentNullException>(
			async () => await Helper.Run(
				_ => MustacheViewEngine.MustacheView(null!, views, "Index.html", new object())));

		Assert.Equal("outputStream", streamModel.ParamName!);
	}

	[Theory]
	[ClassData(typeof(MustacheViewNoModelTheoryData))]
	public static async Task When_The_TemplatePath_Parameter_Is_Null(MustacheViewNoModel entryPoint)
	{
		var views = new MustacheViews(new NullFileProvider());

		var exception = await Assert.ThrowsAnyAsync<ArgumentNullException>(async () => await entryPoint(views, null!));

		Assert.Equal("templatePath", exception.ParamName!);
	}

	[Theory]
	[ClassData(typeof(MustacheViewWithModelTheoryData))]
	public static async Task When_The_ViewModel_Parameter_Is_Null(MustacheViewWithModel entryPoint)
	{
		var views = new MustacheViews(new NullFileProvider());

		var exception =
			await Assert.ThrowsAnyAsync<ArgumentNullException>(
				async () => await entryPoint(views, "Index.html", null!));

		Assert.Equal("viewModel", exception.ParamName!);
	}
}
