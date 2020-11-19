using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_MustacheViewEngine
{
	[ExcludeFromCodeCoverage]
	public static class MustacheView_Throws_ArgumentNullException
	{
		[Fact]
		public static async Task When_The_TemplatePath_Parameter_Is_Null()
		{
			var exceptionStreamNoViewModel = await Assert.ThrowsAnyAsync<ArgumentNullException>(
				async () => await Helper.Execute(Helper.Views, o => o.MustacheView(null!)));

			Assert.Equal("templatePath", exceptionStreamNoViewModel.ParamName);

			var exceptionStreamViewModel = await Assert.ThrowsAnyAsync<ArgumentNullException>(
				async () => await Helper.Execute(Helper.Views, o => o.MustacheView(null!, new object())));

			Assert.Equal("templatePath", exceptionStreamViewModel.ParamName);

			var exceptionStringNoViewModel = await Assert.ThrowsAnyAsync<ArgumentNullException>(
				async () => await MustacheViewEngine.MustacheView(Helper.Views, null!));

			Assert.Equal("templatePath", exceptionStringNoViewModel.ParamName);

			var exceptionStringViewModel = await Assert.ThrowsAnyAsync<ArgumentNullException>(
				async () => await MustacheViewEngine.MustacheView(Helper.Views, null!, new object()));

			Assert.Equal("templatePath", exceptionStringViewModel.ParamName);
		}

		[Fact]
		public static async Task When_The_ViewModel_Parameter_Is_Null()
		{
			var exceptionStreamViewModel = await Assert.ThrowsAnyAsync<ArgumentNullException>(
				async () => await Helper.Execute(Helper.Views, o => o.MustacheView("Index.html", (object)null!)));

			Assert.Equal("viewModel", exceptionStreamViewModel.ParamName);

			var exceptionStringViewModel = await Assert.ThrowsAnyAsync<ArgumentNullException>(
				async () => await MustacheViewEngine.MustacheView(Helper.Views, "Index.html", (object)null!));

			Assert.Equal("viewModel", exceptionStringViewModel.ParamName);
		}

		[Fact]
		public static async Task When_The_Views_Parameter_Is_Null()
		{
			var exceptionStringNoViewModel = await Assert.ThrowsAnyAsync<ArgumentNullException>(
				async () => await MustacheViewEngine.MustacheView(null!, "Index.html"));

			Assert.Equal("views", exceptionStringNoViewModel.ParamName);

			var exceptionStringViewModel = await Assert.ThrowsAnyAsync<ArgumentNullException>(
				async () => await MustacheViewEngine.MustacheView(null!, "Index.html", new object()));

			Assert.Equal("views", exceptionStringViewModel.ParamName);
		}
	}
}
