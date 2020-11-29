using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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

			var mustacheViews = responseStream.Request.Dependency<MustacheViews>();

			if (mustacheViews == null)
			{
				throw new DependencyNotFound(typeof(MustacheViews));
			}

			try
			{
				await MustacheViewEngine.RenderToStream(responseStream.Stream, mustacheViews, templatePath, viewModel);
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

			var mustacheViews = responseStream.Request.Dependency<MustacheViews>();

			if (mustacheViews == null)
			{
				throw new DependencyNotFound(typeof(MustacheViews));
			}

			try
			{
				await MustacheViewEngine.RenderToStream(
					responseStream.Stream,
					mustacheViews,
					templatePath,
					MustacheViewEngine.NoModel);
			}
			catch (Exception exception)
			{
				throw exception;
			}
		}

		/// <summary>Renders the view to a string.</summary>
		/// <param name="mustacheViews">The view template collection.</param>
		/// <param name="templatePath">The path to the view template file.</param>
		/// <returns>A representation of the view as a string.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="mustacheViews"/> or <paramref name="templatePath"/> is <see langword="null"/>.</exception>
		/// <exception cref="TemplateNotFound">The view template specified by <paramref name="templatePath"/>, or a view template referenced by it, was not found.</exception>
		[return: NotNull]
		public static async Task<string> MustacheView(
			[DisallowNull] MustacheViews mustacheViews,
			[DisallowNull] string templatePath)
		{
			if (mustacheViews == null)
			{
				throw new ArgumentNullException(nameof(mustacheViews));
			}

			if (templatePath == null)
			{
				throw new ArgumentNullException(nameof(templatePath));
			}

			string result;

			try
			{
				result = await MustacheViewEngine.RenderToString(
					mustacheViews,
					templatePath,
					MustacheViewEngine.NoModel);
			}
			catch (Exception exception)
			{
				throw exception;
			}

			return result;
		}

		/// <summary>Renders the view to a string.</summary>
		/// <typeparam name="T">The type of the view model.</typeparam>
		/// <param name="mustacheViews">The view template collection.</param>
		/// <param name="templatePath">The path to the view template file.</param>
		/// <param name="viewModel">The view model.</param>
		/// <returns>A representation of the view as a string.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="mustacheViews"/>, <paramref name="templatePath"/> or <paramref name="viewModel"/> is <see langword="null"/>.</exception>
		/// <exception cref="TemplateNotFound">The view template specified by <paramref name="templatePath"/>, or a view template referenced by it, was not found.</exception>
		[return: NotNull]
		public static async Task<string> MustacheView<T>(
			[DisallowNull] MustacheViews mustacheViews,
			[DisallowNull] string templatePath,
			[DisallowNull] T viewModel)
			where T : notnull
		{
			if (mustacheViews == null)
			{
				throw new ArgumentNullException(nameof(mustacheViews));
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
				result = await MustacheViewEngine.RenderToString(mustacheViews, templatePath, viewModel);
			}
			catch (Exception exception)
			{
				throw exception;
			}

			return result;
		}

		private static async Task RenderToStream<T>(
			Stream outputStream,
			MustacheViews mustacheViews,
			string templatePath,
			T viewModel)
		{
			await using (var streamWriter = new StreamWriter(outputStream))
			{
				await mustacheViews.Execute(streamWriter, templatePath, viewModel);
			}
		}

		private static async Task<string> RenderToString<T>(
			MustacheViews mustacheViews,
			string templatePath,
			T viewModel)
		{
			await using (var memoryStream = new MemoryStream())
			{
				await MustacheViewEngine.RenderToStream(memoryStream, mustacheViews, templatePath, viewModel);

				return Encoding.UTF8.GetString(memoryStream.ToArray());
			}
		}
	}
}
