#if (diHosting)
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Hosting;
#endif
using Microsoft.Extensions.DependencyInjection;

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

#if (diHosting && (isApplicationAddin || isCommandAddin))
    public static async Task StartAsync()
#else
    public static void Start()
#endif
    {
#if (diHosting)
        var builder = new HostApplicationBuilder(new HostApplicationBuilderSettings
        {
            ApplicationName = "Nice3point.Revit.AddIn.1",
            ContentRootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            DisableDefaults = true
        });

        _host = builder.Build();
#if (isApplicationAddin || isCommandAddin)
        await _host.StartAsync();
#else
        _host.StartAsync().GetAwaiter().GetResult();
#endif
#else
#if (diContainer)
        var services = new ServiceCollection();

        _serviceProvider = services.BuildServiceProvider();
#endif
#endif
    }
#if (diHosting)

    /// <summary>
    ///     Stops the host and handle <see cref="IHostedService"/> services
    /// </summary>
#if (isApplicationAddin || isCommandAddin)
    public static async Task StopAsync()
#else
    public static void Stop()
#endif
    {
        if (_host is null) return;

#if (isApplicationAddin || isCommandAddin)
        await _host.StopAsync();
#else
        _host.StopAsync().GetAwaiter().GetResult();
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
