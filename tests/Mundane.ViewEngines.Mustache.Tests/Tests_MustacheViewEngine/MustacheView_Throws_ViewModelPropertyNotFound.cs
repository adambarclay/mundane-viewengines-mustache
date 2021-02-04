using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_MustacheViewEngine
{
	[ExcludeFromCodeCoverage]
	public static class MustacheView_Throws_ViewModelPropertyNotFound
	{
		[Theory]
		[ClassData(typeof(MustacheViewWithModelTheoryData))]
		public static async Task When_The_View_Model_Dictionary_Does_Not_Contain_The_Template_Property(
			MustacheViewWithModel entryPoint)
		{
			var views = new MustacheViews(
				new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/SimpleSubstitutions/Single"));

			var viewModel = new Dictionary<string, string> { { "Title", "Simple Substitutions" } };

			var exception = await Assert.ThrowsAnyAsync<ViewModelPropertyNotFound>(
				async () => await entryPoint(views, "Single.html", viewModel));

			Assert.Equal("Property \"Nested.Value\" was not found.", exception.Message);
		}

		[Theory]
		[ClassData(typeof(MustacheViewWithModelTheoryData))]
		public static async Task When_The_View_Model_Object_Does_Not_Contain_The_Nested_Template_Property(
			MustacheViewWithModel entryPoint)
		{
			var views = new MustacheViews(
				new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/SimpleSubstitutions/Single"));

			var viewModel = new
			{
				Title = "Simple Substitutions",
				Nested = (object?)null
			};

			var exception = await Assert.ThrowsAnyAsync<ViewModelPropertyNotFound>(
				async () => await entryPoint(views, "Single.html", viewModel));

			Assert.Equal("Property \"Nested.Value\" was not found.", exception.Message);
		}

		[Theory]
		[ClassData(typeof(MustacheViewWithModelTheoryData))]
		public static async Task When_The_View_Model_Object_Does_Not_Contain_The_Template_Property(
			MustacheViewWithModel entryPoint)
		{
			var views = new MustacheViews(
				new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/SimpleSubstitutions/Single"));

			var viewModel = new { Nested = new { Value = "Simple Substitutions" } };

			var exception = await Assert.ThrowsAnyAsync<ViewModelPropertyNotFound>(
				async () => await entryPoint(views, "Single.html", viewModel));

			Assert.Equal("Property \"Title\" was not found.", exception.Message);
		}

		[Theory]
		[ClassData(typeof(MustacheViewNoModelTheoryData))]
		public static async Task When_The_View_Template_Contains_A_Property_And_The_View_Model_Is_Not_Supplied(
			MustacheViewNoModel entryPoint)
		{
			var views = new MustacheViews(
				new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/SimpleSubstitutions/Single"));

			var exception = await Assert.ThrowsAnyAsync<ViewModelPropertyNotFound>(
				async () => await entryPoint(views, "Single.html"));

			Assert.Equal("Property \"Title\" was not found.", exception.Message);
		}
	}
}
