using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

var arr = (new int[] { 3,9,8,2,5,4,7,1,6 }).Concat(Enumerable.Range(10, 1_000_000 - 9)).ToArray();
//var arr = new int[] { 3,9,8,2,5,4,7,1,6 };

var successorsArray = new int[1 + arr.Length];

for (int i = 0; i < arr.Length; i++)
{
    successorsArray[arr[i]] = arr[(i+1)%arr.Length];
}

var current = arr[0];
for (int i = 0; i < 10_000_000; i++)
{
    var pickStart = successorsArray[current];
    var pickEnd = successorsArray[successorsArray[pickStart]];

    var destination = current - 1;
    var pickedContainsDestination = EnumerateSuccessors(pickStart).Take(3).Has(destination);
    if (pickedContainsDestination)
    {
        while(destination > 0)
        {
            destination--;
            if(!EnumerateSuccessors(pickStart).Take(3).Has(destination))
            {
                break;
            }
        }
    }
    if (destination < 1)
    {
        destination = EnumerateSuccessors(successorsArray[pickEnd]).Take(arr.Length - 3 - 1).Max();
    }

    successorsArray[current] = successorsArray[pickEnd];
    successorsArray[pickEnd] = successorsArray[destination];
    successorsArray[destination] = pickStart;

    current = successorsArray[current];
}
//Console.WriteLine(string.Join(string.Empty, EnumerateSuccessors(successorsArray[1]).Take(arr.Length-1)));
Console.WriteLine(EnumerateSuccessors(successorsArray[1]).First() * (long)EnumerateSuccessors(successorsArray[1]).ElementAt(1));

IEnumerable<int> EnumerateSuccessors(int start)
{
    yield return start;
    while (true)
    {
        start = successorsArray[start];
        yield return start;
    }
}

public static class Ext
{
    public static bool Has(this IEnumerable<int> enumerable, int value)
    {
        foreach (var item in enumerable)
        {
            if(item == value)
            {
                return true;
            }
        }
        return false;
    }
}




