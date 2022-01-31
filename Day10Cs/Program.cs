using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var nums = File.ReadLines("input10.txt")
    .Select(Int32.Parse)
    .ToArray();
var chain = nums.OrderBy(_ => _).ToArray();
var diff1 = chain.Skip(1).Where((i, ind) => i - chain[ind] == 1).Count() + (chain[0] - 0 == 1 ? 1 : 0);
var diff3 = chain.Skip(1).Where((i, ind) => i - chain[ind] == 3).Count() + (chain[0] - 0 == 3 ? 1 : 0) + 1;
Console.WriteLine(diff1 * diff3);



var options = chain.Append(chain.Max() + 3).Aggregate(new List<List<int>>{new (){0}}, (acc, num) =>
{
    if (num - acc[^1][^1] <= 2)
    {
        acc[^1].Add(num);
    }
    else
    {
        acc.Add(new List<int> {num});
    }
    return acc;
})
    .Where(list => list.Count > 2).Aggregate((ulong)1,(acc,list) => acc * (ulong)( list.Count switch
{
    3 => 2,
    4 => 4,
    5 => 7
} ));


Console.WriteLine(options);

