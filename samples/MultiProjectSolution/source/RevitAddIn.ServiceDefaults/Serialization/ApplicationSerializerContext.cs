using System.Text.Json.Serialization;

namespace RevitAddIn.ServiceDefaults.Serialization;

/// <summary>
///     Provides the source-generated JSON serialization metadata of the application.
/// </summary>
/// <remarks>A type absent from this context has no metadata at run time, and serializing it throws.</remarks>
[JsonSerializable(typeof(ProjectMetadata))]
public sealed partial class ApplicationSerializerContext : JsonSerializerContext;
