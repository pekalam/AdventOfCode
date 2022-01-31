using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


var fileContents = File.ReadLines("input16.txt");
string state = "rules";
var rules = new List<TicketRule>();
int[] myTicket = null!;
var otherTickets = new List<int[]>();
foreach (var line in fileContents)
{
    switch (state)
    {
        case "rules":
            if (string.IsNullOrWhiteSpace(line))
            {
                state = "my";
                break;
            }

            var split = line.Split(':');
            var rule = new TicketRule()
            {
                Name = split[0],
                Ranges = split[1].Split("or").Select(s => s.Trim()).Select(s => new TicketRule.Range()
                {
                    Start = Convert.ToInt32(s.Split('-')[0]),
                    End = Convert.ToInt32(s.Split('-')[1]),
                }).ToArray()
            };
            rules.Add(rule);

            break;
        case "my":
            if (string.IsNullOrWhiteSpace(line))
            {
                state = "other";
                break;
            }

            if (line == "your ticket:")
            {
                break;
            }

            myTicket = line.Split(',').Select((s) => Convert.ToInt32(s)).ToArray();
            break;
        case "other":

            if (line == "nearby tickets:")
            {
                break;
            }

            var ticket = line.Split(',').Select((s) => Convert.ToInt32(s)).ToArray();
            otherTickets.Add(ticket);
            break;
    }
}




Func<int, bool> ticketFieldNotInRange = i => rules.All(rule => rule.Ranges.All(range => !range.InRange(i)));

//part1
var errorRate =
    otherTickets
        .SelectMany(ints => ints)
        .Where(ticketFieldNotInRange)
        .Sum();
Console.WriteLine(errorRate);


//part2
var validTickets = otherTickets
    .Where(ints => !ints.Any(ticketFieldNotInRange));


var validTicketsWithInd = validTickets.Select(ints => ints.Select((n, ind) => (num: n, ind)).ToArray()).ToArray();

var possiblePositions = new List<(int[] positions, TicketRule rule)>();
foreach (var rule in rules)
{
    Func<(int num, int ind), bool> inAnyRange = tuple => rule.Ranges.Any(range => range.InRange(tuple.num));

    var validPositionsOfFirst = validTicketsWithInd[0]
        .Where(inAnyRange)
        .ToArray();

    var validPositions = validPositionsOfFirst
        .Where(validPosTuple => validTicketsWithInd
            .Skip(1)
            .All(ticketTuples => ticketTuples.Any(tuple => tuple.ind == validPosTuple.ind && inAnyRange(tuple)))
        )
        .Select(tuple => tuple.ind)
        .ToArray();

    possiblePositions.Add((validPositions, rule));
}
possiblePositions.Sort((a, b) => a.positions.Length < b.positions.Length ? -1 : 1);

var toExclude = new List<int>();
foreach (var possiblePosition in possiblePositions)
{
    possiblePosition.rule.Position = possiblePosition.positions.Except(toExclude).First();
    toExclude.Add(possiblePosition.rule.Position);
}

Console.WriteLine(rules.Where(rule => rule.Name.StartsWith("departure")).Select(rule => myTicket![rule.Position]).Aggregate(1L, (acc, val) => val * acc));


class TicketRule
{
    public class Range
    {
        public int Start { get; init; }
        public int End { get; init; }

        public bool InRange(int x) => x >= Start && x <= End;
    }

    public string Name { get; init; }
    public Range[] Ranges { get; init; }
    public int Position { get; set; } = -1;
}