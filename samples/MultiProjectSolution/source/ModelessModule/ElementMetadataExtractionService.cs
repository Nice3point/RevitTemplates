using Microsoft.Extensions.Logging;

namespace ModelessModule;

public sealed class ElementMetadataExtractionService(ILogger<ElementMetadataExtractionService> logger)
{
    public ElementMetadata? ExtractMetadata(Element? element)
    {
        if (element is null)
        {
            return null;
        }

        var elementName = element.Name ?? string.Empty;
        var categoryName = element.Category?.Name ?? string.Empty;

        return new ElementMetadata
        {
            ElementName = elementName,
            CategoryName = categoryName
        };
    }

    public double CalculateVolume(Element? element)
    {
        if (element is null)
        {
            return 0;
        }

        try
        {
            var geometry = element.get_Geometry(new Options { DetailLevel = ViewDetailLevel.Fine });
            if (geometry is null)
            {
                logger.LogWarning("No geometry found for element {ElementId}", element.Id);
                return 0;
            }

            return CalculateGeometryVolume(geometry);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error calculating volume for element {ElementId}", element.Id);
            return 0;
        }
    }

    private double CalculateGeometryVolume(GeometryElement geometryElement)
    {
        var totalVolume = 0d;

        foreach (var geometryObject in geometryElement)
        {
            if (geometryObject is Solid { Volume: > 0 } solid)
            {
                totalVolume += solid.Volume;
            }
            else if (geometryObject is GeometryInstance instance)
            {
                var instanceGeometry = instance.GetInstanceGeometry();
                if (instanceGeometry is not null)
                {
                    totalVolume += CalculateGeometryVolume(instanceGeometry);
                }
            }
            else if (geometryObject is GeometryElement nestedGeometry)
            {
                totalVolume += CalculateGeometryVolume(nestedGeometry);
            }
        }

        return totalVolume;
    }
}