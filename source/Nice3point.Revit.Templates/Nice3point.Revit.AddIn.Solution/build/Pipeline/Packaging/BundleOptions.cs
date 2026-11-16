using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Build.Pipeline.Packaging;

/// <summary>
///     Represents the vendor details of the Autodesk application bundle.
/// </summary>
/// <seealso href="https://www.autodesk.com/autodesk-university/class/AppBundle-Cross-Distribution-Autodesk-Products-App-Store-and-Forge-2020">AppBundle: Cross-Distribution Autodesk Products, App Store, and Forge</seealso>
[PublicAPI]
public sealed class BundleOptions
{
    /// <summary>
    ///     The name of the configuration section that contains the options.
    /// </summary>
    public const string ConfigurationSectionName = "Bundle";

    /// <summary>
    ///     Gets or sets the name of the vendor that publishes the add-in.
    /// </summary>
    [Required]
    public string VendorName { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the URL of the vendor website.
    /// </summary>
    public string? VendorUrl { get; set; }

    /// <summary>
    ///     Gets or sets the email address of the vendor.
    /// </summary>
    public string? VendorEmail { get; set; }
}
