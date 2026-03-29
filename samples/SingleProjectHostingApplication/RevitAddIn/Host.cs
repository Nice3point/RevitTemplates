using System.IO;
using System.Reflection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RevitAddIn.Views;
using RevitAddIn.ViewModels;

namespace RevitAddIn;

/// <summary>
///     Provides a host for the application's services and manages their lifetimes
/// </summary>
public static class Host
{
    private static IHost? _host;

    /// <summary>
    ///     Starts the host and configures the application's services
    /// </summary>
    public static async Task StartAsync()
    {
        var builder = new HostApplicationBuilder(new HostApplicationBuilderSettings
        {
            ContentRootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            DisableDefaults = true
        });

        builder.Services.AddTransient<RevitAddInViewModel>();
        builder.Services.AddTransient<RevitAddInView>();

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

    /// <summary>
    ///     Get service of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type of service object to get</typeparam>
    /// <exception cref="System.InvalidOperationException">There is no service of type <typeparamref name="T"/></exception>
    public static T GetService<T>() where T : class
    {
        return _host.Services.GetRequiredService<T>();
    }
}