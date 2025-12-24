#if (_documentation)
// https://github.com/dotnet/sdk/blob/main/template_feed/Microsoft.DotNet.Common.ItemTemplates/content/Class-CSharp/.template.config/template.json
#endif
using Nice3point.TUnit.Revit;
using Nice3point.TUnit.Revit.Executors;
using TUnit.Core.Executors;
#if (ImplicitUsings != "enable")
using Autodesk.Revit.DB;
#endif

#if (csharpFeature_FileScopedNamespaces)
namespace Tests;

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
#else
namespace Tests
{
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
}
#endif
