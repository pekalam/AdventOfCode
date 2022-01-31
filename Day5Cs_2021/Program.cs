
(Point p1, Point p2)[] ptsPairs = GetPairsOfPoints();

Solve(ptsPairs, true); //part1
Solve(ptsPairs, false); //part2


static (Point p1, Point p2)[] GetPairsOfPoints()
{
    var ptsEnum = File.ReadLines("input5_2021.txt")
        .Select(s => s.Split("->", StringSplitOptions.TrimEntries))
        .SelectMany(spl => spl.SelectMany(s => s.Split(',')))
        .Select(int.Parse);

    var ptsEnumx = ptsEnum.Where((n, i) => i % 2 == 0).ToArray();
    var ptsEnumy = ptsEnum.Where((n, i) => i % 2 != 0).ToArray();

    var pts = ptsEnumx.Zip(ptsEnumy, (x, y) => new Point(x, y)).ToArray();

    var ptsPair1 = pts.Where((n, i) => i % 2 == 0);
    var ptsPair2 = pts.Where((n, i) => i % 2 != 0);

    var ptsPairs = ptsPair1.Zip(ptsPair2, (p1, p2) => (p1, p2)).ToArray();
    return ptsPairs;
}

static void Solve((Point p1, Point p2)[] ptsPairs, bool ignoreDiag)
{
    var map = new Dictionary<Point, int>(ptsPairs.Length * 2);

    for (int i = 0; i < ptsPairs.Length; i++)
    {
        var (p1, p2) = ptsPairs[i];

        if (p1.x - p2.x != 0 && p1.y - p2.y != 0) //diag
        {
            if (ignoreDiag) continue;
            map[p1] = map.ContainsKey(p1) ? map[p1] + 1 : 1;

            while (p1.x - p2.x != 0)
            {
                p1.x += Math.Sign(p2.x - p1.x);
                p1.y += Math.Sign(p2.y - p1.y);
                map[p1] = map.ContainsKey(p1) ? map[p1] + 1 : 1;
            }
        }
        else
        {
            map[p1] = map.ContainsKey(p1) ? map[p1] + 1 : 1;

            while (p1.x - p2.x != 0)
            {
                p1.x += Math.Sign(p2.x - p1.x);
                map[p1] = map.ContainsKey(p1) ? map[p1] + 1 : 1;
            }
            while (p1.y - p2.y != 0)
            {
                p1.y += Math.Sign(p2.y - p1.y);
                map[p1] = map.ContainsKey(p1) ? map[p1] + 1 : 1;
            }
        }
    }

    Console.WriteLine(map.Values.Where(v => v >= 2).Count());
}

record struct Point(int x, int y);