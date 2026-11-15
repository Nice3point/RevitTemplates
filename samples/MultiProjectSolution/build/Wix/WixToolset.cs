using Build.Wix.Eula;
using Build.Wix.Extension;

namespace Build.Wix;

/// <summary>
///     Provides the command groups of the WiX Toolset that the pipeline runs through the command line.
/// </summary>
internal sealed class WixToolset(IWixEula eula, IWixExtension extension) : IWix
{
    /// <inheritdoc />
    public IWixEula Eula { get; } = eula;

    /// <inheritdoc />
    public IWixExtension Extension { get; } = extension;
}
