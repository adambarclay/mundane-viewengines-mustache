namespace Mundane.ViewEngines.Mustache.Compilation
{
	internal sealed class LineCounter
	{
		private char lastCharacter;

		internal LineCounter(int startLine, int startColumn)
		{
			this.Line = startLine;
			this.Column = startColumn;
			this.lastCharacter = '\0';
		}

		internal int Column { get; private set; }

		internal int Line { get; private set; }

		internal void Advance(char character)
		{
			if (character == '\r')
			{
				++this.Line;
				this.Column = 0;
			}
			else if (character == '\n')
			{
				if (this.lastCharacter != '\r')
				{
					++this.Line;
					this.Column = 0;
				}
			}
			else
			{
				++this.Column;
			}

			this.lastCharacter = character;
		}
	}
}
