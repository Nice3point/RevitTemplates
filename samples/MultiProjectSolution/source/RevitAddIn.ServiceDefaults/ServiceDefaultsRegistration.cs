using RevitAddIn.ServiceDefaults.Logging;
using RevitAddIn.ServiceDefaults.Serialization;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting;

/// <summary>
///     Provides extension methods for <see cref="IHostApplicationBuilder" /> to apply the defaults every project of the add-in shares.
/// </summary>
[PublicAPI]
public static class ServiceDefaultsRegistration
{
    /// <typeparam name="TBuilder">The host application builder type.</typeparam>
    /// <param name="builder">The host application builder.</param>
    extension<TBuilder>(TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        /// <summary>
        ///     Adds the shared logging, diagnostics, and serialization defaults to the specified <see cref="IHostApplicationBuilder" />.
        /// </summary>
        /// <returns>The <typeparamref name="TBuilder" /> for chaining.</returns>
        public TBuilder AddServiceDefaults()
        {
            builder.AddLoggingDefaults();
            builder.ConfigureJsonSerializer();

            return builder;
        }
    }
}
