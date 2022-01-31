using static System.Console;

var data = File.ReadLines("input1_2021.txt")
    .Select(int.Parse);

WriteLine(GetInc(data));
WriteLine(GetInc(SlidingWindows(data)));

int GetInc(IEnumerable<int> data)
{
    return data.Aggregate((prev: (int?)null, inc: 0),
        (acc, curr) => !acc.prev.HasValue ? (prev: curr, inc: acc.inc) : (prev: curr, inc: curr > acc.prev ? acc.inc + 1 : acc.inc))
        .inc;
}

IEnumerable<int> SlidingWindows(IEnumerable<int> data)
{
    if (data.Count() == 0)
    {
        return data;
    }
    return (new[] { data.Take(3).Sum() }).Concat(SlidingWindows(data.Skip(1)));
}

