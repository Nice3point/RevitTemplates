using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ModularPipelines.Attributes;

namespace Build.Wix;

/// <summary>
///     Provides the registration of the WiX Toolset commands in the pipeline.
/// </summary>
public static class WixRegistration
{
    /// <summary>
    ///     Adds the WiX Toolset commands to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection" /> for chaining.</returns>
    /// <remarks>
    ///     ModularPipelines calls the method when it builds the pipeline.
    ///     The method name defines the <c>context.Tools.Wix</c> property that returns <see cref="IWix" />.
    /// </remarks>
    [ModularPipelinesIntegration]
    public static IServiceCollection RegisterWixContext(IServiceCollection services)
    {
        services.TryAddScoped<IWix, WixToolset>();
        return services;
    }
}
