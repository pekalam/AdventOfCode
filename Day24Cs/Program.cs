using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// is black = false
var tiles = new Dictionary<Coords, Tile>(1_000);

var reference = new Coords(0, 0);
tiles.Add(reference, new Tile(false));

foreach (var line in File.ReadLines("input24.txt"))
{
    var next = reference;

    for (int i = 0; i < line.Length; i++)
    {
        var (nx, ny) = line[i] switch
        {
            'e' => Tile.DirMap[Dir.E],
            's' when (i + 1) < line.Length && line[i + 1] == 'e' => Tile.DirMap[Dir.SE],
            's' when (i + 1) < line.Length && line[i + 1] == 'w' => Tile.DirMap[Dir.SW],
            'w' => Tile.DirMap[Dir.W],
            'n' when (i + 1) < line.Length && line[i + 1] == 'w' => Tile.DirMap[Dir.NW],
            'n' when (i + 1) < line.Length && line[i + 1] == 'e' => Tile.DirMap[Dir.NE],
        };
        if(ny != 0)
        {
            i++;
        }

        next = next with { x = next.x+nx, y = next.y+ny };
    }

    if (tiles.ContainsKey(next))
    {
        tiles[next].Flip();
    }
    else
    {
        tiles.Add(next, new Tile(true)); 
    }
}

Console.WriteLine(tiles.Values.Count(x => x.Black));
Console.WriteLine();

var blackCoords = tiles.Where(t => t.Value.Black).Select(t => t.Key);
List<Coords> newBlackCoords = new(1000);

for (int i = 0; i < 100; i++)
{
    newBlackCoords.Clear();
    var blackCoordsWithNeighbours = blackCoords.SelectMany(c => Tile.DirMap.Values.Select(d => c + d)).Distinct();
    foreach (var coords in blackCoordsWithNeighbours)
    {
        var blackNeighbours = blackCoords.Intersect(Tile.DirMap.Values.Select(d => coords + d)).Count();

        if (blackCoords.Contains(coords))
        {
            if(!(blackNeighbours == 0 || blackNeighbours > 2))
            {
                newBlackCoords.Add(coords);
            }
        }
        else
        {
            if(blackNeighbours == 2)
            {
                newBlackCoords.Add(coords);
            }
        }
    }
    blackCoords = newBlackCoords.ToArray();
    Console.WriteLine(newBlackCoords.Count);
}
Console.WriteLine("finished");


record Coords(int x, int y)
{
    public static Coords operator+(Coords c1, Coords c2)
    {
        return c1 with { x=c1.x+c2.x, y=c1.y+c2.y };
    }
}

class Tile
{
    public bool Black;

    public Tile(bool black)
    {
        Black = black;
    }

    public static Dictionary<Dir, Coords> DirMap { get; } = new()
    {
        { Dir.E, new Coords(2,0) },
        { Dir.SE, new Coords(1,-2) },
        { Dir.SW, new Coords(-1,-2) },
        { Dir.W, new Coords(-2, 0) },
        { Dir.NW, new Coords(-1,2) },
        { Dir.NE, new Coords(1,2) },
    };
    
    //prevent beforefieldinit
    static Tile() { }

    public void Flip() => Black = !Black;
}

enum Dir
{
    E, SE, SW, W, NW, NE
}


