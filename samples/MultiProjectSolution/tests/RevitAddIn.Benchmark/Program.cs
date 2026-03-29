using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Nice3point.BenchmarkDotNet.Revit;
using RevitAddIn.Benchmark.Benchmarks;

var configuration = ManualConfig.Create(DefaultConfig.Instance)
    .AddJob(Job.Dry.WithCurrentConfiguration())
    .AddDiagnoser(MemoryDiagnoser.Default);

BenchmarkRunner.Run<VolumeCalculationBenchmarks>(configuration);