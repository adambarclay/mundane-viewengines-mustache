using System.Collections.Generic;
using Mundane.ViewEngines.Mustache.Engine;

namespace Mundane.ViewEngines.Mustache.Compilation
{
	internal readonly struct CompiledProgram
	{
		internal readonly List<string[]> Identifiers;
		internal readonly List<Instruction> Instructions;
		internal readonly List<string> Literals;
		internal readonly List<string> TemplatePaths;

		internal CompiledProgram(
			List<Instruction> instructions,
			List<string> literals,
			List<string[]> identifiers,
			List<string> templatePaths)
		{
			this.Instructions = instructions;
			this.Literals = literals;
			this.Identifiers = identifiers;
			this.TemplatePaths = templatePaths;
		}
	}
}
