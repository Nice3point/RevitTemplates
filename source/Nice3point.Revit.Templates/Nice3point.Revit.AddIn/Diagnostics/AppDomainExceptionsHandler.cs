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
/// <param name="logger">The logger for unhandled exceptions.</param>
#if (diContainer)
public sealed partial class AppDomainExceptionsHandler(ILogger<AppDomainExceptionsHandler> logger)
#elseif (diHosting)
public sealed partial class AppDomainExceptionsHandler(ILogger<AppDomainExceptionsHandler> logger) : IHostedService
#endif
{
#if (diContainer)
    /// <summary>
    ///     Subscribes to unhandled <see cref="AppDomain" /> exceptions and writes them to the log.
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

    [LoggerMessage(LogLevel.Critical, "An unhandled exception occurred in the application domain.")]
    private static partial void LogDomainUnhandledException(ILogger<AppDomainExceptionsHandler> logger, Exception exception);

    [LoggerMessage(LogLevel.Critical, "An unhandled object that is not an exception was thrown in the application domain. Terminating: {IsTerminating}.")]
    private static partial void LogNonExceptionDomainUnhandledException(ILogger<AppDomainExceptionsHandler> logger, bool isTerminating);
}
