using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var bagDict = new Dictionary<string, List<(int num, string bagName)>>();
(int num, string bagName)[] ContentSplit(string contentStr)
{
    if (contentStr.Trim() == "no other bags.")
    {
        return new (int num, string bagName)[0];
    }

    return contentStr.Split(',', StringSplitOptions.TrimEntries)
        .Select(s => s.TrimEnd('.').Replace("bags", null).Replace("bag", null).TrimEnd())
        .Select(s => (num: Convert.ToInt32(s.Substring(0, s.IndexOf(' '))), bagName: s.Substring(s.IndexOf(' ') + 1)))
        .ToArray();
}

foreach (var line in File.ReadLines("input7.txt"))
{
    var split = line.Split("contain", StringSplitOptions.TrimEntries);
    var (name, content) = (split[0].Replace("bags", null).TrimEnd(), split[1]);
    var contentSplit = ContentSplit(content);

    bagDict[name] = new List<(int num, string bagName)>(contentSplit);
}


//part1
int DfsSearch(string current) => (bagDict[current].Any(tuple => tuple.bagName == "shiny gold") ||
                                  bagDict[current].Any(tuple => DfsSearch(tuple.bagName) > 0))
    ? 1
    : 0;

var total = bagDict.Keys.Where(s => s != "shiny gold").Sum(DfsSearch);
Console.WriteLine(total);

//part2
int DfsSearch2(string current)
{
    int _DfsSearch2(string current)
    {
        return bagDict[current].Count == 0 ? 1 : bagDict[current].Sum(tuple => tuple.num * _DfsSearch2(tuple.bagName)) + 1;
    }
    return _DfsSearch2(current) - 1;
}
var totalPart2 = DfsSearch2("shiny gold");
Console.WriteLine(totalPart2);