#if (diContainer)
using Microsoft.Extensions.DependencyInjection;
#elseif (diHosting)
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
#endif
using Microsoft.Extensions.Logging;
using Nice3point.Revit.Logging;
using Nice3point.Revit.ServiceDefaults._1.Diagnostics;

namespace Nice3point.Revit.ServiceDefaults._1.Logging;

/// <summary>
#if (diContainer)
///     Provides extension methods for <see cref="IServiceCollection" /> to add the application logging defaults.
#elseif (diHosting)
///     Provides extension methods for <see cref="IHostApplicationBuilder" /> to add the application logging defaults.
#endif
/// </summary>
/// <example>
///     <code lang="csharp">
///         public partial class Class(ILogger&lt;Class&gt; logger)
///         {
///             private void Execute()
///             {
///                 LogMessage(logger);
///             }
///
///             [LoggerMessage(LogLevel.Information, "Message")]
///             private static partial void LogMessage(ILogger&lt;Class&gt; logger);
///         }
///     </code>
/// </example>
public static class LoggingRegistration
{
#if (diContainer)
    /// <param name="services">The <see cref="IServiceCollection" /> to add the services to.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Adds the default logging providers to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <returns>The <see cref="IServiceCollection" /> for chaining.</returns>
        public IServiceCollection AddLoggingDefaults()
        {
            services.AddLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Debug);
                logging.AddDebug();
                logging.AddRevitJournal(RevitApiContext.Application);
                logging.AddFilter<RevitJournalLoggerProvider>(null, LogLevel.Error);
            });

            services.AddSingleton<AppDomainExceptionsHandler>();

            return services;
        }
    }
#elseif (diHosting)
    /// <typeparam name="TBuilder">The host application builder type.</typeparam>
    /// <param name="builder">The host application builder.</param>
    extension<TBuilder>(TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        /// <summary>
        ///     Adds the default logging providers to the specified <see cref="IHostApplicationBuilder" />.
        /// </summary>
        /// <returns>The <typeparamref name="TBuilder" /> for chaining.</returns>
        public TBuilder AddLoggingDefaults()
        {
            builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Logging:LogLevel:Default"] = nameof(LogLevel.Debug),
                ["Logging:RevitJournal:LogLevel:Default"] = nameof(LogLevel.Error)
            });

            builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
            builder.Logging.AddDebug();
            builder.Logging.AddRevitJournal(RevitApiContext.Application, options => options.ApplicationName = builder.Environment.ApplicationName);

            builder.Services.AddHostedService<AppDomainExceptionsHandler>();
            builder.Services.Configure<ConsoleLifetimeOptions>(options => options.SuppressStatusMessages = true);

            return builder;
        }
    }
#endif
}
