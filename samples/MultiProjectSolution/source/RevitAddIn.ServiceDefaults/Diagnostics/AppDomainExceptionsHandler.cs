using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace RevitAddIn.ServiceDefaults.Diagnostics;

/// <summary>
///     Represents a hosted service that writes unhandled <see cref="AppDomain" /> exceptions to the log while the host is running.
/// </summary>
/// <param name="logger">The logger the service writes unhandled exceptions to.</param>
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

    [LoggerMessage(LogLevel.Critical, "Domain unhandled exception")]
    private static partial void LogDomainUnhandledException(ILogger<AppDomainExceptionsHandler> logger, Exception exception);

    [LoggerMessage(LogLevel.Critical, "Domain unhandled non-exception object, terminating: {isTerminating}")]
    private static partial void LogNonExceptionDomainUnhandledException(ILogger<AppDomainExceptionsHandler> logger, bool isTerminating);
}
