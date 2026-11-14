using ModularPipelines.Context;
using ModularPipelines.Options;

namespace Build.Wix;

/// <summary>
///     Provides the commands of the WiX Toolset that the pipeline runs through the command line.
/// </summary>
internal sealed class WixToolset(ICommandContext command) : IWix
{
    /// <inheritdoc />
    public Task<CommandResult> EulaAcceptAsync(WixEulaAcceptOptions options, CommandExecutionOptions? executionOptions = null, CancellationToken cancellationToken = default)
    {
        return command.ExecuteCommandLineToolAsync(options, executionOptions, cancellationToken);
    }

    /// <inheritdoc />
    public Task<CommandResult> ExtensionAddAsync(WixExtensionAddOptions options, CommandExecutionOptions? executionOptions = null, CancellationToken cancellationToken = default)
    {
        return command.ExecuteCommandLineToolAsync(options, executionOptions, cancellationToken);
    }
}
