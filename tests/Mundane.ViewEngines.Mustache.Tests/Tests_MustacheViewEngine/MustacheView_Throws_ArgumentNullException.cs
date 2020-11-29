using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_MustacheViewEngine
{
	[ExcludeFromCodeCoverage]
	public static class MustacheView_Throws_ArgumentNullException
	{
		[Fact]
		public static async Task When_The_TemplatePath_Parameter_Is_Null()
		{
			var views = new MustacheViews(new NullFileProvider());

			var exceptionStreamNoViewModel = await Assert.ThrowsAnyAsync<ArgumentNullException>(
				async () => await Helper.Execute(views, o => o.MustacheView(null!)));

			Assert.Equal("templatePath", exceptionStreamNoViewModel.ParamName);

			var exceptionStreamViewModel = await Assert.ThrowsAnyAsync<ArgumentNullException>(
				async () => await Helper.Execute(views, o => o.MustacheView(null!, new object())));

			Assert.Equal("templatePath", exceptionStreamViewModel.ParamName);

			var exceptionStringNoViewModel = await Assert.ThrowsAnyAsync<ArgumentNullException>(
				async () => await MustacheViewEngine.MustacheView(views, null!));

			Assert.Equal("templatePath", exceptionStringNoViewModel.ParamName);

			var exceptionStringViewModel = await Assert.ThrowsAnyAsync<ArgumentNullException>(
				async () => await MustacheViewEngine.MustacheView(views, null!, new object()));

			Assert.Equal("templatePath", exceptionStringViewModel.ParamName);
		}

		[Fact]
		public static async Task When_The_ViewModel_Parameter_Is_Null()
		{
			var views = new MustacheViews(new NullFileProvider());

			var exceptionStreamViewModel = await Assert.ThrowsAnyAsync<ArgumentNullException>(
				async () => await Helper.Execute(views, o => o.MustacheView("Index.html", (object)null!)));

			Assert.Equal("viewModel", exceptionStreamViewModel.ParamName);

			var exceptionStringViewModel = await Assert.ThrowsAnyAsync<ArgumentNullException>(
				async () => await MustacheViewEngine.MustacheView(views, "Index.html", (object)null!));

			Assert.Equal("viewModel", exceptionStringViewModel.ParamName);
		}

		[Fact]
		public static async Task When_The_Views_Parameter_Is_Null()
		{
			var exceptionStringNoViewModel = await Assert.ThrowsAnyAsync<ArgumentNullException>(
				async () => await MustacheViewEngine.MustacheView(null!, "Index.html"));

			Assert.Equal("mustacheViews", exceptionStringNoViewModel.ParamName);

			var exceptionStringViewModel = await Assert.ThrowsAnyAsync<ArgumentNullException>(
				async () => await MustacheViewEngine.MustacheView(null!, "Index.html", new object()));

			Assert.Equal("mustacheViews", exceptionStringViewModel.ParamName);
		}
	}
}
