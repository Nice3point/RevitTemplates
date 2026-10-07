using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace RevitAddIn.ServiceDefaults.Diagnostics;

/// <summary>
///     Represents a hosted service that writes unhandled <see cref="AppDomain" /> exceptions to the log while the host is running.
/// </summary>
/// <param name="logger">The logger for unhandled exceptions.</param>
public sealed partial class AppDomainExceptionsHandler(ILogger<AppDomainExceptionsHandler> logger) : IHostedService
{
    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
        return Task.CompletedTask;
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
    {
        if (args.ExceptionObject is Exception exception)
        {
            LogDomainUnhandledException(logger, exception);
            return;
        }

        LogNonExceptionDomainUnhandledException(logger, args.IsTerminating);
    }

    [LoggerMessage(LogLevel.Critical, "An unhandled exception occurred in the application domain.")]
    private static partial void LogDomainUnhandledException(ILogger<AppDomainExceptionsHandler> logger, Exception exception);

    [LoggerMessage(LogLevel.Critical, "An unhandled object that is not an exception was thrown in the application domain. Terminating: {IsTerminating}.")]
    private static partial void LogNonExceptionDomainUnhandledException(ILogger<AppDomainExceptionsHandler> logger, bool isTerminating);
}
