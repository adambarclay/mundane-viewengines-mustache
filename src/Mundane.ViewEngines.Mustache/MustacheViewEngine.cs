using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Mundane.ViewEngines.Mustache;

/// <summary>The Mustache view engine.</summary>
public static class MustacheViewEngine
{
	internal const string TrimmingWarning = "";
	private const string TrimmingSupressJustification = "";

	private static readonly object NoModel = new object();

	/// <summary>Renders the view to the response stream using the <see cref="MustacheViews"/> request dependency.</summary>
	/// <param name="responseStream">The response stream.</param>
	/// <param name="templatePath">The path to the view template file.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="templatePath"/> is <see langword="null"/>.</exception>
	/// <exception cref="DependencyNotFound">The <see cref="MustacheViews"/> dependency has not been registered.</exception>
	/// <exception cref="TemplateNotFound">The view template specified by <paramref name="templatePath"/>, or a view template referenced by it, was not found.</exception>
	/// <exception cref="ViewModelPropertyNotFound">The view template contains a property name and a view model was not supplied.</exception>
	[UnconditionalSuppressMessage(
		"ReflectionAnalysis",
		"IL2026",
		Justification = MustacheViewEngine.TrimmingSupressJustification)]
	public static async ValueTask MustacheView(this ResponseStream responseStream, string templatePath)
	{
		if (templatePath is null)
		{
			throw new ArgumentNullException(nameof(templatePath));
		}

		var mustacheViews = responseStream.Request.Dependency<MustacheViews>();

		try
		{
			await mustacheViews.Execute(
				responseStream.Stream,
				responseStream.Request.PathBase,
				templatePath,
				MustacheViewEngine.NoModel);
		}
		catch (TemplateNotFound exception)
		{
			throw exception;
		}
		catch (ViewModelPropertyNotFound exception)
		{
			throw exception;
		}
	}

	/// <summary>Renders the view to the response stream using the <see cref="MustacheViews"/> request dependency.</summary>
	/// <param name="responseStream">The response stream.</param>
	/// <param name="templatePath">The path to the view template file.</param>
	/// <param name="viewModel">The view model.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="templatePath"/> or <paramref name="viewModel"/> is <see langword="null"/>.</exception>
	/// <exception cref="DependencyNotFound">The <see cref="MustacheViews"/> dependency has not been registered.</exception>
	/// <exception cref="TemplateNotFound">The view template specified by <paramref name="templatePath"/>, or a view template referenced by it, was not found.</exception>
	/// <exception cref="ViewModelPropertyNotFound">The view template contains a property name which is not present in the view model.</exception>
	[RequiresUnreferencedCode(MustacheViewEngine.TrimmingWarning)]
	public static async ValueTask MustacheView(
		this ResponseStream responseStream,
		string templatePath,
		object viewModel)
	{
		if (templatePath is null)
		{
			throw new ArgumentNullException(nameof(templatePath));
		}

		if (viewModel is null)
		{
			throw new ArgumentNullException(nameof(viewModel));
		}

		var mustacheViews = responseStream.Request.Dependency<MustacheViews>();

		try
		{
			await mustacheViews.Execute(
				responseStream.Stream,
				responseStream.Request.PathBase,
				templatePath,
				viewModel);
		}
		catch (TemplateNotFound exception)
		{
			throw exception;
		}
		catch (ViewModelPropertyNotFound exception)
		{
			throw exception;
		}
	}

	/// <summary>Renders the view to the response stream.</summary>
	/// <param name="responseStream">The response stream.</param>
	/// <param name="mustacheViews">The view template collection.</param>
	/// <param name="templatePath">The path to the view template file.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="mustacheViews"/>, <paramref name="templatePath"/> is <see langword="null"/>.</exception>
	/// <exception cref="TemplateNotFound">The view template specified by <paramref name="templatePath"/>, or a view template referenced by it, was not found.</exception>
	/// <exception cref="ViewModelPropertyNotFound">The view template contains a property name and a view model was not supplied.</exception>
	[UnconditionalSuppressMessage(
		"ReflectionAnalysis",
		"IL2026",
		Justification = MustacheViewEngine.TrimmingSupressJustification)]
	public static async ValueTask MustacheView(
		this ResponseStream responseStream,
		MustacheViews mustacheViews,
		string templatePath)
	{
		if (mustacheViews is null)
		{
			throw new ArgumentNullException(nameof(mustacheViews));
		}

		if (templatePath is null)
		{
			throw new ArgumentNullException(nameof(templatePath));
		}

		try
		{
			await mustacheViews.Execute(
				responseStream.Stream,
				responseStream.Request.PathBase,
				templatePath,
				MustacheViewEngine.NoModel);
		}
		catch (TemplateNotFound exception)
		{
			throw exception;
		}
		catch (ViewModelPropertyNotFound exception)
		{
			throw exception;
		}
	}

