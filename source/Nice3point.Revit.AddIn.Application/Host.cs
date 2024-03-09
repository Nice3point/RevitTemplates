#if Hosting
using System.IO
using System.Reflection;
using Microsoft.Extensions.Hosting;
#endif
#if (log && Hosting)
using Microsoft.Extensions.Logging;
#endif
#if Container
using Microsoft.Extensions.DependencyInjection;
#endif
#if (log && UseIoc)
using Nice3point.Revit.AddIn.Config;
#endif

namespace Nice3point.Revit.AddIn;

/// <summary>
///     Provides a host for the application's services and manages their lifetimes.
/// </summary>
public static class Host
{
#if Container
    private static IServiceProvider _serviceProvider;
#endif
#if Hosting
    private static IHost _host;
#endif

    /// <summary>
    ///     Starts the host and configures the application's services.
    /// </summary>
    public static void Start()
    {
#if Container
        var services = new ServiceCollection();
#if log

        services.AddSerilogConfiguration();
#endif

        _serviceProvider = services.BuildServiceProvider();
#endif
#if Hosting
        var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            ContentRootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly()!.Location),
            DisableDefaults = true
        });
#if log

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilogConfiguration();
#endif

        _host = builder.Build();
        _host.Start();
#endif
    }
#if Hosting

    /// <summary>
    ///     Stops the host.
    /// </summary>
    public static void Stop()
    {
        _host.StopAsync();
    }
#endif

    /// <summary>
    ///     Gets a service of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of service object to get.</typeparam>
    /// <returns>A service object of type T or null if there is no such service.</returns>
    public static T GetService<T>() where T : class
    {
#if Container
        return _serviceProvider.GetService(typeof(T)) as T;
#endif
#if Hosting
        return _host.Services.GetService(typeof(T)) as T;
#endif
    }
}