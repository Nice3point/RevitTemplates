using Nice3point.TUnit.Revit;

namespace Nice3point.Unit.Revit._1;

/// <summary>
///     Provides Revit API tests.
/// </summary>
public sealed class RevitTests : RevitApiTest
{
    private Document _document = null!;

    [Before(Test)]
    public void SeedModel()
    {
        _document = Application.NewProjectDocument(UnitSystem.Metric);

        using var transaction = new Transaction(_document, "Seed model");
        transaction.Start();

        transaction.Commit();
    }

    [After(Test)]
    public void CloseModel()
    {
        _document.Close(false);
    }

    [Test]
    public async Task RevitTestAsync()
    {
        // Arrange

        // Act
        var isValidDocument = _document.IsValidObject;

        // Assert
        await Assert.That(isValidDocument).IsTrue();
    }
}
