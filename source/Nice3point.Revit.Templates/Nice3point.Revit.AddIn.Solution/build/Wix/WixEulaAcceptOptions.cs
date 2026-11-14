using ModularPipelines.Attributes;

namespace Build.Wix;

/// <summary>
///     Represents the options of the <c>wix eula accept</c> command.
/// </summary>
/// <param name="EulaId">The identifier of the end-user license agreement to accept.</param>
/// <example>
///     wix7
/// </example>
[CliSubCommand("eula", "accept")]
public sealed record WixEulaAcceptOptions([property: CliArgument] string EulaId) : WixOptions;
