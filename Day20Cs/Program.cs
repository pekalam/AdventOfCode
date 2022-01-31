using Day20Cs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

var img = new Image("input20.txt");

// part1
var corners = img.FindCorners();
Console.WriteLine(corners.Aggregate(1L, (acc, i) => acc * i.Id));

//part2
var arrangedImg = img.Arrange();

var monster = new string[3]
{
    "                  # ",
    "#    ##    ##    ###",
    " #  #  #  #  #  #   ",
};
var monsterWidth = monster[0].Length;
var monsterHeight = monster.Length;

var patternMatchIndices = monster.Select(r => 
                    r.Select((c, i) => (c, i)).Where(t => t.c == '#').Select(t => t.i).ToArray()
                    ).ToArray();

void AddMonster(char[][] chTransformedImg, int starth, int startv)
{
    for (int i = 0; i < monster.Length; i++)
    {
        for (int j = 0; j < monster[0].Length; j++)
        {
            chTransformedImg[i+startv][j+starth] = monster[i][j] == '#' ? 'o' : chTransformedImg[i+startv][j+starth];
        }
    }
}

foreach (var transformedImg in arrangedImg.EnumeratreTransformations())
{
    var chTransformedImg = transformedImg.Select(s => s.ToCharArray()).ToArray();
    var hasMatch = false;
    var totalMatches = 0;

    for (int starth = 0; starth <= transformedImg[0].Length - monsterWidth; starth++)
    for (int startv = 0; startv <= transformedImg.Length - monsterHeight; startv++)
        {
            var match = patternMatchIndices.Select((indices, i) => (indices, i)).All(t => t.indices.All(j => transformedImg[startv+t.i][starth+j] == '#'));
            hasMatch = match || hasMatch;
            if (match)
            {
                AddMonster(chTransformedImg, starth, startv);
                totalMatches++;
            }

        }
    if (!hasMatch)
    {
        continue;
    }
    foreach (var arr in chTransformedImg)
    {
        Console.WriteLine(string.Join(string.Empty, arr));
    }

    var hashPerMonster = monster.Sum(s => s.Count(c => c == '#'));
    var roughness = transformedImg.Sum(s => s.Count(c => c == '#')) - totalMatches*hashPerMonster;

    Console.WriteLine(roughness);
    break;
}



public static class StringExt
{
    public static string Reverse(this string str) => string.Join(string.Empty, Enumerable.Reverse(str));

    public static string[] Rotate90(this string[] strArray)
    {
        var c = strArray[0].Length;

        var newArr = new string[c];
        for (int i = 0; i < c; i++)
        {
            newArr[i] = string.Join(string.Empty, strArray.Select(s => s[i]).Reverse());
        }
        return newArr;
    }

    public static string[] FlipLr(this string[] strArray)
    {
        return strArray.Select(s => s.Reverse()).ToArray();
    }
}

class ArrangedImage
{
    public string ImgContent { get; }

    public ArrangedImage(string imgContent)
    {
        ImgContent = imgContent;
    }

    public IEnumerable<string[]> EnumeratreTransformations()
    {
        // 0 deg
        var imgContentArr = ImgContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        yield return imgContentArr;
        // 0deg flip
        yield return imgContentArr.FlipLr();
        // 90deg
        imgContentArr = imgContentArr.Rotate90();
        yield return imgContentArr;
        // 90deg flip
        yield return imgContentArr.FlipLr();
        // 180deg
        imgContentArr = imgContentArr.Rotate90();
        yield return imgContentArr;
        // 180deg flip
        yield return imgContentArr.FlipLr();
        // 270deg
        imgContentArr = imgContentArr.Rotate90();
        yield return imgContentArr;
        // 270deg flip
        yield return imgContentArr.FlipLr();
    }
}

class Image
{
    private Tile[] _tiles;
    private Tile[] _corners;
    private readonly int rows;
    private readonly int cols;

    public Image(string filePath)
    {
        _tiles = Input.ReadImagesFromFile(filePath);
        for (int a = 0; a < _tiles.Length; a++)
        {
            for (int b = 0; b < _tiles.Length; b++)
            {
                if (a != b && _tiles[a].IsNeighbour(_tiles[b]))
                {
                    _tiles[a].Neighbours.Add(_tiles[b]);
                    _tiles[b].Neighbours.Add(_tiles[a]);
                }
            }
        }
        rows = (int)Math.Sqrt(_tiles.Length);
        cols = rows;
    }

    public Tile[] FindCorners()
    {
        return _corners = _tiles.Where(i => i.Neighbours.Count == 2).ToArray();
    }

