using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_MustacheViewEngine
{
	[ExcludeFromCodeCoverage]
	public static class MustacheView_Writes_Contents_Of_Inverted_Block_A_Single_Time
	{
		[Theory]
		[ClassData(typeof(MustacheViewWithModelTheoryData))]
		public static async Task When_The_Inverted_Block_Condition_Is_Empty_Enumerable(MustacheViewWithModel entryPoint)
		{
			var views = new MustacheViews(
				new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/Block"));

			var viewModel = new
			{
				Title = "Simple Block",
				BlockCondition = Array.Empty<object>(),
				Value = "Block Contents"
			};

			Assert.Equal(
				await Helper.Results("Block/Simple.html"),
				await entryPoint(views, "Inverted/Inverted.html", viewModel));
		}

		[Theory]
		[ClassData(typeof(MustacheViewWithModelTheoryData))]
		public static async Task When_The_Inverted_Block_Condition_Is_Empty_String(MustacheViewWithModel entryPoint)
		{
			var views = new MustacheViews(
				new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/Block"));

			var viewModel = new
			{
				Title = "Simple Block",
				BlockCondition = string.Empty,
				Value = "Block Contents"
			};

			Assert.Equal(
				await Helper.Results("Block/Simple.html"),
				await entryPoint(views, "Inverted/Inverted.html", viewModel));
		}

		[Theory]
		[ClassData(typeof(MustacheViewWithModelTheoryData))]
		public static async Task When_The_Inverted_Block_Condition_Is_False(MustacheViewWithModel entryPoint)
		{
			var views = new MustacheViews(
				new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/Block"));

			var viewModel = new
			{
				Title = "Simple Block",
				BlockCondition = false,
				Value = "Block Contents"
			};

			Assert.Equal(
				await Helper.Results("Block/Simple.html"),
				await entryPoint(views, "Inverted/Inverted.html", viewModel));
		}

		[Theory]
		[ClassData(typeof(MustacheViewWithModelTheoryData))]
		public static async Task When_The_Inverted_Block_Condition_Is_Null(MustacheViewWithModel entryPoint)
		{
			var views = new MustacheViews(
				new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/Block"));

			var viewModel = new
			{
				Title = "Simple Block",
				BlockCondition = (object?)null,
				Value = "Block Contents"
			};

			Assert.Equal(
				await Helper.Results("Block/Simple.html"),
				await entryPoint(views, "Inverted/Inverted.html", viewModel));
		}

		[Theory]
		[ClassData(typeof(MustacheViewWithModelTheoryData))]
		public static async Task When_The_Inverted_Block_Condition_Is_Zero(MustacheViewWithModel entryPoint)
		{
			var views = new MustacheViews(
				new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/Block"));

			var zeros = new object[] { (sbyte)0, (byte)0, (short)0, (ushort)0, 0, 0U, 0L, 0UL, 0F, 0D, 0M };

			foreach (var zero in zeros)
			{
				var viewModel = new
				{
					Title = "Simple Block",
					BlockCondition = zero,
					Value = "Block Contents"
				};

				Assert.Equal(
					await Helper.Results("Block/Simple.html"),
					await entryPoint(views, "Inverted/Inverted.html", viewModel));
			}
		}
	}
}
