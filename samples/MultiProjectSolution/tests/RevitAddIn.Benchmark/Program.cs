using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Nice3point.BenchmarkDotNet.Revit;
using RevitAddIn.Benchmark.Benchmarks;

var configuration = ManualConfig.Create(DefaultConfig.Instance)
    .AddJob(Job.Dry.WithCurrentConfiguration());

BenchmarkRunner.Run<VolumeCalculationBenchmarks>(configuration);