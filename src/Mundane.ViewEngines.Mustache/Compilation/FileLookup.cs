using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.FileProviders;

namespace Mundane.ViewEngines.Mustache.Compilation
{
	internal static class FileLookup
	{
		internal static List<(string Path, IFileInfo File)> LookupFiles(IFileProvider fileProvider)
		{
			var files = new List<(string, IFileInfo)>();

			foreach (var content in fileProvider.GetDirectoryContents("/"))
			{
				if (content.IsDirectory)
				{
					FileLookup.GetFiles(fileProvider, content.Name, files);
				}
				else
				{
					files.Add((content.Name, content));
				}
			}

			return files;
		}

		private static void GetFiles(IFileProvider fileProvider, string path, List<(string Path, IFileInfo File)> files)
		{
			foreach (var content in fileProvider.GetDirectoryContents(path))
			{
				if (content.IsDirectory)
				{
					FileLookup.GetFiles(fileProvider, Path.Combine(path, content.Name), files);
				}
				else
				{
					files.Add((Path.Combine(path, content.Name).Replace('\\', '/'), content));
				}
			}
		}
	}
}
