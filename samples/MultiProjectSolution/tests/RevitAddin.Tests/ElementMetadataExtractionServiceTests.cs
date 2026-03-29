using ModelessModule.Services;
using RevitAddin.Tests.Abstractions;
using RevitAddin.Tests.DataSources;

namespace RevitAddin.Tests;

[DependencyInjectionDataSource]
public sealed class ElementMetadataExtractionServiceTests(ElementMetadataExtractionService extractionService) : RevitFamilySampleTest
{
    [Test]
    public async Task ExtractMetadata_WhenElementIsNull_ReturnsNull()
    {
        var result = extractionService.ExtractMetadata(null);

        await Assert.That(result).IsNull();
    }

    [Test]
    [MethodDataSource(nameof(RevitFamilies))]
    public async Task ExtractMetadata_WithValidElements_ReturnsNonNullResult(string path)
    {
        // Arrange
        var document = FamilyDocuments[path];
        var element = document.CollectElements()
            .Instances()
            .First();

        // Act
        var result = extractionService.ExtractMetadata(element);

        // Assert
        await Assert.That(result).IsNotNull();
    }

    [Test]
    [MethodDataSource(nameof(RevitFamilies))]
    public async Task ExtractMetadata_ElementsWithNullCategory_ReturnsEmptyCategoryName(string path)
    {
        // Arrange
        var document = FamilyDocuments[path];

        var elementWithoutCategory = document.CollectElements()
            .Instances()
            .First(element => element.Category is null);

        // Act
        var result = extractionService.ExtractMetadata(elementWithoutCategory);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.CategoryName).IsEqualTo(string.Empty);
    }

    [Test]
    [MethodDataSource(nameof(RevitFamilies))]
    public async Task ExtractMetadata_ElementsWithCategory_ReturnsCategoryName(string path)
    {
        // Arrange
        var document = FamilyDocuments[path];
        var elementWithCategory = document.CollectElements()
            .Instances()
            .FirstOrDefault(element => element.Category is not null);

        // Act
        var result = extractionService.ExtractMetadata(elementWithCategory);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.CategoryName).IsNotEmpty();
    }
}