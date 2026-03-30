#if (diHosting)
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Hosting;
#endif
#if (diHosting && addinLogging)
using Microsoft.Extensions.Logging;
#endif
using Microsoft.Extensions.DependencyInjection;
#if (diHosting || addinLogging)
using Nice3point.Revit.AddIn._1.Configuration;
#endif

namespace Nice3point.Revit.AddIn._1;

/// <summary>
///     Provides a host for the application's services and manages their lifetimes
/// </summary>
public static class Host
{
#if (diContainer)
    private static IServiceProvider? _serviceProvider;
#endif
#if (diHosting)
    private static IHost? _host;
#endif

    /// <summary>
    ///     Starts the host and configures the application's services
    /// </summary>
#if (diHosting)
    public static async Task StartAsync()
    {
        var builder = new HostApplicationBuilder(new HostApplicationBuilderSettings
        {
            ContentRootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            DisableDefaults = true
        });
#if (addinLogging)

        //Logging
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog();
#endif

        //Configuration
        builder.ConfigureHosting();

        _host = builder.Build();
        await _host.StartAsync();
    }

    /// <summary>
    ///     Stops the host and handle <see cref="IHostedService"/> services
    /// </summary>
    public static async Task StopAsync()
    {
        if (_host is null) return;

        await _host.StopAsync();
    }
#else
    public static void Start()
    {
#if (diContainer)
        var services = new ServiceCollection();
#if (addinLogging)

        //Logging
        services.AddSerilog();
#endif

        _serviceProvider = services.BuildServiceProvider();
#endif
    }
#endif

    /// <summary>
    ///     Get service of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type of service object to get</typeparam>
    /// <exception cref="System.InvalidOperationException">There is no service of type <typeparamref name="T"/></exception>
    public static T GetService<T>() where T : class
    {
#if (diContainer)
        return _serviceProvider!.GetRequiredService<T>();
#endif
#if (diHosting)
        return _host!.Services.GetRequiredService<T>();
#endif
    }
}