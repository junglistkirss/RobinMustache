// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using Robin.Benchmarks;

var summary = BenchmarkRunner.Run<TweetsBencnmarks>();