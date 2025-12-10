using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Nice3point.BenchmarkDotNet.Revit;
using Nice3point.Benchmark.Revit.Benchmarks;

var configuration = ManualConfig.Create(DefaultConfig.Instance)
    .AddJob(Job.Default.WithCurrentConfiguration());

BenchmarkRunner.Run<RevitBenchmarks>(configuration);