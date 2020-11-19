using System;
using Microsoft.Extensions.FileProviders;

namespace Mundane.ViewEngines.Mustache
{
	/// <summary>A collection of view templates.</summary>
	public sealed class Views
	{
		/// <summary>Initializes a new instance of the <see cref="Views"/> class.</summary>
		/// <param name="viewFileProvider">The view template file provider.</param>
		public Views(IFileProvider viewFileProvider)
		{
			if (viewFileProvider == null)
			{
				throw new ArgumentNullException(nameof(viewFileProvider));
			}

			this.ViewFileProvider = viewFileProvider;
		}

		/// <summary>Gets the view template file provider.</summary>
		public IFileProvider ViewFileProvider { get; }
	}
}
