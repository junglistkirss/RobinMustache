// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using Robin.Benchmarks;

var summaryEvaluator = BenchmarkRunner.Run<EvaluatorBenchmarks>();
var summaryTweets = BenchmarkRunner.Run<TweetsBencnmarks>();