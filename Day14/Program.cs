using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;


var memory = new Dictionary<long, long>();
long orMask = 0, andMask = 0;
string maskStr = null;
foreach (var line in File.ReadLines("input14.txt"))
{
    if (line.StartsWith("mask"))
    {
        (orMask, andMask, maskStr) = ReadMask(line);
    }
    else
    {
        var (ind, val, addrStr) = ReadMemAssignment(line);

        foreach (var addr in GetAllPossibleAddresses(addrStr, maskStr))
        {
            memory[addr] = val; //(val | orMask) & andMask;
        }

        //memory[ind] = (val | orMask) & andMask;
    }
}

BigInteger b = 0;
foreach (var value in memory.Values)
{
    b += value;
}

Console.WriteLine(b);
IEnumerable<IEnumerable<T>> GetVariationsWithRepeatitions<T>(int k, IEnumerable<T> vals)
{
    foreach (var i in vals)
    {
        if (k == 1)
        {
            yield return new[] {i};
        }
        else
        {
            foreach (var rest in GetVariationsWithRepeatitions(k - 1, vals))
            {
                yield return new[] {i}.Concat(rest);
            }
        }
    }
}

IEnumerable<long> GetAllPossibleAddresses(string addrStr, string maskStr)
{
    addrStr = addrStr.PadLeft(maskStr.Length, '0');
    var resultMask = new string(addrStr.Select((c, i) => maskStr[i] == '0' ? c : maskStr[i] == '1' ? '1' : 'X').ToArray());
    var floatingInd = resultMask.Select((c, i) => i).Where(i => resultMask[i] == 'X').ToArray();

    var sb = new StringBuilder(resultMask);

    foreach (var variation in GetVariationsWithRepeatitions(floatingInd.Length, new []{'0', '1'}))
    {
        int i = 0;
        foreach (var c in variation)
        {
            sb[floatingInd[i++]] = c;
        }

        yield return ToDec(sb.ToString());
    }
}

(long ind, long val, string addr) ReadMemAssignment(string line)
{
    var split = line.Split('=').Select(s => s.Trim()).ToArray();
    var addr = split[0].Remove(0, 3).Trim('[', ']');
    var ind = Convert.ToInt64(addr);
    var val = Convert.ToInt64(split[1]);
    return (ind, val, ToBin(Convert.ToInt64(addr)));
}

long ToDec(string s)
{
    long _ToDec(string s, int i, long acc)
    {
        if (s.Length == 0)
        {
            return acc;
        }

        return _ToDec(s[..^1], i + 1, acc + Convert.ToInt64(s[^1] - 48) * (1L << i));
    }

    return _ToDec(s, 0, 0L);
}


string ToBin(long x)
{
    var sb = new StringBuilder();
    do
    {
        sb.Insert(0, x % 2 == 0 ? '0' : '1');
        x = x / 2;
    } while (x != 0);

    return sb.ToString();
}

(long or, long and, string maskStr) ReadMask(string line)
{
    var maskStr = line.Split('=')[^1].Trim();
    var andMask = ToDec(maskStr.Replace('X', '1'));
    var orMask = ToDec(maskStr.Replace('X', '0'));

    return (orMask, andMask, maskStr);
}