    public ArrangedImage Arrange()
    {
        var start = _corners[0];
        var img = new Tile[rows, cols];
        img[0, 0] = start;

        var possibleStartTransform = new List<int[]>();
        foreach (var t in Tile.EnumerateTransformations())
        {
            var topIntersects = start.Neighbours.Count(n => n.AllTransformedEdges.Where(ne => ne == start.Top(t)).Count() > 0) > 0;
            var leftIntersects = start.Neighbours.Count(n => n.AllTransformedEdges.Where(ne => ne == start.Left(t)).Count() > 0) > 0;
            if (!topIntersects && !leftIntersects)
            {
                possibleStartTransform.Add(t);
            }
        }
        Debug.Assert(possibleStartTransform.Count > 0);

        start.Tran = possibleStartTransform[1];


        void dfs(Tile start, int r, int c, List<Tile> visited)
        {
            var rightFound = c == cols - 1;
            if (c < cols - 1)
                foreach (var n in start.Neighbours)
                {
                    if (visited.Contains(n))
                    {
                        continue;
                    }

                    foreach (var nt in Tile.EnumerateTransformations())
                    {
                        var rightIntersects = start.Right(start.Tran) == n.Left(nt);
                        if (rightIntersects)
                        {
                            rightFound = true;
                            img[r, c + 1] = n;
                            n.Tran = nt;
                            visited.Add(n);
                            dfs(n, r, c + 1, visited);
                            break;
                        }
                    }
                }

            var bottomFound = r == rows - 1;
            if (r < rows - 1)
                foreach (var n in start.Neighbours)
                {
                    if (visited.Contains(n))
                    {
                        continue;
                    }

                    foreach (var nt in Tile.EnumerateTransformations())
                    {
                        var bottomIntersects = start.Bottom(start.Tran) == n.Top(nt);
                        if (bottomIntersects)
                        {
                            bottomFound = true;
                            img[r + 1, c] = n;
                            n.Tran = nt;
                            visited.Add(n);
                            dfs(n, r + 1, c, visited);
                            break;
                        }
                    }
                }
        }


        dfs(start, 0, 0, new List<Tile>() { start });

        for (int i = 0; i < rows; i++)
        {
            var id = "";
            for (int j = 0; j < cols; j++)
            {
                id += img[i, j].Id + " ";
            }
            Console.WriteLine(id);
        }


        var transformedImgContent = new string[rows][][];
        for (int i = 0; i < rows; i++)
        {
            transformedImgContent[i] = new string[cols][];
            for (int j = 0; j < cols; j++)
            {
                transformedImgContent[i][j] = Tile.RemoveBorder(img[i, j].GetTransformedImgContent());
            }
        }

        var str = "";

        for (int i = 0; i < rows; i++)
        {
            for (int r = 0; r < transformedImgContent[i][0].Length; r++)
            {
                var col = "";
                for (int j = 0; j < cols; j++)
                {
                    col += transformedImgContent[i][j][r];
                }
                col += "\n";
                str += col;
            }
            //str += "\n";
        }

        Console.WriteLine(str);
        return new ArrangedImage(str);
    }
}


class Tile
{
    public static readonly int[][] Transformations = new int[][]
    {
       new[] {1,2,3,4}, // 0
       new[] {-1,4,-3,2}, // 0 LR
       new[] {-4,1,-2,3}, // 90
       new[] {4,3,2,1}, // 90 lR
       new[] {-3,-4,-1,-2}, // 180
       new[] {3,-2,1,-4}, // 180 LR
       new[] {2,-3,4,-1}, // 270
       new[] {-2,-1,-4,-3}, // 270 LR
    };

    public static IEnumerable<int[]> EnumerateTransformations()
    {
        foreach (var t in Transformations)
        {
            yield return t;
        }
    }

    public static string[] RemoveBorder(string[] img) => img.Skip(1).SkipLast(1).Select(s => s.Substring(1, s.Length - 2)).ToArray();

    private string[] _imgContent;
    public readonly int Id;
    public readonly string[] AllTransformedEdges;
    /// <summary>
    /// selected transformation
    /// </summary>
    public int[] Tran;

    public HashSet<Tile> Neighbours = new HashSet<Tile>();


    public Tile(int id, string[] imgContent)
    {
        Id = id;
        _imgContent = imgContent;

        var upperRow = imgContent[0];
        var bottomRow = imgContent[^1];
        var leftColumn = string.Join(string.Empty, imgContent.Select(s => s[0]));
        var rightColumn = string.Join(string.Empty, imgContent.Select(s => s[^1]));

        var edges = new[] { upperRow, rightColumn, bottomRow, leftColumn }
            //.Select(s => string.Intern(s))
            .ToArray();

        var reversedEdges = edges.Select(s => s.Reverse())
            //.Select(s => string.Intern(s))
            .ToArray();
        AllTransformedEdges = edges.Concat(reversedEdges).ToArray();
    }

    private string GetEdgeFromTransformationElement(int n)
    {
        return AllTransformedEdges[n < 0 ? -1*n-1+4 : n-1];
    }

    public string[] GetTransformedImgContent()
    {
        Debug.Assert(Tran != null);
        var result = _imgContent;
        for (int i = 0; i < Transformations.Length; i++)
        {
            if (!(i == 0 || i == 1 || i == 3 || i == 5 || i == 7)) result = result.Rotate90();

            if(Tran == Transformations[i])
            {
                if (i == 1 || i == 3 || i == 5 || i == 7)
                {
                    result = result.FlipLr();
                }
                return result;
            }
        }
        throw new ArgumentException();
    }

    public string Top(int[] t) => GetEdgeFromTransformationElement(t[0]);
    public string Right(int[] t) => GetEdgeFromTransformationElement(t[1]);
    public string Bottom(int[] t) => GetEdgeFromTransformationElement(t[2]);
    public string Left(int[] t) => GetEdgeFromTransformationElement(t[3]);

    public bool IsNeighbour(Tile tile)
    {
        return AllTransformedEdges.Intersect(tile.AllTransformedEdges).Count() > 0;
    }

    
}
