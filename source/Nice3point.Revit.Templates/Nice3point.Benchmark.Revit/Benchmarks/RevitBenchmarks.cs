using BenchmarkDotNet.Attributes;
using Nice3point.BenchmarkDotNet.Revit;

namespace Nice3point.BenchmarkDotNet.Revit.Benchmarks;

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