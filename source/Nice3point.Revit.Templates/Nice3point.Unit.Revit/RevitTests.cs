using Nice3point.TUnit.Revit;
using Nice3point.TUnit.Revit.Executors;
using TUnit.Core.Executors;

namespace Nice3point.Unit.Revit._1;

public sealed class RevitTests : RevitApiTest
{
    private Document _document = null!;

    [Before(Test)]
    [HookExecutor<RevitThreadExecutor>]
    public void SeedModel()
    {
        _document = Application.NewProjectDocument(UnitSystem.Metric);

        using var transaction = new Transaction(_document, "Seed model");
        transaction.Start();

        transaction.Commit();
    }

    [After(Test)]
    [HookExecutor<RevitThreadExecutor>]
    public void CloseModel()
    {
        _document.Close(false);
    }

    [Test]
    public async Task RevitTest()
    {
        // Arrange
        
        // Act
        
        // Assert
        // await Assert.That();
    }
}