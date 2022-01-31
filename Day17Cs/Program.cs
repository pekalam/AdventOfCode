using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary>
/// Highly inefficient and unoptimized version. It serves purpose for experimentation with source generators and cache-aware code.
/// </summary>

const int TotalCycles = 6;
var lines = File.ReadAllLines("input17.txt").Select(s => s.Length % 2 == 0 ? s + '.' : s).ToArray();
if (lines.Length % 2 == 0)
{
    lines = lines.Append(string.Concat(lines[0].Select(c => '.'))).ToArray();
}

var txtGrid = lines.SelectMany(s => s.ToCharArray()).ToArray();
CountActiveCubes3d();
CountActiveCubes4d();

void FillGrid(Tensor<char> grid)
{
    for (int i = 0; i < txtGrid.Length; i++)
    {
        var ind = new[] {i % lines[0].Length - grid.Sz / 2, i / lines[0].Length - grid.Sz / 2}
            .Concat(Enumerable.Repeat(0, grid.Dims - 2)).ToArray();
        grid.At(ind) =
            txtGrid[i];
    }
}

void CountActiveCubes3d()
{
    var grid = new Tensor<char>(lines.Length + 2, 3, '.');
    FillGrid(grid);

    for (int i = 0; i < TotalCycles; i++)
    {
        var cartesianProduct = Day17SrcGen.EnumerableUtils.Cartesian3N(-grid.Sz / 2, grid.Sz);
        var toInvert = new List<(int x, int y, int z)>();

        var sz = grid.Sz;

        foreach (var p in cartesianProduct)
        {
            var active = grid.IsActive(p);
            var activeNeighbors = grid.ActiveNeighbors(p);

            if (active && activeNeighbors != 2 && activeNeighbors != 3)
            {
                toInvert.Add(p);
            }

            if (!active && activeNeighbors == 3)
            {
                toInvert.Add(p);
            }

            if (grid.Sz == sz)
            {
                grid.ExpandIfOnEdge(p.a, p.b, p.c);
            }
        }

        foreach (var p in toInvert.Distinct())
        {
            grid.At(p.x, p.y, p.z) = grid.At(p.x, p.y, p.z) == '.' ? '#' : '.';
        }
    }

    Console.WriteLine(grid.Storage.Count(c => c == '#'));
}

void CountActiveCubes4d()
{
    var grid = new Tensor<char>(lines.Length+2, 4, '.');
    FillGrid(grid);

    for (int i = 0; i < TotalCycles; i++)
    {
        var cartesianProduct = Day17SrcGen.EnumerableUtils.Cartesian4N(-grid.Sz / 2, grid.Sz);
        var toInvert = new List<(int x, int y, int z, int w)>();

        var sz = grid.Sz;

        foreach (var p in cartesianProduct)
        {
            var active = grid.IsActive(p);
            var activeNeighbors = grid.ActiveNeighbors(p);

            if (active && activeNeighbors != 2 && activeNeighbors != 3)
            {
                toInvert.Add(p);
            }

            if (!active && activeNeighbors == 3)
            {
                toInvert.Add(p);
            }

            if (grid.Sz == sz)
            {
                grid.ExpandIfOnEdge(p.a, p.b, p.c, p.d);
            }
        }

        foreach (var p in toInvert.Distinct())
        {
            grid.At(p.x, p.y, p.z, p.w) = grid.At(p.x, p.y, p.z, p.w) == '.' ? '#' : '.';
        }
    }

    Console.WriteLine(grid.Storage.Count(c => c == '#'));
}


static class GridExtensions
{
    public static int ActiveNeighbors(this Tensor<char> map, (int x, int y, int z) p)
    {
        var total = (
            from zi in Enumerable.Range(-1, 3)
            from yi in Enumerable.Range(-1, 3)
            from xi in Enumerable.Range(-1, 3)
            where (xi != 0 || yi != 0 || zi != 0)
                  && map.InRange(p.x + xi, p.y + yi, p.z + zi) &&
                  map.At(p.x + xi, p.y + yi, p.z + zi) == '#'
            select 1
        ).Sum();

        return total;
    }

    public static bool IsActive(this Tensor<char> map, (int x, int y, int z) p)
    {
        return map.InRange(p.x, p.y, p.z) && map.At(p.x, p.y, p.z) == '#';
    }

    public static int ActiveNeighbors(this Tensor<char> map, (int x, int y, int z, int w) p)
    {
        var total = (
            from wi in Enumerable.Range(-1, 3)
            from zi in Enumerable.Range(-1, 3)
            from yi in Enumerable.Range(-1, 3)
            from xi in Enumerable.Range(-1, 3)
            where (xi != 0 || yi != 0 || zi != 0 || wi != 0)
                  && map.InRange(p.x + xi, p.y + yi, p.z + zi, p.w + wi) &&
                  map.At(p.x + xi, p.y + yi, p.z + zi, p.w + wi) == '#'
            select 1
        ).Sum();

        return total;
    }

    public static bool IsActive(this Tensor<char> map, (int x, int y, int z, int w) p)
    {
        return map.InRange(p.x, p.y, p.z, p.w) && map.At(p.x, p.y, p.z, p.w) == '#';
    }
}