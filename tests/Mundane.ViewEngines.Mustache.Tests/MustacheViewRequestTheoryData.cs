using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;

namespace Mundane.ViewEngines.Mustache.Tests;

public delegate ValueTask<string> MustacheViewRequest(MustacheViews views, string templatePath);

[ExcludeFromCodeCoverage]
internal sealed class MustacheViewRequestTheoryData : TheoryData<MustacheViewRequest>
{
	public MustacheViewRequestTheoryData()
	{
		this.Add(
			(views, templatePath) => Helper.Run(
				new Dependencies(new Dependency<MustacheViews>(views)),
				o => o.MustacheView(templatePath),
				"/my-app"));

		this.Add(
			(views, templatePath) => Helper.Run(
				new Dependencies(),
				o => o.MustacheView(views, templatePath),
				"/my-app"));

		this.Add(
			(views, templatePath) => Helper.Run(
				new Dependencies(new Dependency<MustacheViews>(views)),
				o => o.MustacheView(templatePath, new object()),
				"/my-app"));

		this.Add(
			(views, templatePath) => Helper.Run(
				new Dependencies(),
				o => o.MustacheView(views, templatePath, new object()),
				"/my-app"));
	}
}
