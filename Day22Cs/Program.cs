using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;



Part1();
Part2();


void Part2()
{
    var (p1Deck, p2Deck) = ReadPlayersDeck("input22.txt");

    var winner = RecursiveCombat(p1Deck, p2Deck) == 1 ? p1Deck : p2Deck;
    Console.WriteLine(CalculateScore(winner));
}

//TODO SPAN custom queue

int CalculateScore(Queue<int> winnerDeck)
{
    return winnerDeck.Select((c, i) => (card: c, points: winnerDeck.Count - i)).Aggregate(0, (acc, el) => acc + (el.card * el.points));
}


int RecursiveCombat(Queue<int> p1Deck, Queue<int> p2Deck)
{
    var hashSet = new HashSet<(int, int, int?, int?, int?, int?)>();

    while (p1Deck.Count != 0 && p2Deck.Count != 0)
    {
        var p1Arr = p1Deck.ToArray();
        var p2Arr = p2Deck.ToArray();
        var headTail = (p1Deck.Peek(), p2Deck.Peek(), p1Arr.Length > 1 ? (int?)p1Arr[^1] : null, p1Arr.Length > 2 ? (int?)p1Arr[^2] : null,
            p2Arr.Length > 2 ? (int?)p2Arr[^2] : null, p2Arr.Length > 2 ? (int?)p2Arr[^2] : null);
        if (hashSet.Contains(headTail))
        {
            return 1;
        }
        hashSet.Add(headTail);


        var p1 = p1Deck.Dequeue();
        var p2 = p2Deck.Dequeue();
        
        
        if (p1 == p2)
        {
            continue;
        }

        if(p1Deck.Count >= p1 && p2Deck.Count >= p2) 
        {
            var winner = RecursiveCombat(new Queue<int>(p1Deck.ToArray().Take(p1)), new Queue<int>(p2Deck.ToArray().Take(p2)));
            if(winner == 1)
            {
                p1Deck.Enqueue(p1); p1Deck.Enqueue(p2);
            }
            else
            {
                p2Deck.Enqueue(p2); p2Deck.Enqueue(p1);
            }
        }
        else if (p1 > p2)
        {
            p1Deck.Enqueue(p1); p1Deck.Enqueue(p2);
        }
        else
        {
            p2Deck.Enqueue(p2); p2Deck.Enqueue(p1);
        }
    }
    return p1Deck.Count > 0 ? 1 : 2;
}


void Part1()
{
    var (p1Deck, p2Deck) = ReadPlayersDeck("input.txt");

    while (p1Deck.Count != 0 && p2Deck.Count != 0)
    {
        var p1 = p1Deck.Dequeue();
        var p2 = p2Deck.Dequeue();
        if (p1 == p2)
        {
            continue;
        }

        if (p1 > p2)
        {
            p1Deck.Enqueue(p1); p1Deck.Enqueue(p2);
        }
        else
        {
            p2Deck.Enqueue(p2); p2Deck.Enqueue(p1);
        }
    }
    var winnerDeck = p1Deck.Count == 0 ? p2Deck : p1Deck;

    Console.WriteLine(CalculateScore(winnerDeck));
}

(Queue<int> p1Deck, Queue<int> p2Deck) ReadPlayersDeck(string filePath)
{
    var p1Deck = new Queue<int>(20);
    var p2Deck = new Queue<int>(20);

    var currentDeck = p1Deck;
    foreach (var line in File.ReadLines("input.txt"))
    {
        if (line.Contains("Player 1"))
        {
            currentDeck = p1Deck;
            continue;
        }
        else if (line.Contains("Player 2"))
        {
            currentDeck = p2Deck;
            continue;
        }
        if (string.IsNullOrEmpty(line))
        {
            continue;
        }

        currentDeck.Enqueue(int.Parse(line));
    }
    return (p1Deck, p2Deck);
}



