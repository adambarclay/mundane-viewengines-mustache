using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_MustacheViewEngine
{
	[ExcludeFromCodeCoverage]
	public static class MustacheView_Does_Not_Write_Contents_Of_Inverted_Block
	{
		[Theory]
		[ClassData(typeof(MustacheViewWithModelTheoryData))]
		public static async Task When_The_Inverted_Block_Condition_Is_Non_Empty_String(MustacheViewWithModel entryPoint)
		{
			var views = new MustacheViews(
				new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/Block"));

			var viewModel = new
			{
				Title = "Simple Block",
				BlockCondition = Guid.NewGuid().ToString(),
				Value = "Block Contents"
			};

			Assert.Equal(
				await Helper.Results("Block/NoBlock.html"),
				await entryPoint(views, "Inverted/Inverted.html", viewModel));
		}

		[Theory]
		[ClassData(typeof(MustacheViewWithModelTheoryData))]
		public static async Task When_The_Inverted_Block_Condition_Is_Non_Zero(MustacheViewWithModel entryPoint)
		{
			var views = new MustacheViews(
				new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/Block"));

			var zeros = new object[] { (sbyte)1, (byte)1, (short)1, (ushort)1, 1, 1U, 1L, 1UL, 1F, 1D, 1M };

			foreach (var zero in zeros)
			{
				var viewModel = new
				{
					Title = "Simple Block",
					BlockCondition = zero,
					Value = "Block Contents"
				};

				Assert.Equal(
					await Helper.Results("Block/NoBlock.html"),
					await entryPoint(views, "Inverted/Inverted.html", viewModel));
			}
		}

		[Theory]
		[ClassData(typeof(MustacheViewWithModelTheoryData))]
		public static async Task When_The_Inverted_Block_Condition_Is_Not_Null(MustacheViewWithModel entryPoint)
		{
			var views = new MustacheViews(
				new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/Block"));

			var viewModel = new
			{
				Title = "Simple Block",
				BlockCondition = new object(),
				Value = "Block Contents"
			};

			Assert.Equal(
				await Helper.Results("Block/NoBlock.html"),
				await entryPoint(views, "Inverted/Inverted.html", viewModel));
		}

		[Theory]
		[ClassData(typeof(MustacheViewWithModelTheoryData))]
		public static async Task When_The_Inverted_Block_Condition_Is_True(MustacheViewWithModel entryPoint)
		{
			var views = new MustacheViews(
				new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates/Block"));

			var viewModel = new
			{
				Title = "Simple Block",
				BlockCondition = true,
				Value = "Block Contents"
			};

			Assert.Equal(
				await Helper.Results("Block/NoBlock.html"),
				await entryPoint(views, "Inverted/Inverted.html", viewModel));
		}
	}
}
