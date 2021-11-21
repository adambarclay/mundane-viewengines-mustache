using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests;

public delegate ValueTask<string> MustacheViewNoModel(MustacheViews views, string templatePath);

[ExcludeFromCodeCoverage]
internal sealed class MustacheViewNoModelTheoryData : TheoryData<MustacheViewNoModel>
{
	public MustacheViewNoModelTheoryData()
	{
		this.Add(
			(views, templatePath) => Helper.Run(
				new Dependencies(new Dependency<MustacheViews>(views)),
				o => o.MustacheView(templatePath)));

		this.Add((views, templatePath) => Helper.Run(new Dependencies(), o => o.MustacheView(views, templatePath)));

		this.Add(
			(views, templatePath) => Helper.Run(
				stream => MustacheViewEngine.MustacheView(stream, views, templatePath)));

		this.Add(MustacheViewEngine.MustacheView);

		this.Add(
			(views, templatePath) => Helper.Run(
				new Dependencies(new Dependency<MustacheViews>(views)),
				o => o.MustacheView(templatePath, new object())));

		this.Add(
			(views, templatePath) => Helper.Run(
				new Dependencies(),
				o => o.MustacheView(views, templatePath, new object())));

		this.Add(
			(views, templatePath) => Helper.Run(
				stream => MustacheViewEngine.MustacheView(stream, views, templatePath, new object())));

		this.Add((views, templatePath) => MustacheViewEngine.MustacheView(views, templatePath, new object()));
	}
}
