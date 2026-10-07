using System.Text.Json;
using System.Text.Json.Serialization;

using uaParserLibrary.Models;

namespace uaParserDemoComponents.Interop;

/// <summary>
/// JSON for the demos, generated at build time so it also works in trimmed WebAssembly apps.
/// Same shape as ASP.NET Core's JSON: camelCase names, null values included.
/// </summary>
[JsonSourceGenerationOptions(JsonSerializerDefaults.Web, WriteIndented = true)]
[JsonSerializable(typeof(ClientInfo))]
[JsonSerializable(typeof(JsClientHints))]
public partial class DemoJson : JsonSerializerContext;
