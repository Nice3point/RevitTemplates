using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using JetBrains.Annotations;

namespace Installer.Manifest;

/// <summary>
///     Provides extension methods for <see cref="FileInfo" /> to read a JSON file with source-generated metadata.
/// </summary>
[PublicAPI]
public static class JsonFileExtensions
{
    /// <summary>
    ///     Defines extensions for <see cref="FileInfo" />.
    /// </summary>
    /// <param name="file">The JSON file to read.</param>
    extension(FileInfo file)
    {
        /// <summary>
        ///     Reads the value of the JSON file.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="typeInfo">The source-generated metadata of the value.</param>
        /// <returns>The deserialized value.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="typeInfo" /> is <see langword="null" />.</exception>
        /// <exception cref="FileNotFoundException">The file doesn't exist.</exception>
        /// <exception cref="JsonException">The file contains JSON that is not valid, or the JSON lacks a required property.</exception>
        /// <exception cref="InvalidDataException">The file contains the JSON <c>null</c> literal.</exception>
        public T ReadJson<T>(JsonTypeInfo<T> typeInfo)
        {
            ArgumentNullException.ThrowIfNull(typeInfo);

            if (!file.Exists)
            {
                throw new FileNotFoundException($"Cannot read the file '{file.FullName}'. The file was not found.", file.FullName);
            }

            using var stream = file.OpenRead();
            var value = JsonSerializer.Deserialize(stream, typeInfo);
            if (value is null)
            {
                throw new InvalidDataException($"Cannot read the file '{file.FullName}'. The file contains the JSON null literal in place of an object.");
            }

            return value;
        }
    }
}
