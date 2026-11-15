using ModularPipelines.Attributes;

namespace Build.Wix;

/// <summary>
///     Represents the options of the <c>wix extension add</c> command.
/// </summary>
/// <param name="ExtensionReference">The package identifier of the extension, followed by an optional version.</param>
/// <example><c>WixToolset.UI.wixext</c> or <c>WixToolset.UI.wixext/7.0.0</c>.</example>
[CliSubCommand("extension", "add")]
public sealed record WixExtensionAddOptions([property: CliArgument] string ExtensionReference) : WixOptions
{
    /// <summary>
    ///     Gets or initializes a value indicating whether the extension is added to the extension cache of the current user.
    /// </summary>
    /// <remarks>If the value is not <see langword="true" />, the extension is added to the extension cache of the working directory.</remarks>
    [CliFlag("--global", ShortForm = "-g")]
    public bool? Global { get; init; }
}
