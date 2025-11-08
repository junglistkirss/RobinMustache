// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using Robin.Benchmarks;


Type[] benchmarks =
[
    typeof(EvaluatorBenchmarks),
    typeof(TweetsBencnmarks)
];

BenchmarkSwitcher.FromTypes(benchmarks).Run(args);