using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RevitAddIn.Diagnostics;

namespace RevitAddIn.Logging;

/// <summary>
///     Provides extension methods for <see cref="IHostApplicationBuilder" /> to add the application logging defaults.
/// </summary>
/// <example>
/// <code lang="csharp">
/// public partial class Class(ILogger&lt;Class&gt; logger)
/// {
///     private void Execute()
///     {
///         LogMessage(logger);
///     }
///
///     [LoggerMessage(LogLevel.Information, "Message")]
///     private static partial void LogMessage(ILogger&lt;Class&gt; logger);
/// }
/// </code>
/// </example>
public static class LoggingRegistration
{
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
                //["Logging:RevitJournal:LogLevel:Default"] = nameof(LogLevel.Error)
            });

            builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
            builder.Logging.AddDebug();

            //TODO: uncomment the Revit journal provider and its log level after the Nice3point.Revit.Logging release
            //builder.Logging.AddRevitJournal();

            builder.Services.AddHostedService<AppDomainExceptionsHandler>();
            builder.Services.Configure<ConsoleLifetimeOptions>(options => options.SuppressStatusMessages = true);

            return builder;
        }
    }
}
