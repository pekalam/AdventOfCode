using System;
using System.Collections.Generic;
using System.Linq;

var input = "6,19,0,5,7,13,1";
var startNumbers = input.Split(',').Select((Func<string, int>)Convert.ToInt32).ToArray();

var game = new Game(startNumbers);

Console.WriteLine(Enumerable.Range(0, int.MaxValue)
    .Select(_ => game.SpeakNumber())
    .First(_ => game.Turn == 2020));

Console.WriteLine(Enumerable.Range(0, int.MaxValue)
    .Select(_ => game.SpeakNumber())
    .First(_ => game.Turn == 30000000));


class Game
{
    private readonly int[] _startNumbers;
    // solution for lack of common implemented interface of Dictionary and Array classes
    private readonly Func<int, int> _spokenAtGet;
    private readonly Action<int, int> _spokenAtSet;
    private int _currentSpoken;

    // solution for lack of common implemented interface of Dictionary and Array classes
    private int this[int ind]
    {
        get => _spokenAtGet(ind);
        set => _spokenAtSet(ind, value);
    }

    public int Turn { get; private set; }

    /// <summary>
    /// If maxNth is greater than 0 and <see cref="SpeakNumber"/> is used more than maxNth times <see cref="SpeakNumber"/> CAN throw.
    /// </summary>
    public Game(int[] startNumbers, int maxNth = 0)
    {
        if (maxNth > 0)
        {
            var arrStorage = Enumerable.Range(0, maxNth).Select(_ => 0).ToArray();
            _spokenAtGet = (ind) => arrStorage[ind];
            _spokenAtSet = (ind, val) => arrStorage[ind] = val;
        }
        else
        {
            var dictStorage = new Dictionary<int, int>();
            _spokenAtGet = (ind) => dictStorage.ContainsKey(ind) ? dictStorage[ind] : 0;
            _spokenAtSet = (ind, val) => dictStorage[ind] = val;
        }

        _startNumbers = startNumbers;
        foreach (var number in startNumbers)
        {
            this[number] = ++Turn;
        }
        _currentSpoken = startNumbers[^1];
    }

    public int SpeakNumber()
    {
        if (Turn == _startNumbers.Length)
        {
            Turn++;
            return _currentSpoken = 0;
        }

        if (this[_currentSpoken] != 0)
        {
            var newSpoken = Turn - this[_currentSpoken];
            this[_currentSpoken] = Turn;
            Turn++;
            _currentSpoken = newSpoken;
        }
        else
        {
            this[_currentSpoken] = Turn;
            _currentSpoken = 0;
            Turn++;
        }

        return _currentSpoken;
    }
}