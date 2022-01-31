using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


var count = File.ReadLines("input6.txt")
    .Aggregate(new List<List<string>>() {new()}, (acc, s) =>
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            acc.Add(new List<string>());
            return acc;
        }
        acc[^1].Add(s);
        return acc;
    }, acc => acc)
    .Select(list =>
        // list.Aggregate(new Dictionary<char, int>(), (acc, s) =>
        //     s.Aggregate(acc, (dict, c) =>
        //     {
        //         dict[c] = 1;
        //         return dict;
        //     })
        // ))

        /// part2
        list.Aggregate(new List<Dictionary<char, int>>(), (acc, s) =>
            {
                var dict = s.Aggregate(new Dictionary<char, int>(), (dict, c) =>
                {
                    dict[c] = 1;
                    return dict;
                });
                acc.Add(dict);
                return acc;
            }
        )
        ).Select(list => list.Aggregate(new {Empty=true, Dict=new Dictionary<char, int>()}, (acc, dict) =>
        {
            return acc.Empty ? new {Empty=false, Dict=dict} : new { Empty = false, Dict=acc.Dict.Intersect(dict).ToDictionary(pair => pair.Key, pair => pair.Value)};
        })).Select(arg => arg.Dict)
        // end of part2
    .SelectMany(dict => dict.Values).Sum();
Console.WriteLine(count);