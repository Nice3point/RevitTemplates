using Microsoft.Extensions.Logging;
using Serilog.Core;
using Serilog.Events;

namespace RevitAddIn.Config;

public static class LoggerConfigurator
{
    private const string LogTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}";

    public static void AddSerilogConfiguration(this ILoggingBuilder builder)
    {
        var logger = CreateDefaultLogger();
        builder.AddSerilog(logger);

        AppDomain.CurrentDomain.UnhandledException += OnOnUnhandledException;
    }

    private static Logger CreateDefaultLogger()
    {
        return new LoggerConfiguration()
            .WriteTo.Debug(LogEventLevel.Debug, LogTemplate)
            .MinimumLevel.Debug()
            .CreateLogger();
    }

    private static void OnOnUnhandledException(object sender, UnhandledExceptionEventArgs args)
    {
        var exception = (Exception)args.ExceptionObject;
        var logger = Host.GetService<ILogger<AppDomain>>();
        logger.LogCritical(exception, "Domain unhandled exception");
    }
}