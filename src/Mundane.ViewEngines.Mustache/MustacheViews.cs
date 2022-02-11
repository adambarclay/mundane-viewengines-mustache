using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Mundane.ViewEngines.Mustache.Compilation;
using Mundane.ViewEngines.Mustache.Engine;

namespace Mundane.ViewEngines.Mustache;

/// <summary>A collection of view templates.</summary>
public sealed class MustacheViews
{
	private readonly ReadOnlyDictionary<string, int> entryPoints;
	private readonly UrlResolver urlResolver;
	private readonly ViewProgram viewProgram;

	/// <summary>Initializes a new instance of the <see cref="MustacheViews"/> class.</summary>
	/// <param name="viewFileProvider">The view template file provider.</param>
	/// <exception cref="ArgumentNullException"><paramref name="viewFileProvider"/> is <see langword="null"/>.</exception>
	/// <exception cref="MustacheCompilerError">An error is discovered during view compilation.</exception>
	public MustacheViews(IFileProvider viewFileProvider)
		: this(viewFileProvider, (pathBase, url) => pathBase + url)
	{
	}

	/// <summary>Initializes a new instance of the <see cref="MustacheViews"/> class.</summary>
	/// <param name="viewFileProvider">The view template file provider.</param>
	/// <param name="urlResolver">Resolves URLs when specified in URL tags e.g. {{~ /some-url }}.</param>
	/// <exception cref="ArgumentNullException"><paramref name="viewFileProvider"/> is <see langword="null"/>.</exception>
	/// <exception cref="MustacheCompilerError">An error is discovered during view compilation.</exception>
	public MustacheViews(IFileProvider viewFileProvider, UrlResolver urlResolver)
	{
		if (viewFileProvider is null)
		{
			throw new ArgumentNullException(nameof(viewFileProvider));
		}

		if (urlResolver is null)
		{
			throw new ArgumentNullException(nameof(urlResolver));
		}

		this.urlResolver = urlResolver;

		try
		{
			var templateFiles = FileLookup.LookupFiles(viewFileProvider);

			var programs = Compiler.Compile(templateFiles);

			(this.viewProgram, this.entryPoints) = Linker.Link(programs);
		}
		catch (MustacheCompilerError exception)
		{
			throw exception;
		}
	}

	[UnconditionalSuppressMessage(
		"Trimming",
		"IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
		Justification = "")]
	internal async ValueTask Execute(Stream outputStream, string pathBase, string templatePath, object viewModel)
	{
		if (this.entryPoints.TryGetValue(MustacheViews.NormalisePath(templatePath), out var entryPoint))
		{
			await this.viewProgram.Execute(outputStream, this.urlResolver, pathBase, entryPoint, viewModel);
		}
		else
		{
			throw new TemplateNotFound(templatePath);
		}
	}

	private static string NormalisePath(string path)
	{
		path = path.Replace('\\', '/');

		if (!path.StartsWith('/'))
		{
			path = "/" + path;
		}

		return path;
	}
}
