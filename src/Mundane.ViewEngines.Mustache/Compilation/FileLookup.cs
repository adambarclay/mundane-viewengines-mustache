using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.FileProviders;

namespace Mundane.ViewEngines.Mustache.Compilation
{
	internal static class FileLookup
	{
		internal static List<(string Path, IFileInfo File)> LookupFiles(IFileProvider fileProvider)
		{
			var files = new List<(string, IFileInfo)>();

			FileLookup.GetFiles(fileProvider, "/", files);

			return files;
		}

		private static void GetFiles(IFileProvider fileProvider, string path, List<(string Path, IFileInfo File)> files)
		{
			foreach (var content in fileProvider.GetDirectoryContents(path)!.OrderBy(o => o.IsDirectory + o.Name))
			{
				if (content.IsDirectory)
				{
					FileLookup.GetFiles(fileProvider, Path.Combine(path, content.Name!), files);
				}
				else
				{
					files.Add((Path.Combine(path, content.Name!).Replace('\\', '/'), content));
				}
			}
		}
	}
}
