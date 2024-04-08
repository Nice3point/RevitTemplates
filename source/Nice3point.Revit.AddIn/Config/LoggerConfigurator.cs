#if (Container)
using Microsoft.Extensions.DependencyInjection;
#endif
#if (Hosting)
using Microsoft.Extensions.Logging;
#endif
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Nice3point.Revit.AddIn.Config;

/// <summary>
///     Application logging configuration
/// </summary>
/// <example>
/// <code lang="csharp">
#if (Container)
/// public class Class(ILogger logger)
/// {
///     private void Execute()
///     {
///         logger.Information("Message");
///     }
/// }
#elseif (Hosting)
/// public class Class(ILogger&lt;Class&gt; logger)
/// {
///     private void Execute()
///     {
///         logger.LogInformation("Message");
///     }
/// }
#endif
/// </code>
/// </example>
public static class LoggerConfigurator
{
#if (Container)
    private const string LogTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}]: {Message:lj}{NewLine}{Exception}";
#elseif (Hosting)
    private const string LogTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}";
#endif

#if (Container)
    public static void AddSerilogConfiguration(this IServiceCollection services)
    {
        var logger = CreateDefaultLogger();
        services.AddSingleton<ILogger>(logger);

        AppDomain.CurrentDomain.UnhandledException += OnOnUnhandledException;
    }
#elseif (Hosting)
    public static void AddSerilogConfiguration(this ILoggingBuilder builder)
    {
        var logger = CreateDefaultLogger();
        builder.AddSerilog(logger);

        AppDomain.CurrentDomain.UnhandledException += OnOnUnhandledException;
    }
#endif

    private static Logger CreateDefaultLogger()
    {
        return new LoggerConfiguration()
            .WriteTo.Debug(LogEventLevel.Debug, LogTemplate)
            .MinimumLevel.Debug()
            .CreateLogger();
    }

#if (Container)
    private static void OnOnUnhandledException(object sender, UnhandledExceptionEventArgs args)
    {
        var exception = (Exception)args.ExceptionObject;
        var logger = Host.GetService<ILogger>();
        logger.Fatal(exception, "Domain unhandled exception");
    }
#elseif (Hosting)
    private static void OnOnUnhandledException(object sender, UnhandledExceptionEventArgs args)
    {
        var exception = (Exception)args.ExceptionObject;
        var logger = Host.GetService<ILogger<AppDomain>>();
        logger.LogCritical(exception, "Domain unhandled exception");
    }
#endif
}