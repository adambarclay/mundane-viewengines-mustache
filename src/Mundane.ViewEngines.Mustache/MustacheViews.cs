using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Mundane.ViewEngines.Mustache.Compilation;
using Mundane.ViewEngines.Mustache.Engine;

namespace Mundane.ViewEngines.Mustache
{
	/// <summary>A collection of view templates.</summary>
	public sealed class MustacheViews
	{
		private static readonly string FullPathPrefix = Environment.CurrentDirectory + Path.DirectorySeparatorChar;

		private readonly ReadOnlyDictionary<string, int> entryPoints;
		private readonly ViewProgram viewProgram;

		/// <summary>Initializes a new instance of the <see cref="MustacheViews"/> class.</summary>
		/// <param name="viewFileProvider">The view template file provider.</param>
		/// <exception cref="ArgumentNullException"><paramref name="viewFileProvider"/> is <see langword="null"/>.</exception>
		/// <exception cref="MustacheCompilerError">An error is discovered during view compilation.</exception>
		public MustacheViews(IFileProvider viewFileProvider)
		{
			if (viewFileProvider is null)
			{
				throw new ArgumentNullException(nameof(viewFileProvider));
			}

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

		internal async Task Execute<T>(Stream outputStream, string templatePath, T viewModel)
			where T : notnull
		{
			if (this.entryPoints.TryGetValue(MustacheViews.ResolvePath(templatePath), out var entryPoint))
			{
				await this.viewProgram.Execute(outputStream, entryPoint, viewModel);
			}
			else
			{
				throw new TemplateNotFound(templatePath);
			}
		}

		private static string ResolvePath(string path)
		{
			path = path.Replace('\\', '/');

			while (path.StartsWith('/'))
			{
				path = path[1..]!;
			}

			return Path.GetFullPath(path)
				.Replace(MustacheViews.FullPathPrefix, string.Empty, StringComparison.Ordinal)
				.Replace('\\', '/');
		}
	}
}
