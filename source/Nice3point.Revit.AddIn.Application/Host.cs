#if (Hosting)
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Hosting;
#endif
#if (log && Hosting)
using Microsoft.Extensions.Logging;
#endif
using Microsoft.Extensions.DependencyInjection;
#if (log && UseIoc)
using Nice3point.Revit.AddIn.Config;
#endif

namespace Nice3point.Revit.AddIn;

/// <summary>
///     Provides a host for the application's services and manages their lifetimes
/// </summary>
public static class Host
{
#if (Container)
    private static IServiceProvider _serviceProvider;
#endif
#if (Hosting)
    private static IHost _host;
#endif

    /// <summary>
    ///     Starts the host and configures the application's services
    /// </summary>
    public static void Start()
    {
#if (Container)
        var services = new ServiceCollection();
#if (log)

        services.AddSerilogConfiguration();
#endif

        _serviceProvider = services.BuildServiceProvider();
#endif
#if (Hosting)
        var builder = new HostApplicationBuilder(new HostApplicationBuilderSettings
        {
            ContentRootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly()!.Location),
            DisableDefaults = true
        });
#if (log)

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilogConfiguration();
#endif

        _host = builder.Build();
        _host.Start();
#endif
    }
#if (Hosting)

    /// <summary>
    ///     Stops the host and handle <see cref="IHostedService"/> services
    /// </summary>
    public static void Stop()
    {
        _host.StopAsync().GetAwaiter().GetResult();
    }
#endif

    /// <summary>
    ///     Get service of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type of service object to get</typeparam>
    /// <exception cref="System.InvalidOperationException">There is no service of type <typeparamref name="T"/></exception>
    public static T GetService<T>() where T : class
    {
#if (Container)
        return _serviceProvider.GetRequiredService<T>();
#endif
#if (Hosting)
        return _host.Services.GetRequiredService<T>();
#endif
    }
}