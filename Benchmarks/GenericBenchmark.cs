using System;
using System.Collections.Generic;
using System.IO;
using BenchmarkDotNet.Attributes;

public class GenericBenchmark
{
    private Action _invokeMain;

    [ParamsSource(nameof(BenchmarkedProjectNames))]
    public string BenchmarkedProjectName { get; set; }

    [ParamsSource(nameof(BenchmarkedDlls))]
    public string BenchmarkedDll { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        Console.SetOut(new StreamWriter(MemoryStream.Null));
        _invokeMain = ProjectRunner.GetMain(BenchmarkedProjectName, BenchmarkedDll);
    }

    [Benchmark]
    public void Bench() => _invokeMain();

    public static List<string> BenchmarkedDlls { get; } = new();
    public static List<string> BenchmarkedProjectNames { get; } = new();
}
