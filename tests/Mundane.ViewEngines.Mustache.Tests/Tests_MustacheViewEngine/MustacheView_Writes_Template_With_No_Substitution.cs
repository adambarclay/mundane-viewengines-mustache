using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_MustacheViewEngine;

[ExcludeFromCodeCoverage]
public static class MustacheView_Writes_Template_With_No_Substitution
{
	[Theory]
	[ClassData(typeof(MustacheViewWithModelTheoryData))]
	public static async Task When_Property_ToString_Returns_Null(MustacheViewWithModel entryPoint)
	{
		var views = new MustacheViews(
			new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/SimpleSubstitutions/Single"));

		var viewModel = new
		{
			Title = "Simple Substitutions",
			Nested = new { Value = new TestObject() }
		};

		Assert.Equal(
			await Helper.Results("SimpleSubstitutions/SingleNoOutput.html"),
			await entryPoint(views, "Single.html", viewModel));
	}

	[Theory]
	[ClassData(typeof(MustacheViewWithModelTheoryData))]
	public static async Task When_Property_Value_Is_Null(MustacheViewWithModel entryPoint)
	{
		var views = new MustacheViews(
			new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/SimpleSubstitutions/Single"));

		var viewModel = new
		{
			Title = "Simple Substitutions",
			Nested = new { Value = (object?)null }
		};

		Assert.Equal(
			await Helper.Results("SimpleSubstitutions/SingleNoOutput.html"),
			await entryPoint(views, "Single.html", viewModel));
	}

	private sealed class TestObject
	{
		public override string? ToString()
		{
			return null;
		}
	}
}
