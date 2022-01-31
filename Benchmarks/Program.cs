using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Extensions;
using BenchmarkDotNet.Running;
using Spectre.Console;
using System;
using System.IO;
using System.Linq;

public class SolutionBenchmarks
{
    [GlobalSetup]
    public void Setup()
    {
        Console.SetOut(new StreamWriter(MemoryStream.Null));
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        //Console.SetOut(Console.Out);
    }

    [Benchmark]
    public void Day1() => Day1Program.Day1.main(null);
}


public partial class Program
{
    public static void Main(string[] args)
    {
        var projectMetadatas = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
            .Where(s => s.Split(Path.DirectorySeparatorChar)[^1].Contains("Day"))
            .Select(s => new ProjectMetadata(s.Split(Path.DirectorySeparatorChar)[^1].Replace(".dll", string.Empty), s))
            .ToArray();
        var projectDisplayNameToProject = projectMetadatas.ToDictionary(p => p.DisplayName, p => p);

        //// Echo the fruit back to the terminal
        //AnsiConsole.WriteLine($"I agree. {fruit} is tasty!");

        //toLoad.ForEach(path => loadedAssemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path))));

        //var x = loadedAssemblies[9].GetTypes().ToArray();
        //var d1 = x[0];
        //var main = d1.GetMethods().First(m => m.Name == "main");

        //main.Invoke(null, main.GetParameters().Select(_ => (object)null).ToArray());

        var projectName = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("Select project to run")
            .PageSize(20)
            .MoreChoicesText("[grey](Move up and down to reveal more projects)[/]")
            .AddChoices(projectMetadatas.SortAlphabetically().Select(m => m.DisplayName)));
        var selectedProjectMetadata = projectDisplayNameToProject[projectName];

        var runMethods = new[] { "Run main", "Run benchmark (BenchmarkDotNet)" };
#if DEBUG
        var action = runMethods[0];
#else
        var action = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("Select run method")
            .AddChoices("Run main", "Run benchmark (BenchmarkDotNet)"));
#endif


        if (action == runMethods[0])
        {
            ProjectRunner.GetMain(selectedProjectMetadata.Name, selectedProjectMetadata.Dll)();
            return;
        }

        //run(projectName, projectNamesToDll);


        GenericBenchmark.BenchmarkedDlls.Add(projectDisplayNameToProject[projectName].Dll);
        GenericBenchmark.BenchmarkedProjectNames.Add(projectDisplayNameToProject[projectName].Name);

        var b1 = BenchmarkRunner.Run<GenericBenchmark>(
             ManualConfig
                    .Create(DefaultConfig.Instance)

                    .WithOptions(ConfigOptions.DisableOptimizationsValidator));
        var stats = b1.Reports[0].ResultStatistics;


        Console.ReadKey();


        //var b = BenchmarkRunner.Run<SolutionBenchmarks>(
        //     ManualConfig
        //            .Create(DefaultConfig.Instance)
        //            .WithOptions(ConfigOptions.DisableOptimizationsValidator));
        //var stats = b.Reports[0].ResultStatistics;

        Console.ReadKey();

        // Create a table
        var table = new Table();

        // Add some columns
        table.AddColumn(new TableColumn("Project name").LeftAligned());
        table.AddColumn(new TableColumn("Mean").Centered());

        var formatter = stats.CreateNanosecondFormatter(System.Globalization.CultureInfo.InvariantCulture);
        table.AddRow("Day1", $"[green]{formatter(stats.Mean)}[/]");
        AnsiConsole.Write(table);



        Console.ReadKey();
        //HelloFrom("asd");
    }


    static partial void HelloFrom(string name);
}