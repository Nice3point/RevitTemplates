using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RevitAddIn.ServiceDefaults.Serialization;

/// <summary>
///     Provides extension methods for <see cref="IHostApplicationBuilder" /> to configure JSON serialization.
/// </summary>
[PublicAPI]
public static class SerializerRegistration
{
    /// <typeparam name="TBuilder">The host application builder type.</typeparam>
    /// <param name="builder">The host application builder.</param>
    extension<TBuilder>(TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        /// <summary>
        ///     Configures the JSON serializer options the application serializes through.
        /// </summary>
        /// <returns>The <typeparamref name="TBuilder" /> for chaining.</returns>
        /// <remarks>The options reach a consumer through <see cref="Microsoft.Extensions.Options.IOptions{TOptions}" />.</remarks>
        public TBuilder ConfigureJsonSerializer()
        {
            builder.Services.Configure<JsonSerializerOptions>(options =>
            {
                options.WriteIndented = builder.Environment.IsDevelopment();
                options.PropertyNameCaseInsensitive = true;
                options.DefaultIgnoreCondition = builder.Environment.IsDevelopment() ? JsonIgnoreCondition.Never : JsonIgnoreCondition.WhenWritingNull;
                options.Converters.Add(new JsonStringEnumConverter());
                options.TypeInfoResolverChain.Add(ApplicationSerializerContext.Default);
            });

            return builder;
        }
    }
}
