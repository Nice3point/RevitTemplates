using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModalModule.ViewModels;
using ModalModule.Views;
using ModelessModule.Services;
using ModelessModule.ViewModels;
using ModelessModule.Views;
using RevitAddIn.Config;
using RevitAddIn.Configuration;

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
            DisableDefaults = true
        });

        //Configuration
        builder.ConfigureJsonSerializer();
        
        //Logging
        builder.Logging.ClearProviders();
        builder.AddSerilogLoggingProvider();

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
        return _host!.Services.GetRequiredService<T>();
    }
    
    /// <summary>
    ///     Creates a FrameworkElement with the scope lifetime and manages the scope lifecycle.
    /// </summary>
    /// <typeparam name="T">The type of FrameworkElement to get.</typeparam>
    /// <returns>A FrameworkElement of type T with managed scope lifecycle.</returns>
    /// <remarks>
    ///     The scope is automatically disposed when the element is unloaded or, 
    ///     in the case of a Window, when it is closed.
    /// </remarks>
    /// <exception cref="System.InvalidOperationException">There is no service of type <typeparamref name="T"/></exception>
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