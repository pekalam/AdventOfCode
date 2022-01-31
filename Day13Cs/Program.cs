using System;
using System.IO;
using System.Linq;


var lines = File.ReadAllLines("input13.txt");
var (timestamp, ids) = (Convert.ToInt32(lines[0]), lines[1].Split(',').Select(s => Convert.ToInt32(s == "x" ? -1 : s)).ToArray());
var timestampDep = Enumerable.Range(timestamp, int.MaxValue - timestamp + 1)
    .First(i => ids.Where(id => id != -1).Any(id => i % id == 0));
var idDep = ids.Where(id => id != -1).First(i => timestampDep % i == 0);
Console.WriteLine(idDep * (timestampDep - timestamp));



