using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Nice3point.BenchmarkDotNet.Revit;
using Nice3point.Benchmark.Revit._1.Benchmarks;

var configuration = ManualConfig.Create(DefaultConfig.Instance)
    .AddJob(Job.Default.WithCurrentConfiguration())
    .AddDiagnoser(MemoryDiagnoser.Default);

BenchmarkRunner.Run<RevitBenchmarks>(configuration);