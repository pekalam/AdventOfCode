using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Day17SrcGen
{
    [Generator]
    public class HelloWorldGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG
            //if (!Debugger.IsAttached)
            //{
            //    Debugger.Launch();
            //}
#endif 
        }


        private void AddCartesian(int n, StringBuilder builder)
        {
            builder.Append(
                @"
public static IEnumerable<(");
            builder.Append(Enumerable.Range(0, n).Select(i => "int " + (char) ('a' + i)).Aggregate((acc, x) => acc + "," + x).TrimStart(','));
            builder.Append($@"
)> Cartesian{n}N(int start, int count)
{{
    return
");

            builder.Append(Enumerable.Range(0, n).Reverse().Select(i => (char)('a' + i)).Select(i => $"from {i} in Enumerable.Range(start, count)\n").Aggregate((s, s1) => s+s1));
            builder.Append(Enumerable.Range(0, n).Select(i => (char) ('a' + i))
                .Aggregate("select (", (acc, c) => acc + c + ",").TrimEnd(','));
            builder.Append(@") ;
}");
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var sourceBuilder = new StringBuilder(@"

using System;
using System.Linq;
using System.Collections.Generic;

namespace Day17SrcGen
{
    public static class EnumerableUtils
    {
");

            for (int i = 2; i <= (int)('z'-'a'); i++)
            {
                AddCartesian(i, sourceBuilder);
            }


            sourceBuilder.Append(@"
    }
}");





            var code = sourceBuilder.ToString();
            context.AddSource("enumerableUtilsGenerator", SourceText.From(code, Encoding.UTF8));
        }
    }
}
