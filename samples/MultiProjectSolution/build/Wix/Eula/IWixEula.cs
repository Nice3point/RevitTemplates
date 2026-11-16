using ModularPipelines.Options;

namespace Build.Wix.Eula;

/// <summary>
///     Defines a contract that runs the <c>wix eula</c> commands.
/// </summary>
public interface IWixEula
{
    /// <summary>
    ///     Accepts an end-user license agreement of the WiX Toolset.
    /// </summary>
    /// <param name="options">The options of the command.</param>
    /// <param name="executionOptions">The options of the command execution, or <see langword="null" /> for the default options.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> used to cancel the operation.</param>
    /// <returns>
    ///     A task that represents the asynchronous command execution.
    ///     The task result contains the result of the command.
    /// </returns>
    Task<CommandResult> AcceptAsync(WixEulaAcceptOptions options, CommandExecutionOptions? executionOptions = null, CancellationToken cancellationToken = default);
}
