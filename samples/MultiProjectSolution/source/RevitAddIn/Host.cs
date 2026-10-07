using Path = System.IO.Path;
using System.Reflection;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModalModule.ViewModels;
using ModalModule.Views;
using ModelessModule;
using ModelessModule.ViewModels;
using ModelessModule.Views;

namespace RevitAddIn;

/// <summary>
///     Provides a host for the application's services and manages their lifetimes.
/// </summary>
public static class Host
{
    private static IHost? _host;

    /// <summary>
    ///     Registers the add-in services and starts the host.
    /// </summary>
    /// <returns>A task that represents the asynchronous host startup operation.</returns>
    public static async Task StartAsync()
    {
        var builder = new HostApplicationBuilder(new HostApplicationBuilderSettings
        {
            ApplicationName = "RevitAddIn",
            ContentRootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            DisableDefaults = true
        });

        //Service defaults
        builder.AddServiceDefaults();

        //MVVM services
        builder.Services.AddScoped<ModalModuleView>();
        builder.Services.AddScoped<ModalModuleViewModel>();
        builder.Services.AddScoped<ModelessModuleView>();
        builder.Services.AddScoped<ModelessModuleViewModel>();
        builder.Services.AddSingleton<IMessenger>(StrongReferenceMessenger.Default);
        builder.Services.AddSingleton<ElementMetadataExtractionService>();

        _host = builder.Build();
        await _host.StartAsync();
    }

    /// <summary>
    ///     Stops the host and its <see cref="IHostedService" /> services.
    /// </summary>
    /// <returns>A task that represents the asynchronous host shutdown operation.</returns>
    public static async Task StopAsync()
    {
        if (_host is null) return;

        await _host.StopAsync();
    }

    /// <summary>
    ///     Gets the service of the specified type from the add-in service provider.
    /// </summary>
    /// <typeparam name="T">The type of service to resolve.</typeparam>
    /// <returns>The service instance.</returns>
    /// <exception cref="System.InvalidOperationException">No service of type <typeparamref name="T" /> is registered.</exception>
    /// <remarks>Service resolution requires a started host.</remarks>
    public static T GetService<T>() where T : class
    {
        return _host!.Services.GetRequiredService<T>();
    }

    /// <summary>
    ///     Creates a <see cref="FrameworkElement" /> in a new service scope.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="FrameworkElement" /> to create.</typeparam>
    /// <returns>The created element.</returns>
    /// <exception cref="System.InvalidOperationException">No service of type <typeparamref name="T" /> is registered.</exception>
    /// <remarks>The scope is disposed when the element is unloaded, or when the <see cref="Window" /> is closed.</remarks>
    public static T CreateScope<T>() where T : FrameworkElement
    {
        var scopeFactory = _host!.Services.GetRequiredService<IServiceScopeFactory>();
        var scope = scopeFactory.CreateScope();

        var element = scope.ServiceProvider.GetRequiredService<T>();

        if (element is Window window)
        {
            window.Closed += (_, _) => scope.Dispose();
        }
        else
        {
            element.Unloaded += (_, _) => scope.Dispose();
        }

        return element;
    }
}
