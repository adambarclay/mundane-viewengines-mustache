using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests.Tests_MustacheViewEngine
{
	[ExcludeFromCodeCoverage]
	public static class MustacheView_Writes_Template_To_Stream_Unchanged
	{
		[Fact]
		public static async Task When_The_Template_Contains_No_Substitution_Parameters()
		{
			const string templatePath = "NoSubstitutions.html";

			Assert.Equal(
				await Helper.Results(templatePath),
				await Helper.Execute(Helper.Views, o => o.MustacheView(templatePath)));

			Assert.Equal(
				await Helper.Results(templatePath),
				await Helper.Execute(Helper.Views, o => o.MustacheView(templatePath, new object())));

			Assert.Equal(
				await Helper.Results(templatePath),
				await MustacheViewEngine.MustacheView(Helper.Views, templatePath));

			Assert.Equal(
				await Helper.Results(templatePath),
				await MustacheViewEngine.MustacheView(Helper.Views, templatePath, new object()));
		}
	}
}
