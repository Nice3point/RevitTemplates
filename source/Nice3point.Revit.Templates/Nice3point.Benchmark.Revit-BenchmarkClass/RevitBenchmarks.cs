#if (_documentation)
// https://github.com/dotnet/sdk/blob/main/template_feed/Microsoft.DotNet.Common.ItemTemplates/content/Class-CSharp/.template.config/template.json
#endif
using BenchmarkDotNet.Attributes;
using Nice3point.BenchmarkDotNet.Revit;
#if (ImplicitUsings != "enable")
using Autodesk.Revit.DB;
#endif

#if (csharpFeature_FileScopedNamespaces)
namespace Benchmarks;

[MemoryDiagnoser]
public class RevitBenchmarks : RevitApiBenchmark
{
    private Document _documentFile = null!;

    protected sealed override void OnSetup()
    {
        _documentFile = Application.OpenDocumentFile("");
    }
    
    protected sealed override void OnCleanup()
    {
        _documentFile.Close(false);
    }

    [Benchmark]
    public void Benchmark()
    {
    }
}
#else
namespace Benchmarks
{
    [MemoryDiagnoser]
    public class RevitBenchmarks : RevitApiBenchmark
    {
        private Document _documentFile = null!;

        protected sealed override void OnSetup()
        {
            _documentFile = Application.OpenDocumentFile("");
        }
        
        protected sealed override void OnCleanup()
        {
            _documentFile.Close(false);
        }

        [Benchmark]
        public void Benchmark()
        {
        }
    }
}
#endif
