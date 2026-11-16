using ModularPipelines.Context;
using ModularPipelines.Options;

namespace Build.Wix.Extension;

/// <summary>
///     Provides the <c>wix extension</c> commands that the pipeline runs through the command line.
/// </summary>
internal sealed class WixExtension(ICommandContext command) : IWixExtension
{
    /// <inheritdoc />
    public Task<CommandResult> AddAsync(WixExtensionAddOptions options, CommandExecutionOptions? executionOptions = null, CancellationToken cancellationToken = default)
    {
        return command.ExecuteCommandLineToolAsync(options, executionOptions, cancellationToken);
    }
}
