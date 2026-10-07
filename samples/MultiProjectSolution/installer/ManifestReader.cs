using System.Text.Json;

namespace Installer;

/// <summary>
///     Provides extension methods for <see cref="FileInfo" /> to read the installer manifest.
/// </summary>
public static class ManifestReader
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <param name="file">The manifest file.</param>
    extension(FileInfo file)
    {
        /// <summary>
        ///     Reads the installer manifest from the JSON file.
        /// </summary>
        /// <returns>The deserialized <see cref="Manifest" />.</returns>
        /// <exception cref="FileNotFoundException">The file doesn't exist.</exception>
        /// <exception cref="JsonException">The file contains JSON that is not valid, or the JSON lacks a required property.</exception>
        /// <exception cref="InvalidDataException">The file contains the JSON <c>null</c> literal.</exception>
        public Manifest ReadManifest()
        {
            if (!file.Exists)
            {
                throw new FileNotFoundException($"Cannot read the installer manifest. The file '{file.FullName}' was not found.", file.FullName);
            }

            using var stream = file.OpenRead();
            var manifest = JsonSerializer.Deserialize<Manifest>(stream, SerializerOptions);
            if (manifest is null)
            {
                throw new InvalidDataException($"Cannot read the installer manifest. The file '{file.FullName}' contains the JSON null literal.");
            }

            return manifest;
        }
    }
}
