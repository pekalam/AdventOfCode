using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using allergen = System.String;
using ingredient = System.String;


var readResults = ReadInput("input21.txt");

var ingredientsContainingAlergen = new HashSet<ingredient>();
var maxIngredients = new List<(ingredient Ingredient, allergen Allergen, int Count)>();
foreach (var (k,v) in readResults.allergenToIngredients)
{
    var grouped = v.GroupBy(v => v, (a, aGroup) => (Ingredient: a, Allergen: k, Count: aGroup.Count()));
    var maxCount = grouped.Max(g => g.Count);
    var maxAllergensGroup = grouped.Where(g => g.Count == maxCount).ToArray();
    foreach (var a in maxAllergensGroup)
    {
        ingredientsContainingAlergen.Add(a.Ingredient);
    }
    maxIngredients.AddRange(maxAllergensGroup);
}


//part1
var possibleNotContaining = readResults.allergenToIngredients.Values.Select(a => a.Except(ingredientsContainingAlergen)).SelectMany(a => a).Distinct().ToArray();
Console.WriteLine(readResults.productIngredients.Sum(v => v.Count(a => possibleNotContaining.Contains(a))));

//part2
//MAX-FLOW - maxIngrededients -> allergens
var ingredientIndicesMap = maxIngredients.Select(i => i.Ingredient).Distinct().Select((ing, i) => (ing, i)).ToDictionary(k => k.ing, v => v.i+1);
var allergenIndicesMap = readResults.Allergens.Select((a, i) => (a,i)).ToDictionary(k => k.a, v => ingredientsContainingAlergen.Count+v.i+1);
var adjacencyMatrix = new int[ingredientIndicesMap.Count + allergenIndicesMap.Count + 2, ingredientIndicesMap.Count + allergenIndicesMap.Count + 2];
var ingredientToAllergen = new Dictionary<ingredient, allergen>();

for (int i = 0; i < maxIngredients.Count; i++)
{
    adjacencyMatrix[allergenIndicesMap[maxIngredients[i].Allergen], adjacencyMatrix.GetLength(1)-1] = 1; 
    adjacencyMatrix[0, ingredientIndicesMap[maxIngredients[i].Ingredient]] = 1; 
    adjacencyMatrix[ingredientIndicesMap[maxIngredients[i].Ingredient], allergenIndicesMap[maxIngredients[i].Allergen]] = 1; 
}

var flow = ford(adjacencyMatrix, 0, adjacencyMatrix.GetLength(0) - 1);

for (int i = 1; i < flow.GetLength(0)/2; i++)
{
    var ingredient = ingredientIndicesMap.First(kv => kv.Value == i).Key;

    for (int j = flow.GetLength(1)/2; j < flow.GetLength(1)-1; j++)
    {
        if(flow[i, j] == 0)
        {
            continue;
        }

        var allergen = allergenIndicesMap.First(kv => kv.Value == j).Key;
        ingredientToAllergen[ingredient] = allergen;
    }
}

Console.WriteLine(string.Join(',', ingredientToAllergen.OrderBy(kv => kv.Value).Select(kv => kv.Key)));


static int[,] ford(int[,] adjacencyMatrix, int root, int target)
{
    var residual = (int[,])adjacencyMatrix.Clone();

    int[] bfs()
    {
        var vertices = new (bool visited, int visitedFrom)[adjacencyMatrix.GetLength(0)];
        var queue = new Queue<int>();
        vertices[root].visited = true;
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            var v = queue.Dequeue();
            for (int i = 0; i < adjacencyMatrix.GetLength(1); i++)
            {
                if (residual[v, i] > 0 && !vertices[i].visited)
                {
                    vertices[i].visited = true;
                    vertices[i].visitedFrom = v;
                    queue.Enqueue(i);
                }
            }
        }

        if (!vertices[target].visited)
        {
            return null;
        }

        List<int> path = new(4);
        void GetPath(int target)
        {
            path.Add(target);

            if (target == 0)
            {
                return;
            }
            else
            {
                GetPath(vertices[target].visitedFrom);
            }
        }
        GetPath(target);
        path.Reverse();
        return path.ToArray();
    }


    int[] path = null;
    while ((path = bfs()) != null)
    {
        int cfp = int.MaxValue;
        for (int i = 1; i < path.Length; i++)
        {
            cfp = Math.Min(cfp, residual[path[i - 1], path[i]]);
        }

        for (int i = 1; i < path.Length; i++)
        {
            residual[path[i - 1], path[i]] -= cfp;
            residual[path[i], path[i - 1]] += cfp;
        }
    }

    var flow = (int[,])adjacencyMatrix.Clone();
    for (int i = 0; i < residual.GetLength(0); i++)
    {
        for (int j = 0; j < residual.GetLength(1); j++)
        {
            flow[i, j] = adjacencyMatrix[i, j] > 0 && residual[i, j] < adjacencyMatrix[i, j] ? adjacencyMatrix[i, j] - residual[i, j] : 0;
        }

    }
    return flow;
}


InputReadResults ReadInput(string filePath)
{
    var allergenToIngredients = new Dictionary<allergen, List<ingredient>>();
    var productIngredients = new List<List<ingredient>>();
    var allergenSet = new HashSet<allergen>();
    foreach (var line in File.ReadLines(filePath))
    {
        var split = line.Split(" (contains ");
        var (ingredientList, allergenList) = (split[0], split[1].TrimEnd(')'));

        var ingredients = ingredientList.Split(' ')
            .Select(s => string.Intern(s))
            .ToList();
        var allegrens = allergenList.Replace(" ", "").Split(',');

        productIngredients.Add(ingredients);

        foreach (var a in allegrens)
        {
            if (!allergenToIngredients.ContainsKey(a))
            {
                allergenToIngredients.Add(a, ingredients.ToList());
            }
            else
            {
                allergenToIngredients[a].AddRange(ingredients);
            }
            allergenSet.Add(a);
        }
    }

    return new InputReadResults() { allergenToIngredients=allergenToIngredients, productIngredients=productIngredients, Allergens=allergenSet.ToList() };
}

class InputReadResults
{
    public Dictionary<allergen, List<ingredient>> allergenToIngredients;
    public List<List<ingredient>> productIngredients { get; set; }
    public List<allergen> Allergens { get; set; }
}

