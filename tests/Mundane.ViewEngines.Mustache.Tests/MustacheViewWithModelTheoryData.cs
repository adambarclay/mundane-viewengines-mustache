using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests;

public delegate ValueTask<string> MustacheViewWithModel(MustacheViews views, string templatePath, object viewModel);

[ExcludeFromCodeCoverage]
internal sealed class MustacheViewWithModelTheoryData : TheoryData<MustacheViewWithModel>
{
	public MustacheViewWithModelTheoryData()
	{
		this.Add(
			(views, templatePath, viewModel) => Helper.Run(
				new Dependencies(new Dependency<MustacheViews>(views)),
				o => o.MustacheView(templatePath, viewModel)));

		this.Add(
			(views, templatePath, viewModel) => Helper.Run(
				new Dependencies(),
				o => o.MustacheView(views, templatePath, viewModel)));

		this.Add(
			(views, templatePath, viewModel) => Helper.Run(
				stream => MustacheViewEngine.MustacheView(stream, views, templatePath, viewModel)));

		this.Add(MustacheViewEngine.MustacheView);
	}
}
