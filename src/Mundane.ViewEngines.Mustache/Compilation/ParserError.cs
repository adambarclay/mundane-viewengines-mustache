namespace Mundane.ViewEngines.Mustache.Compilation
{
	internal readonly struct ParserError
	{
		internal readonly string ErrorMessage;

		internal ParserError(string filePath, TokenType tokenType, Token token)
		{
			var tokenString = tokenType == TokenType.CloseTag ? "}}" : "identifier";

			this.ErrorMessage = $"{filePath} Ln {token.Line} Ch {token.Character}: {tokenString} expected.";
		}
	}
}
