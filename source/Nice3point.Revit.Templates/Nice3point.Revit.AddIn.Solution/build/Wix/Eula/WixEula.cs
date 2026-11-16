using ModularPipelines.Context;
using ModularPipelines.Options;

namespace Build.Wix.Eula;

/// <summary>
///     Provides the <c>wix eula</c> commands that the pipeline runs through the command line.
/// </summary>
internal sealed class WixEula(ICommandContext command) : IWixEula
{
    /// <inheritdoc />
    public Task<CommandResult> AcceptAsync(WixEulaAcceptOptions options, CommandExecutionOptions? executionOptions = null, CancellationToken cancellationToken = default)
    {
        return command.ExecuteCommandLineToolAsync(options, executionOptions, cancellationToken);
    }
}
