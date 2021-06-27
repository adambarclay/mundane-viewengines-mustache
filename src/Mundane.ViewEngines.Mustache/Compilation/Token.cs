using System.Diagnostics;

namespace Mundane.ViewEngines.Mustache.Compilation
{
	[DebuggerDisplay("{" + nameof(Token.TokenType) + "}")]
	internal sealed class Token
	{
		internal Token(TokenType tokenType, LineCounter lineCounter)
		{
			this.TokenType = tokenType;
			this.Line = lineCounter.Line;
			this.Character = lineCounter.Column;
		}

		internal int Character { get; }

		internal int Line { get; }

		internal TokenType TokenType { get; }
	}
}
