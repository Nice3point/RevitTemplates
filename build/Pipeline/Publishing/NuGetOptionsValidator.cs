using Microsoft.Extensions.Options;

namespace Build.Pipeline.Publishing;

/// <summary>
///     Validates the options of the NuGet publication.
/// </summary>
[OptionsValidator]
internal sealed partial class NuGetOptionsValidator : IValidateOptions<NuGetOptions>;
