using Microsoft.Extensions.Options;

namespace Build.Pipeline.Packaging;

/// <summary>
///     Validates the options of the installer packages.
/// </summary>
[OptionsValidator]
internal sealed partial class InstallerOptionsValidator : IValidateOptions<InstallerOptions>;
