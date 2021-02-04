using System;

namespace Mundane.ViewEngines.Mustache
{
	/// <summary>The exception thrown when a property appears in a template but not in the view model.</summary>
	public sealed class ViewModelPropertyNotFound : Exception
	{
		internal ViewModelPropertyNotFound(string[] identifiers)
			: base(ViewModelPropertyNotFound.CreateMessage(identifiers))
		{
		}

		private static string CreateMessage(string[] identifiers)
		{
			return "Property \"" + string.Join(".", identifiers) + "\" was not found.";
		}
	}
}
