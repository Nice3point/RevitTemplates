using Build.Wix.Eula;
using Build.Wix.Extension;

namespace Build.Wix;

/// <summary>
///     Defines a contract that provides the command groups of the WiX Toolset.
/// </summary>
public interface IWix
{
    /// <summary>
    ///     Gets the commands of the <c>wix eula</c> group.
    /// </summary>
    IWixEula Eula { get; }

    /// <summary>
    ///     Gets the commands of the <c>wix extension</c> group.
    /// </summary>
    IWixExtension Extension { get; }
}
