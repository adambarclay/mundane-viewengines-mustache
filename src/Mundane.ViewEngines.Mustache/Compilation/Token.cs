using System.Diagnostics;

namespace Mundane.ViewEngines.Mustache.Compilation
{
	[DebuggerDisplay("{" + nameof(Token.TokenType) + "}")]
	internal sealed class Token
	{
		internal Token(TokenType tokenType, int line, int character)
		{
			this.TokenType = tokenType;
			this.Line = line;
			this.Character = character;
		}

		internal int Character { get; }

		internal int Line { get; }

		internal TokenType TokenType { get; }
	}
}
