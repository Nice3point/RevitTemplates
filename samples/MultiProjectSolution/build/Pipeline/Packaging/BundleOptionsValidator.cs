using Microsoft.Extensions.Options;

namespace Build.Pipeline.Packaging;

/// <summary>
///     Validates the vendor details of the Autodesk application bundle.
/// </summary>
[OptionsValidator]
internal sealed partial class BundleOptionsValidator : IValidateOptions<BundleOptions>;
