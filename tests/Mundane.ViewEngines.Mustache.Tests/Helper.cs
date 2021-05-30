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
		internal static async ValueTask<string> Results(string templatePath)
		{
			var fileProvider = new ManifestEmbeddedFileProvider(typeof(Helper).Assembly, "/Results");

			await using (var file = fileProvider.GetFileInfo(templatePath)!.CreateReadStream()!)
			{
				using (var reader = new StreamReader(file, Encoding.UTF8))
				{
					return await reader.ReadToEndAsync();
				}
			}
		}

		internal static async ValueTask<string> Run(Func<Stream, ValueTask> func)
		{
			await using (var stream = new MemoryStream())
			{
				await func(stream);

				return Encoding.UTF8.GetString(stream.ToArray());
			}
		}

		internal static ValueTask<string> Run(MustacheViews views, BodyWriter bodyWriter)
		{
			return Helper.Run(new Dependencies(new Dependency<MustacheViews>(views)), bodyWriter);
		}

		internal static async ValueTask<string> Run(DependencyFinder dependencyFinder, BodyWriter bodyWriter)
		{
			var response = await MundaneEngine.ExecuteRequest(
				MundaneEndpointFactory.Create(() => Response.Ok(bodyWriter)),
				new FakeRequest(dependencyFinder));

			await using (var stream = new MemoryStream())
			{
				await response.WriteBodyToStream(stream);

				return Encoding.UTF8.GetString(stream.ToArray());
			}
		}

		private sealed class FakeRequest : Request
		{
			private readonly DependencyFinder dependencyFinder;

			internal FakeRequest(DependencyFinder dependencyFinder)
			{
				this.dependencyFinder = dependencyFinder;
			}

			public EnumerableCollection<KeyValuePair<string, string>> AllCookies
			{
				get
				{
					return new EnumerableCollection<KeyValuePair<string, string>>(
						new List<KeyValuePair<string, string>>(0));
				}
			}

			public EnumerableCollection<KeyValuePair<string, FileUpload>> AllFileParameters
			{
				get
				{
					return new EnumerableCollection<KeyValuePair<string, FileUpload>>(
						new List<KeyValuePair<string, FileUpload>>(0));
				}
			}

			public EnumerableCollection<KeyValuePair<string, string>> AllFormParameters
			{
				get
				{
					return new EnumerableCollection<KeyValuePair<string, string>>(
						new List<KeyValuePair<string, string>>(0));
				}
			}

			public EnumerableCollection<KeyValuePair<string, string>> AllHeaders
			{
				get
				{
					return new EnumerableCollection<KeyValuePair<string, string>>(
						new List<KeyValuePair<string, string>>(0));
				}
			}

			public EnumerableCollection<KeyValuePair<string, string>> AllQueryParameters
			{
				get
				{
					return new EnumerableCollection<KeyValuePair<string, string>>(
						new List<KeyValuePair<string, string>>(0));
				}
			}

			public Stream Body
			{
				get
				{
					return new MemoryStream(Array.Empty<byte>());
				}
			}

			public string Host
			{
				get
				{
					return string.Empty;
				}
			}

			public string Method
			{
				get
				{
					return HttpMethod.Get;
				}
			}

			public string Path
			{
				get
				{
					return "/";
				}
			}

			public string PathBase
			{
				get
				{
					return string.Empty;
				}
			}

			public CancellationToken RequestAborted
			{
				get
				{
					return CancellationToken.None;
				}
			}

			public string Scheme
			{
				get
				{
					return "https";
				}
			}

			public string Cookie(string cookieName)
			{
				return string.Empty;
			}

			public bool CookieExists(string cookieName)
			{
				return false;
			}

			public T Dependency<T>()
				where T : notnull
			{
				return this.dependencyFinder.Find<T>(this);
			}

			public FileUpload File(string parameterName)
			{
				return FileUpload.Unknown;
			}

			public bool FileExists(string parameterName)
			{
				return false;
			}

			public string Form(string parameterName)
			{
				return string.Empty;
			}

			public bool FormExists(string parameterName)
			{
				return false;
			}

			public string Header(string headerName)
			{
				return string.Empty;
			}

			public bool HeaderExists(string headerName)
			{
				return false;
			}

			public string Query(string parameterName)
			{
				return string.Empty;
			}

			public bool QueryExists(string parameterName)
			{
				return false;
			}

			public string Route(string parameterName)
			{
				return string.Empty;
			}
		}
	}
}
