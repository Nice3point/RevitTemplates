using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ModelessModule;

namespace RevitAddin.Tests.DataSources;

public sealed class DependencyInjectionDataSourceAttribute : DependencyInjectionDataSourceAttribute<IServiceScope>
{
    private static readonly IServiceProvider ServiceProvider = CreateSharedServiceProvider();

    public override IServiceScope CreateScope(DataGeneratorMetadata dataGeneratorMetadata)
    {
        return ServiceProvider.CreateScope();
    }

    public override object? Create(IServiceScope scope, Type type)
    {
        return scope.ServiceProvider.GetService(type);
    }

    private static IServiceProvider CreateSharedServiceProvider()
    {
        return new ServiceCollection()
            .AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Debug);
            })
            .AddTransient<ElementMetadataExtractionService>()
            .BuildServiceProvider();
    }
}
