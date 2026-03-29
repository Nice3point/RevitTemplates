using ModelessModule.Services;
using Nice3point.TUnit.Revit;
using Nice3point.TUnit.Revit.Executors;
using RevitAddin.Tests.DataSources;
using TUnit.Core.Executors;

namespace RevitAddin.Tests;

[DependencyInjectionDataSource]
public sealed class VolumeCalculationTests(ElementMetadataExtractionService extractionService) : RevitApiTest
{
    private Wall _wall = null!;

    [Before(Test)]
    [HookExecutor<RevitThreadExecutor>]
    public void SeedModel()
    {
        var document = Application.NewProjectDocument(UnitSystem.Metric);

        using var transaction = new Transaction(document, "Seed model");
        transaction.Start();

        var level = Level.Create(document, 0);

        _wall = Wall.Create(document, Line.CreateBound(new XYZ(0, 0, 0), new XYZ(1000, 0, 0)), level.Id, false);
        _wall.FindParameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM)!.Set(1000);

        transaction.Commit();
    }

    [Test]
    public async Task CalculateVolume_WhenElementIsNull_ReturnsZero()
    {
        var result = extractionService.CalculateVolume(null);

        await Assert.That(result).IsEqualTo(0);
    }

    [Test]
    public async Task CalculateVolume_ElementsWithGeometry_ReturnsVolume()
    {
        var result = extractionService.CalculateVolume(_wall);

        await Assert.That(result).IsGreaterThan(0);
    }
}