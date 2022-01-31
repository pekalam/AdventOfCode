using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var str = File.ReadAllLines("input19.txt");
var rules = str.TakeWhile(x => x.Length > 0).Select(Rule.FromString).OrderBy(r => r.Index).ToArray();
var toCheck = str.SkipWhile(x => x.Length > 0).Skip(1).ToArray();

//part1
var matching = Generate(rules[0]).Where(x => toCheck.Contains(x)).ToArray();
Console.WriteLine(matching.Length);


IEnumerable<string> Cartesian(IEnumerable<IEnumerable<string>> items)
{
    if(!items.Any())
    {
        return Enumerable.Empty<string>();
    }
    if(items.Count() == 1)
    {
        return items.ElementAt(0);
    }

    return Cartesian(Enumerable.Empty<IEnumerable<string>>().Append(items.ElementAt(0).SelectMany(_ => items.ElementAt(1), (x, y) => x + y)).Concat(items.Skip(2)));
}

IEnumerable<string> Generate(Rule rule)
{
    if (!rule.Char.HasValue)
    {
        return rule.SubRules
            .Select(sub => sub.Select(i => Generate(rules[i])))
            .SelectMany(r => Cartesian(r));
    }
    else
    {
        return Enumerable.Repeat(rule.Char.Value.ToString(), 1);
    }
}

class Rule
{
    public readonly int Index;
    public readonly char? Char;
    public readonly List<int[]>? SubRules;

    private Rule(char @char, int index)
    {
        Char = @char; Index = index;
    }
    private Rule(List<int[]>? subRules, int index)
    {
        SubRules = subRules; Index = index;
    }

    public static Rule FromString(string str)
    {
        var strSplit = str.Split(':');
        var index = int.Parse(strSplit[0]);
        var data = strSplit[1].TrimStart();

        if(data[0] == '\"')
        {
            return new Rule(data[1], index);
        }
        else
        {
            var subRules = new List<int[]>();
            var subRule = new List<int>();
            foreach (var part in data.Split(' '))
            {
                if(part == "|")
                {
                    subRules.Add(subRule.ToArray());
                    subRule.Clear();
                    continue;
                }

                subRule.Add(int.Parse(part));
            }
            if(subRule.Count > 0)
            {
                subRules.Add(subRule.ToArray());
            }

            return new Rule(subRules, index);
        }
    }
}