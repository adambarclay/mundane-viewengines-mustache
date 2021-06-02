using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_MustacheViews
{
	[ExcludeFromCodeCoverage]
	public static class Constructor_Throws_MustacheCompilerError
	{
		[Fact]
		public static void When_A_Close_Block_Identifier_Does_Not_Match_The_Inverted_Block_Identifier()
		{
			var exception = Assert.ThrowsAny<MustacheCompilerError>(
				() => new MustacheViews(
					new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Errors/InvertedBlockIdentifiers")));

			Assert.Equal(
				"/InvertedBlockIdentifiers.html Ln 6 Ch 42: Block closing tag {{/WrongIdentifier}} does not correspond to opening tag {{BlockCondition}}.",
				exception.Message);
		}

		[Fact]
		public static void When_A_Close_Block_Identifier_Does_Not_Match_The_Open_Block_Identifier()
		{
			var exception = Assert.ThrowsAny<MustacheCompilerError>(
				() => new MustacheViews(
					new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Errors/BlockIdentifiers")));

			Assert.Equal(
				"/BlockIdentifiers.html Ln 6 Ch 42: Block closing tag {{/WrongIdentifier}} does not correspond to opening tag {{BlockCondition}}.",
				exception.Message);
		}

		[Fact]
		public static void When_A_Close_Block_Tag_Does_Not_Have_A_Corresponding_Open_Block_Tag()
		{
			var exception = Assert.ThrowsAny<MustacheCompilerError>(
				() => new MustacheViews(
					new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Errors/MissingOpeningBlock")));

			Assert.Equal("/MissingOpeningBlock.html Ln 6 Ch 40: Unexpected token \"{{/\".", exception.Message);
		}

		[Fact]
		public static void When_A_Nested_Close_Block_Identifier_Does_Not_Match_The_Most_Recent_Open_Block_Identifier()
		{
			var exception = Assert.ThrowsAny<MustacheCompilerError>(
				() => new MustacheViews(
					new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Errors/NestedBlockIdentifiers")));

			Assert.Equal(
				"/NestedBlockIdentifiers.html Ln 6 Ch 62: Block closing tag {{/BlockCondition}} does not correspond to opening tag {{WrongIdentifier}}.",
				exception.Message);
		}

		[Fact]
		public static void When_The_Template_Has_A_Missing_Close_Brace()
		{
			var exception = Assert.ThrowsAny<MustacheCompilerError>(
				() => new MustacheViews(
					new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Errors/MissingCloseBrace")));

			Assert.Equal("/MissingCloseBrace.html Ln 3 Ch 18: }} expected.", exception.Message);
		}

		[Fact]
		public static void When_The_Template_Has_A_Missing_Identifier()
		{
			var exception = Assert.ThrowsAny<MustacheCompilerError>(
				() => new MustacheViews(
					new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Errors/MissingIdentifier")));

			Assert.Equal("/MissingIdentifier.html Ln 3 Ch 13: identifier expected.", exception.Message);
		}

		[Fact]
		public static void When_The_Template_Has_Too_Many_Identifiers()
		{
			var exception = Assert.ThrowsAny<MustacheCompilerError>(
				() => new MustacheViews(
					new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Errors/TooManyIdentifiers")));

			Assert.Equal("/TooManyIdentifiers.html Ln 3 Ch 24: }} expected.", exception.Message);
		}

		[Fact]
		public static void With_Details_Of_All_Of_The_Files_Which_Errored()
		{
			var exception = Assert.ThrowsAny<MustacheCompilerError>(
				() => new MustacheViews(new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Errors")));

			Assert.Contains(
				"LineNumberReporting/Linux/Linux.dat Ln 3 Ch 18: }} expected.",
				exception.Message,
				StringComparison.Ordinal);

			Assert.Contains(
				"LineNumberReporting/Mac/Mac.dat Ln 3 Ch 18: }} expected.",
				exception.Message,
				StringComparison.Ordinal);

			Assert.Contains(
				"LineNumberReporting/Windows/Windows.dat Ln 3 Ch 18: }} expected.",
				exception.Message,
				StringComparison.Ordinal);

			Assert.Contains(
				"MissingCloseBrace/MissingCloseBrace.html Ln 3 Ch 18: }} expected.",
				exception.Message,
				StringComparison.Ordinal);

			Assert.Contains(
				"MissingIdentifier/MissingIdentifier.html Ln 3 Ch 13: identifier expected.",
				exception.Message,
				StringComparison.Ordinal);

			Assert.Contains(
				"TooManyIdentifiers/TooManyIdentifiers.html Ln 3 Ch 24: }} expected.",
				exception.Message,
				StringComparison.Ordinal);
		}
	}
}
