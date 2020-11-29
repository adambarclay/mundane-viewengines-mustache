using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;

namespace Mundane.ViewEngines.Mustache.Tests
{
	[ExcludeFromCodeCoverage]
	internal static class Helper
	{
		internal static async Task<string> Execute(MustacheViews views, Func<ResponseStream, Task> bodyWriter)
		{
			return await Helper.Execute(new Dependencies(new Dependency<MustacheViews>(views)), bodyWriter);
		}

		internal static async Task<string> Execute(
			DependencyFinder dependencyFinder,
			Func<ResponseStream, Task> bodyWriter)
		{
			var response = await MundaneEngine.ExecuteRequest(
				MundaneEndpoint.Create(() => Response.Ok(bodyWriter)),
				new Request(
					string.Empty,
					string.Empty,
					new Dictionary<string, string>(0),
					new Dictionary<string, string>(0),
					new MemoryStream(Array.Empty<byte>(), false),
					new Dictionary<string, string>(0),
					new Dictionary<string, string>(0),
					new Dictionary<string, string>(0),
					new Dictionary<string, FileUpload>(0),
					dependencyFinder,
					new RequestHost(string.Empty, string.Empty, string.Empty),
					CancellationToken.None));

			await using (var stream = new MemoryStream())
			{
				await response.WriteBodyToStream(stream);

				return Encoding.UTF8.GetString(stream.ToArray());
			}
		}

		internal static async Task<string> Results(string templatePath)
		{
			var fileProvider = new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Results");

			await using (var file = fileProvider.GetFileInfo(templatePath).CreateReadStream())
			{
				using (var reader = new StreamReader(file, Encoding.UTF8))
				{
					return await reader.ReadToEndAsync();
				}
			}
		}
	}
}
