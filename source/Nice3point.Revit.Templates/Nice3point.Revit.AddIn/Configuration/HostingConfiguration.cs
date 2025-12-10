using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Nice3point.Revit.AddIn.Configuration;

/// <summary>
///     Application hosting configuration.
/// </summary>
public static class HostingConfiguration
{
    /// <summary>
    ///     Configures the application hosting environment settings.
    /// </summary>
    public static TBuilder ConfigureHosting<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.Configure<ConsoleLifetimeOptions>(options => options.SuppressStatusMessages = true);

        return builder;
    }
}