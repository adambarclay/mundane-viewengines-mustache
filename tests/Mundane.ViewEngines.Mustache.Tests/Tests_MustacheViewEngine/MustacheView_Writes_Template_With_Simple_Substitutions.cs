using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_MustacheViewEngine;

[ExcludeFromCodeCoverage]
public static class MustacheView_Writes_Template_With_Simple_Substitutions
{
	[Theory]
	[ClassData(typeof(MustacheViewWithModelTheoryData))]
	public static async Task When_The_Identifier_Contains_A_Single_Close_Brace(MustacheViewWithModel entryPoint)
	{
		var views = new MustacheViews(
			new ManifestEmbeddedFileProvider(
				typeof(Helper).Assembly,
				"/Templates/SimpleSubstitutions/BraceCloseInIdentifier"));

		var viewModel = new Dictionary<string, object>
		{
			{ "Title", "Simple Substitutions" },
			{ "Nes}ed", new { Value = "Nested Value" } }
		};

		Assert.Equal(
			await Helper.Results("SimpleSubstitutions/SimpleSubstitutions.html"),
			await entryPoint(views, "BraceCloseInIdentifier.html", viewModel));
	}

	[Theory]
	[ClassData(typeof(MustacheViewWithModelTheoryData))]
	public static async Task When_The_Tags_Have_Different_Amounts_Of_Whitespace(MustacheViewWithModel entryPoint)
	{
		var views = new MustacheViews(
			new ManifestEmbeddedFileProvider(
				typeof(Helper).Assembly,
				"/Templates/SimpleSubstitutions/WhitespaceVariations"));

		var viewModel = new
		{
			Title = "Simple Substitutions",
			Nested = new { Value = "Nested Value" }
		};

		Assert.Equal(
			await Helper.Results("SimpleSubstitutions/SimpleSubstitutions.html"),
			await entryPoint(views, "WhitespaceVariations.html", viewModel));
	}

	[Theory]
	[ClassData(typeof(MustacheViewWithModelTheoryData))]
	public static async Task When_The_Text_Contains_A_Single_Open_Brace(MustacheViewWithModel entryPoint)
	{
		var views = new MustacheViews(
			new ManifestEmbeddedFileProvider(
				typeof(Helper).Assembly,
				"/Templates/SimpleSubstitutions/BraceOpenInText"));

		var viewModel = new
		{
			Title = "Simple Substitutions",
			Nested = new { Value = "Nested Value" }
		};

		Assert.Equal(
			await Helper.Results("SimpleSubstitutions/BraceOpenInText.html"),
			await entryPoint(views, "BraceOpenInText.html", viewModel));
	}

	[Theory]
	[ClassData(typeof(MustacheViewWithModelTheoryData))]
	public static async Task When_The_Value_Comes_From_A_Field(MustacheViewWithModel entryPoint)
	{
		var views = new MustacheViews(
			new ManifestEmbeddedFileProvider(
				typeof(Helper).Assembly,
				"/Templates/SimpleSubstitutions/WhitespaceVariations"));

		var viewModel = new
		{
			Title = "Simple Substitutions",
			Nested = new TestObject()
		};

		Assert.Equal(
			await Helper.Results("SimpleSubstitutions/SimpleSubstitutions.html"),
			await entryPoint(views, "WhitespaceVariations.html", viewModel));
	}

	private sealed class TestObject
	{
#pragma warning disable SA1401 // Fields should be private
#pragma warning disable 414
		internal readonly string Value = "Nested Value";
#pragma warning restore 414
#pragma warning restore SA1401 // Fields should be private
	}
}
