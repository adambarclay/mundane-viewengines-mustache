using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_MustacheViewEngine;

[ExcludeFromCodeCoverage]
public static class MustacheView_Writes_Template_With_Url_Substitutions
{
	private static readonly Dictionary<string, string> UrlTransforms = new Dictionary<string, string>
	{
		{ "/some-url", "/transformed-url" }
	};

	private static readonly UrlResolver CustomUrlResolver = (pathBase, url) =>
		pathBase + MustacheView_Writes_Template_With_Url_Substitutions.UrlTransforms[url];

	[Theory]
	[ClassData(typeof(MustacheViewNoModelTheoryData))]
	public static async Task When_Using_A_Custom_Url_Resolver_And_Empty_Path_Base(MustacheViewNoModel entryPoint)
	{
		var views = new MustacheViews(
			new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/Url"),
			MustacheView_Writes_Template_With_Url_Substitutions.CustomUrlResolver);

		Assert.Equal(await Helper.Results("Url/UrlTransformedNoBase.html"), await entryPoint(views, "Url.html"));
	}

	[Theory]
	[ClassData(typeof(MustacheViewRequestTheoryData))]
	public static async Task When_Using_A_Custom_Url_Resolver_And_Non_Empty_Path_Base(MustacheViewRequest entryPoint)
	{
		var views = new MustacheViews(
			new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/Url"),
			MustacheView_Writes_Template_With_Url_Substitutions.CustomUrlResolver);

		Assert.Equal(await Helper.Results("Url/UrlTransformed.html"), await entryPoint(views, "Url.html"));
	}

	[Theory]
	[ClassData(typeof(MustacheViewNoModelTheoryData))]
	public static async Task When_Using_The_Default_Url_Resolver_And_Empty_Path_Base(MustacheViewNoModel entryPoint)
	{
		var views = new MustacheViews(new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/Url"));

		Assert.Equal(await Helper.Results("Url/UrlNoBase.html"), await entryPoint(views, "Url.html"));
	}

	[Theory]
	[ClassData(typeof(MustacheViewRequestTheoryData))]
	public static async Task When_Using_The_Default_Url_Resolver_And_Non_Empty_Path_Base(MustacheViewRequest entryPoint)
	{
		var views = new MustacheViews(new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/Url"));

		Assert.Equal(await Helper.Results("Url/Url.html"), await entryPoint(views, "Url.html"));
	}
}
