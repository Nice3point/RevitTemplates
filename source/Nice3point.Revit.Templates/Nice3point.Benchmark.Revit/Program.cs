using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Nice3point.BenchmarkDotNet.Revit;

var configuration = ManualConfig.Create(DefaultConfig.Instance)
    .AddJob(Job.Default.WithCurrentConfiguration())
    .AddDiagnoser(MemoryDiagnoser.Default);

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly)
    .Run(args, configuration);
