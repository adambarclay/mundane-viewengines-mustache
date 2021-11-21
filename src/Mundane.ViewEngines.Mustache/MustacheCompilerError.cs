using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mundane.ViewEngines.Mustache.Compilation;

namespace Mundane.ViewEngines.Mustache;

/// <summary>The exception thrown when one or more mustache templates contain an error.</summary>
public sealed class MustacheCompilerError : Exception
{
	internal MustacheCompilerError(List<ParserError> parserErrors)
		: base(MustacheCompilerError.CreateMessage(parserErrors))
	{
	}

	private static string CreateMessage(List<ParserError> parserErrors)
	{
		var stringBuilder = new StringBuilder(
			parserErrors[0].ErrorMessage,
			parserErrors.Sum(o => o.ErrorMessage.Length));

		for (var i = 1; i < parserErrors.Count; i++)
		{
			stringBuilder.AppendLine();
			stringBuilder.Append(parserErrors[i].ErrorMessage);
		}

		return stringBuilder.ToString();
	}
}
