using System;
using System.Diagnostics;

namespace Mundane.ViewEngines.Mustache;

/// <summary>The exception thrown when a view template is not found.</summary>
public sealed class TemplateNotFound : Exception
{
	internal TemplateNotFound(string templateFileName)
		: base(TemplateNotFound.CreateMessage(templateFileName))
	{
	}

	private static string CreateMessage(string templateFileName)
	{
		Debug.Assert(!templateFileName.AsSpan().Trim().IsEmpty);

		return "Template \"" + templateFileName + "\" was not found.";
	}
}
