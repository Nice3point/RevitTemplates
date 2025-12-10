using BenchmarkDotNet.Attributes;

namespace Nice3point.Benchmark.Revit.Benchmarks;

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