using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RevitAddIn.Serialization;

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
        /// <remarks><see cref="ModalModule.ViewModels.ModalModuleViewModel" /> shows how the options reach a consumer.</remarks>
        public TBuilder ConfigureJsonSerializer()
        {
            builder.Services.Configure<JsonSerializerOptions>(options =>
            {
                options.WriteIndented = builder.Environment.IsDevelopment();
                options.PropertyNameCaseInsensitive = true;
                options.DefaultIgnoreCondition = builder.Environment.IsDevelopment() ? JsonIgnoreCondition.Never : JsonIgnoreCondition.WhenWritingNull;
                options.Converters.Add(new JsonStringEnumConverter());
            });

            return builder;
        }
    }
}
