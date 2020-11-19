using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;

namespace Mundane.ViewEngines.Mustache
{
	/// <summary>The Mustache view engine.</summary>
	public static class MustacheViewEngine
	{
		private static readonly object NoModel = new object();

		/// <summary>Renders the view to the response stream.</summary>
		/// <typeparam name="T">The type of the view model.</typeparam>
		/// <param name="responseStream">The response stream.</param>
		/// <param name="templatePath">The path to the view template file.</param>
		/// <param name="viewModel">The view model.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="templatePath"/> or <paramref name="viewModel"/>  is <see langword="null"/>.</exception>
		/// <exception cref="TemplateNotFound">The view template specified by <paramref name="templatePath"/>, or a view template referenced by it, was not found.</exception>
		[return: NotNull]
		public static async Task MustacheView<T>(
			this ResponseStream responseStream,
			[DisallowNull] string templatePath,
			[DisallowNull] T viewModel)
			where T : notnull
		{
			if (templatePath == null)
			{
				throw new ArgumentNullException(nameof(templatePath));
			}

			if (viewModel == null)
			{
				throw new ArgumentNullException(nameof(viewModel));
			}

			var views = responseStream.Request.Dependency<Views>();

			try
			{
				await MustacheViewEngine.RenderToStream(
					responseStream.Stream,
					views.ViewFileProvider,
					templatePath,
					viewModel);
			}
			catch (Exception exception)
			{
				throw exception;
			}
		}

		/// <summary>Renders the view to the response stream.</summary>
		/// <param name="responseStream">The response stream.</param>
		/// <param name="templatePath">The path to the view template file.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="templatePath"/> is <see langword="null"/>.</exception>
		/// <exception cref="TemplateNotFound">The view template specified by <paramref name="templatePath"/>, or a view template referenced by it, was not found.</exception>
		[return: NotNull]
		public static async Task MustacheView(this ResponseStream responseStream, [DisallowNull] string templatePath)
		{
			if (templatePath == null)
			{
				throw new ArgumentNullException(nameof(templatePath));
			}

			var views = responseStream.Request.Dependency<Views>();

			try
			{
				await MustacheViewEngine.RenderToStream(
					responseStream.Stream,
					views.ViewFileProvider,
					templatePath,
					MustacheViewEngine.NoModel);
			}
			catch (Exception exception)
			{
				throw exception;
			}
		}

		/// <summary>Renders the view to a string.</summary>
		/// <param name="views">The view template collection.</param>
		/// <param name="templatePath">The path to the view template file.</param>
		/// <returns>A representation of the view as a string.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="views"/> or <paramref name="templatePath"/> is <see langword="null"/>.</exception>
		/// <exception cref="TemplateNotFound">The view template specified by <paramref name="templatePath"/>, or a view template referenced by it, was not found.</exception>
		[return: NotNull]
		public static async Task<string> MustacheView([DisallowNull] Views views, [DisallowNull] string templatePath)
		{
			if (views == null)
			{
				throw new ArgumentNullException(nameof(views));
			}

			if (templatePath == null)
			{
				throw new ArgumentNullException(nameof(templatePath));
			}

			string result;

			try
			{
				await using (var stream = new MemoryStream())
				{
					await MustacheViewEngine.RenderToStream(
						stream,
						views.ViewFileProvider,
						templatePath,
						MustacheViewEngine.NoModel);

					result = Encoding.UTF8.GetString(stream.ToArray());
				}
			}
			catch (Exception exception)
			{
				throw exception;
			}

			return result;
		}

		/// <summary>Renders the view to a string.</summary>
		/// <typeparam name="T">The type of the view model.</typeparam>
		/// <param name="views">The view template collection.</param>
		/// <param name="templatePath">The path to the view template file.</param>
		/// <param name="viewModel">The view model.</param>
		/// <returns>A representation of the view as a string.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="views"/>, <paramref name="templatePath"/> or <paramref name="viewModel"/> is <see langword="null"/>.</exception>
		/// <exception cref="TemplateNotFound">The view template specified by <paramref name="templatePath"/>, or a view template referenced by it, was not found.</exception>
		[return: NotNull]
		public static async Task<string> MustacheView<T>(
			[DisallowNull] Views views,
			[DisallowNull] string templatePath,
			[DisallowNull] T viewModel)
			where T : notnull
		{
			if (views == null)
			{
				throw new ArgumentNullException(nameof(views));
			}

			if (templatePath == null)
			{
				throw new ArgumentNullException(nameof(templatePath));
			}

			if (viewModel == null)
			{
				throw new ArgumentNullException(nameof(viewModel));
			}

			string result;

			try
			{
				await using (var stream = new MemoryStream())
				{
					await MustacheViewEngine.RenderToStream(stream, views.ViewFileProvider, templatePath, viewModel);

					result = Encoding.UTF8.GetString(stream.ToArray());
				}
			}
			catch (Exception exception)
			{
				throw exception;
			}

			return result;
		}

		private static async Task RenderToStream<T>(
			Stream outputStream,
			IFileProvider fileProvider,
			string templatePath,
			T viewModel)
		{
			var fileInfo = fileProvider.GetFileInfo(templatePath);

			if (!fileInfo.Exists)
			{
				throw new TemplateNotFound(fileInfo.Name);
			}

			await using (var templateStream = fileInfo.CreateReadStream())
			{
				await templateStream.CopyToAsync(outputStream);
			}
		}
	}
}
