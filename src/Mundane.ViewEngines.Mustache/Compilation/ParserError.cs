using System.Collections.Generic;

namespace Mundane.ViewEngines.Mustache.Compilation
{
	internal readonly struct ParserError
	{
		internal readonly string ErrorMessage;

		internal ParserError(string filePath, Token token, TokenType tokenType)
			: this(filePath, token, ParserError.TokenString(token, tokenType))
		{
		}

		internal ParserError(string filePath, Token token, string message)
		{
			this.ErrorMessage = $"{filePath} Ln {token.Line + 1} Ch {token.Character + 1}: " + message;
		}

		private static string TokenString(Token token, TokenType tokenType)
		{
			var tokenStrings = new Dictionary<TokenType, string>
			{
				{ TokenType.CloseBlock, "{{/" },
				{ TokenType.CloseTag, "}}" },
				{ TokenType.Identifier, "identifier" },
				{ TokenType.InvertedBlock, "{{^" },
				{ TokenType.OpenBlock, "{{#" },
				{ TokenType.OpenTag, "{{" },
				{ TokenType.Text, "text" },
				{ TokenType.Replacement, "{{$" }
			};

			if (tokenType == TokenType.End)
			{
				return "Unexpected token \"" + tokenStrings[token.TokenType] + "\".";
			}

			return tokenStrings.TryGetValue(tokenType, out var tokenString)
				? tokenString + " expected."
				: "Unknown error.";
		}
	}
}
