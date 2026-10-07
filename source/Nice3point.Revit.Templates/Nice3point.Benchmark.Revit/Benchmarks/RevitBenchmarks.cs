using BenchmarkDotNet.Attributes;
using Nice3point.BenchmarkDotNet.Revit;

namespace Nice3point.Benchmark.Revit._1.Benchmarks;

/// <summary>
///     Provides Revit API benchmarks.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class RevitBenchmarks : RevitApiBenchmark
{
    private Document _document = null!;

    /// <inheritdoc />
    protected sealed override void OnGlobalSetup()
    {
        _document = Application.NewProjectDocument(UnitSystem.Metric);

        using var transaction = new Transaction(_document, "Seed model");
        transaction.Start();

        transaction.Commit();
    }

    /// <inheritdoc />
    protected sealed override void OnGlobalCleanup()
    {
        _document.Close(false);
    }

    [Benchmark]
    public void Benchmark()
    {
    }
}
