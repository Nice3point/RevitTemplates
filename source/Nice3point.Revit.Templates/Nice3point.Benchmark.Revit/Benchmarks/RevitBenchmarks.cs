using BenchmarkDotNet.Attributes;
using Nice3point.BenchmarkDotNet.Revit;

namespace Nice3point.Benchmark.Revit._1.Benchmarks;

public class RevitBenchmarks : RevitApiBenchmark
{
    private Document? _document;

    protected sealed override void OnGlobalSetup()
    {
        _document = Application.NewProjectDocument(UnitSystem.Metric);
        
        using var transaction = new Transaction(_document, "Seed model");
        transaction.Start();
        
        transaction.Commit();
    }
    
    protected sealed override void OnGlobalCleanup()
    {
        _documentFile.Close(false);
    }

    [Benchmark]
    public void Benchmark()
    {
    }
}