using System.Collections.Generic;
using Microsoft.Extensions.FileProviders;

namespace Mundane.ViewEngines.Mustache.Compilation;

internal static class Compiler
{
	internal static List<(string Path, CompiledProgram Program)> Compile(
		List<(string Path, IFileInfo File)> templateFiles)
	{
		var programs = new List<(string, CompiledProgram)>();

		var parserErrors = new List<ParserError>();

		foreach ((var path, var file) in templateFiles)
		{
			(var tokens, var literals) = LexicalAnalyser.Tokenise(file);

			(var invalid, var error) = Parser.Parse(path, tokens, literals);

			if (invalid)
			{
				parserErrors.Add(error);
			}

			if (parserErrors.Count == 0)
			{
				programs.Add((path, CodeGenerator.GenerateCode(tokens, literals)));
			}
		}

		if (parserErrors.Count > 0)
		{
			throw new MustacheCompilerError(parserErrors);
		}

		return programs;
	}
}
