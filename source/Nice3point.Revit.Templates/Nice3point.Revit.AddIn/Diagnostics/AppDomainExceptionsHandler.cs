#if (diHosting)
using Microsoft.Extensions.Hosting;
#endif
using Microsoft.Extensions.Logging;

namespace Nice3point.Revit.AddIn._1.Diagnostics;

/// <summary>
#if (diContainer)
///     Represents a service that writes unhandled <see cref="AppDomain" /> exceptions to the log.
#elseif (diHosting)
///     Represents a hosted service that writes unhandled <see cref="AppDomain" /> exceptions to the log while the host is running.
#endif
/// </summary>
/// <param name="logger">The logger the service writes unhandled exceptions to.</param>
#if (diContainer)
public sealed partial class AppDomainExceptionsHandler(ILogger<AppDomainExceptionsHandler> logger)
#elseif (diHosting)
public sealed partial class AppDomainExceptionsHandler(ILogger<AppDomainExceptionsHandler> logger) : IHostedService
#endif
{
#if (diContainer)
    /// <summary>
    ///     Starts writing unhandled <see cref="AppDomain" /> exceptions to the log.
    /// </summary>
    public void LogExceptions()
    {
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
    }
#elseif (diHosting)
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
#endif

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
