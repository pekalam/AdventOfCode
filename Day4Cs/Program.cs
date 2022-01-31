using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


var allowedFields = new[]
{
    "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid",
};

var allowedEcl = new[]
{
    "amb", "blu", "brn", "gry", "grn", "hzl", "oth"
};

bool FieldValidation((string name, string val) field)
{
    int ParseInt(string str, int failVal = -1) => int.TryParse(str, out var x) ? x : failVal;

    return field switch
    {
        (string name, string val) when name == "byr" => ParseInt(val) >= 1920 && ParseInt(val) <= 2002,
        (string name, string val) when name == "iyr" => ParseInt(val) >= 2010 && ParseInt(val) <= 2020,
        (string name, string val) when name == "eyr" => ParseInt(val) >= 2020 && ParseInt(val) <= 2030,
        (string name, string val) when name == "hgt" && val.EndsWith("cm") => ParseInt(val[..^2]) >= 150 && ParseInt(val[..^2]) <= 193,
        (string name, string val) when name == "hgt" && val.EndsWith("in") => ParseInt(val[..^2]) >= 59 && ParseInt(val[..^2]) <= 76,
        (string name, _) when name == "hgt" => false,
        (string name, string val) when name == "hcl" => val.StartsWith("#") && val[1..].All(Char.IsLetterOrDigit),
        (string name, string val) when name == "ecl" => allowedEcl.Contains(val),
        (string name, string val) when name == "pid" => val.Length == 9 && val.All(char.IsDigit),
        ("cid", string val) => true,
    };
}

var blankLine = new string[0];
var totalValid = File
    .ReadLines("input.txt")
    .Select(s => string.IsNullOrWhiteSpace(s) ? blankLine : s.Split(' '))
    .Aggregate(new List<string>(100_000), (acc, strings) =>
    {

        if (strings == blankLine)
        {
            acc.Add("#");
        }
        else
        {
            acc.AddRange(strings
                .Select(s => (name: s.Split(':')[0], val: s.Split(':')[1]))
                .Where(FieldValidation) //part2
                .Select(t => t.name)
            );
        }
        return acc;
    }, list => list.Aggregate(Enumerable.Repeat(0, list.Count + 1).ToList(), (acc, str) =>
    {
        acc[acc[0] + 1] += str != "#" && allowedFields.Contains(str) ? 1 : 0;
        acc[0] += str == "#" ? 1 : 0;
        return acc;
    })).Skip(1).Count(i => i == allowedFields.Length);
Console.WriteLine(totalValid);


