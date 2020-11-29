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
		public MustacheViews(IFileProvider viewFileProvider)
		{
			if (viewFileProvider == null)
			{
				throw new ArgumentNullException(nameof(viewFileProvider));
			}

			try
			{
				var templateFiles = FileLookup.LookupFiles(viewFileProvider);

				var programs = Compiler.Compile(templateFiles);

				(this.viewProgram, this.entryPoints) = Linker.Link(programs);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);

				throw;
			}
		}

		internal async Task Execute<T>(StreamWriter streamWriter, string templatePath, T viewModel)
		{
			var canonicalTemplatePath = MustacheViews.ResolvePath(templatePath);

			if (!this.entryPoints.ContainsKey(canonicalTemplatePath))
			{
				throw new TemplateNotFound(templatePath);
			}

			await this.viewProgram.Execute(streamWriter, this.entryPoints[canonicalTemplatePath], viewModel);
		}

		private static string ResolvePath(string path)
		{
			path = path.Replace('\\', '/');

			while (path.StartsWith('/'))
			{
				path = path.Substring(1, path.Length - 1);
			}

			return Path.GetFullPath(path)
				.Replace(MustacheViews.FullPathPrefix, string.Empty, StringComparison.Ordinal)
				.Replace('\\', '/');
		}
	}
}
