using System.Text.Json.Serialization;

namespace Installer.Manifest;

/// <summary>
///     Provides the source-generated JSON metadata of the installer manifest.
/// </summary>
/// <remarks>The build pipeline writes the manifest, and the installer project reads it.</remarks>
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, WriteIndented = true, GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(InstallerManifest))]
public sealed partial class InstallerJsonContext : JsonSerializerContext;
