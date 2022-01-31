//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;

//var root = new Tile();
//int black = 0;
//foreach (var line in File.ReadLines("input.txt"))
//{
//    Tile? next = null;
//    Tile? reference = root; 
//    Dir d = default;
//    for (int i = 0; i < line.Length; i++)
//    {
//        d = line[i] switch
//        {
//            'e' => Dir.E,
//            's' when (i+1) < line.Length && line[i+1] == 'e' => Dir.SE,
//            's' when (i+1) < line.Length && line[i+1] == 'w' => Dir.SW,
//            'w' => Dir.W,
//            'n' when (i + 1) < line.Length && line[i + 1] == 'w' => Dir.NW,
//            'n' when (i + 1) < line.Length && line[i + 1] == 'e' => Dir.NE,
//        };

//        next = reference[d];
//        if(next == null)
//        {
//            next = reference[d] = new Tile();
//            next[d.C()] = reference;
//        }
//        reference = next;
//    }
//    if (next.Black)
//    {
//        black--;
//    }
//    else
//    {
//        black++;
//    }
//    next.Flip();
//}


//Console.WriteLine(CountBlackDfs(root));

//int CountBlackDfs(Tile root)
//{
//    var toVisit = new Stack<Tile>();
//    var visited = new HashSet<Tile>();
//    toVisit.Push(root);

//    while(toVisit.Count > 0)
//    {
//        var current = toVisit.Pop();
//        if (visited.Contains(current))
//        {
//            continue;
//        }

//        visited.Add(current);

//        foreach (var n in current.EnumerateNeighbours().Where(static e => e != null))
//        {
//            toVisit.Push(n!);
//        }
//    }

//    return visited.Where(v => v.Black).Count();
//}

//IEnumerable<Tile> DFS(Tile root, Action<Tile>? action = null)
//{
//    var toVisit = new Stack<Tile>();
//    var visited = new HashSet<Tile>();
//    toVisit.Push(root);

//    while (toVisit.Count > 0)
//    {
//        var current = toVisit.Pop();
//        if (visited.Contains(current))
//        {
//            continue;
//        }
//        action?.Invoke(current);
//        visited.Add(current);

//        foreach (var n in current.EnumerateNeighbours().Where(static e => e != null))
//        {
//            toVisit.Push(n!);
//        }
//    }

//    return visited;
//}


//class Tile
//{
//    public Tile? E;
//    public Tile? SE;
//    public Tile? SW;
//    public Tile? W;
//    public Tile? NW;
//    public Tile? NE;
//    public bool Black;

//    public void Flip() => Black = !Black;

//    public IEnumerable<Tile?> EnumerateNeighbours()
//    {
//        yield return E;
//        yield return SE;
//        yield return SW;
//        yield return W;
//        yield return NW;
//        yield return NE;
//    }

//    public Tile? this[Dir dir]
//    {
//        get => dir switch
//        {
//            Dir.E => E,
//            Dir.SE => SE,
//            Dir.SW => SW,
//            Dir.W => W,
//            Dir.NW => NW,
//            Dir.NE => NE,
//        };
//        set 
//        {
//            if (dir == Dir.E) E = value;
//            else if (dir == Dir.SE) SE = value;
//            else if (dir == Dir.SW) SW = value;
//            else if (dir == Dir.W) W = value;
//            else if (dir == Dir.NW) NW = value;
//            else if (dir == Dir.NE) NE = value;
//        }
//    }
//}

//enum Dir
//{
//    E=1,SE,SW,W,NW,NE
//}

//static class DirExtensions
//{
//    public static Dir C(this Dir d)
//    {
//        return d switch
//        {
//            Dir.E => Dir.W,
//            Dir.SE => Dir.NW,
//            Dir.SW => Dir.NE,
//            Dir.W => Dir.E,
//            Dir.NW => Dir.SE,
//            Dir.NE => Dir.SW,
//        };
//    }
//}

//static class DirUtils
//{
//    public static IEnumerable<Dir> EnumerateClockwise(Dir startDir)
//    {
//        int start = (int)startDir;
//        while (true)
//        {
//            yield return (Dir)start;
//            start = start - 1;
//            if(start < 1)
//            {
//                start = 6;
//            }
//        }
//    }
//}




