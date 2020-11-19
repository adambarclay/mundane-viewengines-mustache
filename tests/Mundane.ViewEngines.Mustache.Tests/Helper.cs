using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Moq;

namespace Mundane.ViewEngines.Mustache.Tests
{
	[ExcludeFromCodeCoverage]
	internal static class Helper
	{
		internal static readonly Views Views = new Views(
			new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Templates"));

		private static readonly IFileProvider ResultsFileProvider = new ManifestEmbeddedFileProvider(
			typeof(Helper).Assembly,
			"/Results");

		internal static async Task<string> Execute(Views views, Func<ResponseStream, Task> bodyWriter)
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
					new Dependencies(new Dependency<Views>(views)),
					new RequestHost(string.Empty, string.Empty, string.Empty),
					CancellationToken.None));

			await using (var stream = new MemoryStream())
			{
				await response.WriteBodyToStream(stream);

				return Encoding.UTF8.GetString(stream.ToArray());
			}
		}

		internal static Views NotFound(string templatePath)
		{
			var fileProvider = new Mock<IFileProvider>(MockBehavior.Strict);

			fileProvider.Setup(x => x.GetFileInfo(templatePath)).Returns(new NotFoundFileInfo(templatePath));

			return new Views(fileProvider.Object);
		}

		internal static async Task<string> Results(string templatePath)
		{
			await using (var file = Helper.ResultsFileProvider.GetFileInfo(templatePath).CreateReadStream())
			{
				using (var reader = new StreamReader(file, Encoding.UTF8))
				{
					return await reader.ReadToEndAsync();
				}
			}
		}
	}
}
