using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

const int PreambleLength = 25;
var nums = File.ReadLines("input9.txt")
    .Select(ulong.Parse).ToArray();

var firstWithoutProperty = nums
    .Skip(PreambleLength)
    .Select((i, ind) => new {Prev=nums.Skip(ind).Take(PreambleLength).ToArray(), Current=i})
    .First(arg => 
        (from n1 in arg.Prev from n2 in arg.Prev select new { n1, n2 })
        .All(cartEl => cartEl.n1 + cartEl.n2 != arg.Current));

Console.WriteLine(firstWithoutProperty.Current);


for (int i = 2; i < nums.Length; i++)
{
    if (nums[i - 1] == firstWithoutProperty.Current)
    {
        break;
    }

    for (int j = 0; j <= nums.Length - i; j++)
    {
        var list = nums.AsSpan(j, i).ToArray();
        if (list.Sum() == firstWithoutProperty.Current)
        {
            Console.WriteLine(list.Max() + list.Min());
            break;
        }
    }
}


public static class LinqExt
{
    public static ulong Sum(this IEnumerable<ulong> x)
    {
        ulong sum = 0;
        foreach (var el in x)
        {
            sum += el;
        }
        return sum;
    }
}