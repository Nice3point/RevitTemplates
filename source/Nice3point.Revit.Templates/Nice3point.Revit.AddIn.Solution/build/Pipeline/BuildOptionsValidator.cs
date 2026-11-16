using Microsoft.Extensions.Options;

namespace Build.Pipeline;

/// <summary>
///     Validates the options that the steps of the build pipeline share.
/// </summary>
[OptionsValidator]
internal sealed partial class BuildOptionsValidator : IValidateOptions<BuildOptions>;
