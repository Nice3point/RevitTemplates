#if (diHosting)
using Path = System.IO.Path;
using System.Reflection;
using Microsoft.Extensions.Hosting;
#endif
using Microsoft.Extensions.DependencyInjection;

namespace Nice3point.Revit.AddIn._1;

/// <summary>
///     Provides a host for the application's services and manages their lifetimes.
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
    ///     Starts the host and registers the add-in services.
    /// </summary>
#if (diHosting && (isApplicationAddin || isCommandAddin))
    /// <returns>A task that represents the asynchronous host startup operation.</returns>
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
    ///     Stops the host and its <see cref="IHostedService" /> services.
    /// </summary>
#if (isApplicationAddin || isCommandAddin)
    /// <returns>A task that represents the asynchronous host shutdown operation.</returns>
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
    ///     Resolves a required service from the add-in service provider.
    /// </summary>
    /// <typeparam name="T">The type of service to resolve.</typeparam>
    /// <returns>The registered service instance.</returns>
    /// <exception cref="System.InvalidOperationException">No service of type <typeparamref name="T" /> is registered.</exception>
    /// <remarks>Service resolution requires a started host.</remarks>
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
