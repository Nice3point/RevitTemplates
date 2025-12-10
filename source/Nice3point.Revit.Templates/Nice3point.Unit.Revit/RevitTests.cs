using Nice3point.TUnit.Revit;
using Nice3point.TUnit.Revit.Executors;
using TUnit.Core.Executors;

namespace Nice3point.Unit.Revit;

public sealed class RevitTests : RevitApiTest
{
    private static Document _documentFile = null!;

    [Before(Class)]
    [HookExecutor<RevitThreadExecutor>]
    public static void Setup()
    {
        _documentFile = Application.OpenDocumentFile("");
    }

    [After(Class)]
    [HookExecutor<RevitThreadExecutor>]
    public static void Cleanup()
    {
        _documentFile.Close(false);
    }

    [Test]
    [TestExecutor<RevitThreadExecutor>]
    public async Task RevitTest()
    {
        // Arrange
        
        // Act
        
        // Assert
        // await Assert.That();
    }
}