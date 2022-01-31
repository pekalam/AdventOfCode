using System;
using System.IO;
using System.Linq;


var layout = File.ReadAllLines("input11.txt").Select(s => s.ToCharArray()).ToArray();
var cells = layout.Select((str, i) => str.Select((c, j) => new Cell(c, j, i,
    Cell.Part1CellStrategy
    //Cell.Part2CellStrategy
)).ToArray()).ToArray();
while (Cell.Updated)
{
    Cell.Updated = false;
    layout = cells.Select(cellsRow => cellsRow.Select(cell => cell.Update(layout)).ToArray()).ToArray();
}

Console.WriteLine(layout.SelectMany(_ => _).Count(c => c == '#'));

partial class Cell
{
    ref char GetSeatInDir(char[][] layout, int horizontal, int vertical, bool firstSeen = true)
    {
        int i = posY + vertical;
        int j = posX + horizontal;

        while (i >= 0 && i < layout.Length && j >= 0 && j < layout[0].Length)
        {
            if (layout[i][j] != _floor)
            {
                return ref layout[i][j];
            }

            i += vertical;
            j += horizontal;

            if (!firstSeen)
            {
                break;
            }
        }

        return ref _null;
    }

    public static char Part1CellStrategy(Cell cell, char[][] layout)
    {
        if (cell.SeatState == _floor)
        {
            return cell.SeatState;
        }

        ref char topLeft = ref cell.GetSeatInDir(layout, -1, -1, false);
        ref char top = ref cell.GetSeatInDir(layout, 0, -1, false);
        ref char topRight = ref cell.GetSeatInDir(layout, +1, -1, false);
        ref char left = ref cell.GetSeatInDir(layout, -1, 0, false);
        ref char right = ref cell.GetSeatInDir(layout, +1, 0, false);
        ref char downLeft = ref cell.GetSeatInDir(layout, -1, +1, false);
        ref char downRight = ref cell.GetSeatInDir(layout, +1, +1, false);
        ref char down = ref cell.GetSeatInDir(layout, 0, +1, false);

        int totalOccupied = 0;
        totalOccupied += topLeft == _occupied ? 1 : 0;
        totalOccupied += top == _occupied ? 1 : 0;
        totalOccupied += topRight == _occupied ? 1 : 0;
        totalOccupied += left == _occupied ? 1 : 0;
        totalOccupied += right == _occupied ? 1 : 0;
        totalOccupied += downLeft == _occupied ? 1 : 0;
        totalOccupied += downRight == _occupied ? 1 : 0;
        totalOccupied += down == _occupied ? 1 : 0;


        if (cell.SeatState == _empty && totalOccupied == 0)
        {
            return _occupied;
        }

        if (cell.SeatState == _occupied && totalOccupied >= 4)
        {
            return _empty;
        }

        return cell.SeatState;
    }


    public static char Part2CellStrategy(Cell cell, char[][] layout)
    {
        if (cell.SeatState == _floor)
        {
            return cell.SeatState;
        }

        ref char topLeft = ref cell.GetSeatInDir(layout,-1, -1);
        ref char top = ref cell.GetSeatInDir(layout,0, -1);
        ref char topRight = ref cell.GetSeatInDir(layout,+1, -1);
        ref char left = ref cell.GetSeatInDir(layout,-1, 0);
        ref char right = ref cell.GetSeatInDir(layout,+1, 0);
        ref char downLeft = ref cell.GetSeatInDir(layout,-1, +1);
        ref char downRight = ref cell.GetSeatInDir(layout,+1, +1);
        ref char down = ref cell.GetSeatInDir(layout,0, +1);

        int totalOccupied = 0;
        totalOccupied += topLeft == _occupied ? 1 : 0;
        totalOccupied += top == _occupied ? 1 : 0;
        totalOccupied += topRight == _occupied ? 1 : 0;
        totalOccupied += left == _occupied ? 1 : 0;
        totalOccupied += right == _occupied ? 1 : 0;
        totalOccupied += downLeft == _occupied ? 1 : 0;
        totalOccupied += downRight == _occupied ? 1 : 0;
        totalOccupied += down == _occupied ? 1 : 0;


        if (cell.SeatState == _empty && totalOccupied == 0)
        {
            return _occupied;
        }

        if (cell.SeatState == _occupied && totalOccupied >= 5)
        {
            return _empty;
        }

        return cell.SeatState;
    }
}

partial class Cell
{
    private static char _null = char.MinValue;
    private static char _occupied = '#';
    private static char _floor = '.';
    private static char _empty = 'L';
    public static bool Updated = true;

    private int posX;
    private int posY;
    private readonly Func<Cell, char[][], char> _cellStrategy;

    public Cell(char seatState, int posX, int posY, Func<Cell, char[][], char> cellStrategy)
    {
        SeatState = seatState;
        this.posX = posX;
        this.posY = posY;
        _cellStrategy = cellStrategy;
    }

    public char SeatState { get; private set; }

    public char Update(char[][] layout)
    {
        var newState = _cellStrategy(this, layout);
        if (newState != SeatState)
        {
            Updated = true;
            SeatState = newState;
        }

        return SeatState;
    }
}