	/// <summary>Renders the view to the response stream.</summary>
	/// <param name="responseStream">The response stream.</param>
	/// <param name="mustacheViews">The view template collection.</param>
	/// <param name="templatePath">The path to the view template file.</param>
	/// <param name="viewModel">The view model.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="mustacheViews"/>, <paramref name="templatePath"/> or <paramref name="viewModel"/> is <see langword="null"/>.</exception>
	/// <exception cref="TemplateNotFound">The view template specified by <paramref name="templatePath"/>, or a view template referenced by it, was not found.</exception>
	/// <exception cref="ViewModelPropertyNotFound">The view template contains a property name which is not present in the view model.</exception>
	[RequiresUnreferencedCode(MustacheViewEngine.TrimmingWarning)]
	public static async ValueTask MustacheView(
		this ResponseStream responseStream,
		MustacheViews mustacheViews,
		string templatePath,
		object viewModel)
	{
		if (mustacheViews is null)
		{
			throw new ArgumentNullException(nameof(mustacheViews));
		}

		if (templatePath is null)
		{
			throw new ArgumentNullException(nameof(templatePath));
		}

		if (viewModel is null)
		{
			throw new ArgumentNullException(nameof(viewModel));
		}

		try
		{
			await mustacheViews.Execute(
				responseStream.Stream,
				responseStream.Request.PathBase,
				templatePath,
				viewModel);
		}
		catch (TemplateNotFound exception)
		{
			throw exception;
		}
		catch (ViewModelPropertyNotFound exception)
		{
			throw exception;
		}
	}

	/// <summary>Renders the view to a stream.</summary>
	/// <param name="outputStream">The output stream.</param>
	/// <param name="mustacheViews">The view template collection.</param>
	/// <param name="templatePath">The path to the view template file.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="outputStream"/>, <paramref name="mustacheViews"/> or <paramref name="templatePath"/> is <see langword="null"/>.</exception>
	/// <exception cref="TemplateNotFound">The view template specified by <paramref name="templatePath"/>, or a view template referenced by it, was not found.</exception>
	/// <exception cref="ViewModelPropertyNotFound">The view template contains a property name and a view model was not supplied.</exception>
	[UnconditionalSuppressMessage(
		"ReflectionAnalysis",
		"IL2026",
		Justification = MustacheViewEngine.TrimmingSupressJustification)]
	public static async ValueTask MustacheView(Stream outputStream, MustacheViews mustacheViews, string templatePath)
	{
		if (outputStream is null)
		{
			throw new ArgumentNullException(nameof(outputStream));
		}

		if (mustacheViews is null)
		{
			throw new ArgumentNullException(nameof(mustacheViews));
		}

		if (templatePath is null)
		{
			throw new ArgumentNullException(nameof(templatePath));
		}

		try
		{
			await mustacheViews.Execute(outputStream, string.Empty, templatePath, MustacheViewEngine.NoModel);
		}
		catch (TemplateNotFound exception)
		{
			throw exception;
		}
		catch (ViewModelPropertyNotFound exception)
		{
			throw exception;
		}
	}

