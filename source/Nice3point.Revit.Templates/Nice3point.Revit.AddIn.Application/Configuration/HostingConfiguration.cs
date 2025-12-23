using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Nice3point.Revit.AddIn.Configuration;

/// <summary>
///     Application hosting configuration.
/// </summary>
public static class HostingConfiguration
{
    extension<TBuilder>(TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        /// <summary>
        ///     Configures the application hosting environment settings.
        /// </summary>
        public TBuilder ConfigureHosting()
        {
            builder.Services.Configure<ConsoleLifetimeOptions>(options => options.SuppressStatusMessages = true);

            return builder;
        }
    }
}