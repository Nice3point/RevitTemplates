using Nice3point.Revit.ServiceDefaults._1.Logging;

#if (diContainer)
// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;
#elseif (diHosting)
// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting;
#endif

/// <summary>
#if (diContainer)
///     Provides extension methods for <see cref="IServiceCollection" /> to apply the defaults every project of the add-in shares.
#elseif (diHosting)
///     Provides extension methods for <see cref="IHostApplicationBuilder" /> to apply the defaults every project of the add-in shares.
#endif
/// </summary>
/// <remarks>The registration lives in the namespace of the type it extends. A project referencing this one calls it without an extra using directive.</remarks>
[PublicAPI]
public static class ServiceDefaultsRegistration
{
#if (diContainer)
    /// <param name="services">The <see cref="IServiceCollection" /> to add the services to.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Adds the shared logging and diagnostics defaults to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <returns>The <see cref="IServiceCollection" /> for chaining.</returns>
        public IServiceCollection AddServiceDefaults()
        {
            services.AddLoggingDefaults();

            return services;
        }
    }
#elseif (diHosting)
    /// <typeparam name="TBuilder">The host application builder type.</typeparam>
    /// <param name="builder">The host application builder.</param>
    extension<TBuilder>(TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        /// <summary>
        ///     Adds the shared logging and diagnostics defaults to the specified <see cref="IHostApplicationBuilder" />.
        /// </summary>
        /// <returns>The <typeparamref name="TBuilder" /> for chaining.</returns>
        public TBuilder AddServiceDefaults()
        {
            builder.AddLoggingDefaults();

            return builder;
        }
    }
#endif
}
