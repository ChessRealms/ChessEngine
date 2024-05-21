using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using ChessRealms.ChessEngine2.Benchmark.MoveGenerations;

Console.WriteLine("Hello, Benckmarks!");
Console.WriteLine();

var config = DefaultConfig.Instance
    .AddJob(Job
         .MediumRun
         .WithLaunchCount(1)
         .WithToolchain(InProcessEmitToolchain.DontLogOutput));

BenchmarkRunner.Run<PawnMoveGenerationBench>(config);