﻿using BenchmarkDotNet.Attributes;
using ChessRealms.ChessEngine.Core.Attacks;
using ChessRealms.ChessEngine.Core.Types;
using ChessRealms.ChessEngine.Parsing;
using System.Runtime.CompilerServices;

namespace ChessRealms.ChessEngine.Benchmark;

public class PerftBenchmarks
{
    private Position position;

    public PerftBenchmarks()
    {
        _ = FenStrings.TryParse(FenStrings.StartPosition, out position);
        AttackLookups.InvokeInit();
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void StartPos_Depth_6()
    {
        Perft.PerftDriver.Test(position, 6, false);
    }
}
