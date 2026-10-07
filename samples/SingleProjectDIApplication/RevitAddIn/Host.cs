using Microsoft.Extensions.DependencyInjection;
using RevitAddIn.Views;
using RevitAddIn.ViewModels;

namespace RevitAddIn;

/// <summary>
///     Provides a host for the application's services and manages their lifetimes.
/// </summary>
public static class Host
{
    private static IServiceProvider? _serviceProvider;

    /// <summary>
    ///     Registers the add-in services and starts the host.
    /// </summary>
    public static void Start()
    {
        var services = new ServiceCollection();

        services.AddTransient<RevitAddInViewModel>();
        services.AddTransient<RevitAddInView>();

        _serviceProvider = services.BuildServiceProvider();
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
        return _serviceProvider!.GetRequiredService<T>();
    }
}