	/// <summary>Renders the view to a stream.</summary>
	/// <param name="outputStream">The output stream.</param>
	/// <param name="mustacheViews">The view template collection.</param>
	/// <param name="templatePath">The path to the view template file.</param>
	/// <param name="viewModel">The view model.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="outputStream"/>, <paramref name="mustacheViews"/>, <paramref name="templatePath"/> or <paramref name="viewModel"/> is <see langword="null"/>.</exception>
	/// <exception cref="TemplateNotFound">The view template specified by <paramref name="templatePath"/>, or a view template referenced by it, was not found.</exception>
	/// <exception cref="ViewModelPropertyNotFound">The view template contains a property name which is not present in the view model.</exception>
	[RequiresUnreferencedCode(MustacheViewEngine.TrimmingWarning)]
	public static async ValueTask MustacheView(
		Stream outputStream,
		MustacheViews mustacheViews,
		string templatePath,
		object viewModel)
	{
		if (outputStream is null)
		{
			throw new ArgumentNullException(nameof(outputStream));
		}

		if (mustacheViews is null)
		{
			throw new ArgumentNullException(nameof(mustacheViews));
		}

		if (templatePath is null)
		{
			throw new ArgumentNullException(nameof(templatePath));
		}

		if (viewModel is null)
		{
			throw new ArgumentNullException(nameof(viewModel));
		}

		try
		{
			await mustacheViews.Execute(outputStream, string.Empty, templatePath, viewModel);
		}
		catch (TemplateNotFound exception)
		{
			throw exception;
		}
		catch (ViewModelPropertyNotFound exception)
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
	/// <exception cref="ViewModelPropertyNotFound">The view template contains a property name and a view model was not supplied.</exception>
	[UnconditionalSuppressMessage(
		"ReflectionAnalysis",
		"IL2026",
		Justification = MustacheViewEngine.TrimmingSupressJustification)]
	public static async ValueTask<string> MustacheView(MustacheViews mustacheViews, string templatePath)
	{
		if (mustacheViews is null)
		{
			throw new ArgumentNullException(nameof(mustacheViews));
		}

		if (templatePath is null)
		{
			throw new ArgumentNullException(nameof(templatePath));
		}

		string result;

		try
		{
			result = await MustacheViewEngine.RenderToString(
				mustacheViews,
				string.Empty,
				templatePath,
				MustacheViewEngine.NoModel);
		}
		catch (TemplateNotFound exception)
		{
			throw exception;
		}
		catch (ViewModelPropertyNotFound exception)
		{
			throw exception;
		}

		return result;
	}

	/// <summary>Renders the view to a string.</summary>
	/// <param name="mustacheViews">The view template collection.</param>
	/// <param name="templatePath">The path to the view template file.</param>
	/// <param name="viewModel">The view model.</param>
	/// <returns>A representation of the view as a string.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="mustacheViews"/>, <paramref name="templatePath"/> or <paramref name="viewModel"/> is <see langword="null"/>.</exception>
	/// <exception cref="TemplateNotFound">The view template specified by <paramref name="templatePath"/>, or a view template referenced by it, was not found.</exception>
	/// <exception cref="ViewModelPropertyNotFound">The view template contains a property name which is not present in the view model.</exception>
	[RequiresUnreferencedCode(MustacheViewEngine.TrimmingWarning)]
	public static async ValueTask<string> MustacheView(
		MustacheViews mustacheViews,
		string templatePath,
		object viewModel)
	{
		if (mustacheViews is null)
		{
			throw new ArgumentNullException(nameof(mustacheViews));
		}

		if (templatePath is null)
		{
			throw new ArgumentNullException(nameof(templatePath));
		}

		if (viewModel is null)
		{
			throw new ArgumentNullException(nameof(viewModel));
		}

		string result;

		try
		{
			result = await MustacheViewEngine.RenderToString(mustacheViews, string.Empty, templatePath, viewModel);
		}
		catch (TemplateNotFound exception)
		{
			throw exception;
		}
		catch (ViewModelPropertyNotFound exception)
		{
			throw exception;
		}

		return result;
	}

	[RequiresUnreferencedCode(MustacheViewEngine.TrimmingWarning)]
	private static async ValueTask<string> RenderToString(
		MustacheViews mustacheViews,
		string pathBase,
		string templatePath,
		object viewModel)
	{
		await using (var memoryStream = new MemoryStream())
		{
			await mustacheViews.Execute(memoryStream, pathBase, templatePath, viewModel);

			return Encoding.UTF8.GetString(memoryStream.ToArray());
		}
	}
}
