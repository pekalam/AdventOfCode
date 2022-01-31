using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var str = File.ReadAllLines("input19.txt");

var str8Ind = str.Select((x, i) => (x, i)).First((arg) => arg.x == "8: 42").i;
var str11Ind = str.Select((x, i) => (x, i)).First((arg) => arg.x == "11: 42 31").i;

str[str8Ind] = "8: 42 | 42 8";
str[str11Ind] = "11: 42 31 | 42 11 31";

var rules = ProdRule.FromStringArray(str.TakeWhile(x => x.Length > 0).ToArray());

var toCheck = str.SkipWhile(x => x.Length > 0).Skip(1).ToArray();

Console.WriteLine(toCheck.Select(Cyk).Where(r => r == true).Count());

bool Cyk(string str)
{
    bool[,,] P = new bool[str.Length, str.Length, rules.Count];

    for (int i = 0; i < str.Length; i++)
    {
        for (int j = 0; j < rules.Count; j++)
        {
            if (!rules[j].OnlyTerm)
            {
                continue;
            }

            if (rules[j].Term.Contains(str[i]))
            {
                P[0, i, j] = true;
            }
        }
    }


    for (int l = 2; l <= str.Length; l++)
    {
        for (int s = 1; s <= str.Length - l + 1; s++)
        {
            for (int p = 1; p <= l - 1; p++)
            {
                for (int a = 0; a < rules.Count; a++)
                {
                    if (rules[a].OnlyTerm)
                    {
                        continue;
                    }

                    for (int j = 0; j < rules[a].NonTerm.Count; j++)
                    {
                        var rule = rules[a].NonTerm[j];
                        ref var b = ref rule[0];
                        ref var c = ref rule[1];

                        if (P[p - 1, s - 1, b] && P[l - p - 1, s + p - 1, c])
                        {
                            P[l - 1, s - 1, a] = true;
                        }
                    }

                }
            }
        }
    }


    return P[str.Length - 1, 0, 0];
}


class ProdRule
{
    private List<int[]> _nonTerm;
    private List<char> _term;
    public readonly int Index;

    private ProdRule(int index, List<int[]> nonTerm = null, List<char> term = null)
    {
        Index = index;
        _nonTerm = nonTerm ?? new();
        _term = term ?? new();
    }

    public IReadOnlyList<int[]> NonTerm => _nonTerm;
    public IReadOnlyList<char> Term => _term;
    public bool OnlyTerm => NonTerm.Count == 0;

    public static Dictionary<int,ProdRule> FromStringArray(string[] str)
    {
        var rules = str.Select(FromString).OrderBy(r => r.Index).ToDictionary(r => r.Index);
        var totalRules = rules.Count;
        var toAdd = new List<ProdRule>(rules.Count / 2);
        foreach (var (_, rule) in rules)
        {
            var toNorm = rule.Norm(totalRules);
            totalRules += toNorm.Length;
            toAdd.AddRange(toNorm);
        }

        foreach (var item in toAdd)
        {
            rules.Add(item.Index, item);
        }

        foreach (var (_, rule) in rules)
        {
            rule.ReplaceSingleNonTerm(rules);
        }

        return rules;
    }

    private static ProdRule FromString(string s)
    {
        var strSplit = s.Split(':');
        var index = int.Parse(strSplit[0]);
        var data = strSplit[1].TrimStart();

        if (data[0] == '\"')
        {
            return new ProdRule(index, term: new() { data[1] });
        }
        else
        {
            var allNonTerm = new List<int[]>();
            var nonTerm = new List<int>();
            foreach (var part in data.Split(' '))
            {
                if (part == "|")
                {
                    allNonTerm.Add(nonTerm.ToArray());
                    nonTerm.Clear();
                    continue;
                }
                nonTerm.Add(int.Parse(part));
            }
            if (nonTerm.Count > 0)
            {
                allNonTerm.Add(nonTerm.ToArray());
            }

            return new ProdRule(index, nonTerm: allNonTerm);
        }
    }

    private ProdRule[] Norm(int totalRules)
    {
        if (OnlyTerm)
        {
            return Array.Empty<ProdRule>();
        }

        var newRules = new List<ProdRule>();
        for (int i = 0; i < _nonTerm.Count; i++)
        {
            if (_nonTerm[i].Length == 2)
            {
                continue;
            }

            var toSplit = _nonTerm[i].ToList();
            while (toSplit.Count > 2)
            {
                var newRule = new ProdRule(totalRules + newRules.Count, new List<int[]>() { new int[] { toSplit[0], toSplit[1] } });
                toSplit.RemoveAt(0);
                toSplit.RemoveAt(0);
                toSplit.Insert(0, newRule.Index);
                newRules.Add(newRule);
            }
            _nonTerm[i] = toSplit.ToArray();
        }

        return newRules.ToArray();
    }

    private void ReplaceSingleNonTerm(Dictionary<int,ProdRule> rules)
    {
        var toRemove = new Lazy<List<int[]>>();
        var toAddTerm = new Lazy<List<char>>();
        var toAddNonTerm = new Lazy<List<int[]>>();
        foreach (var r in NonTerm)
        {
            if (r.Length == 1 && rules[r[0]].OnlyTerm)
            {
                toRemove.Value.Add(r);
                _term.AddRange(rules[r[0]].Term);
            }
            else if (r.Length == 1 && !rules[r[0]].OnlyTerm)
            {
                rules[r[0]].ReplaceSingleNonTerm(rules);

                toRemove.Value.Add(r);
                toAddTerm.Value.AddRange(rules[r[0]].Term);
                toAddNonTerm.Value.AddRange(rules[r[0]].NonTerm);
            }
        }
        if (toRemove.IsValueCreated)
        {
            foreach (var item in toRemove.Value)
            {
                _nonTerm.Remove(item);
            }
        }
        if (toAddTerm.IsValueCreated)
        {
            _term.AddRange(toAddTerm.Value);
        }
        if (toAddNonTerm.IsValueCreated)
        {
            _nonTerm.AddRange(toAddNonTerm.Value);
        }
    }
}
