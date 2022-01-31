const string filename = "input4_2021.txt";



Part1();
Part2();

IEnumerable<(int n, Board b)> NextWinner(IEnumerable<Board> boards, int[] numbers)
{
    while (boards.Count() > 0)
    {
        var winner = numbers.SelectMany(n => boards.Select(b => (n, b))).Where(t => t.b.Mark(t.n)).First();
        yield return winner;
        boards = boards.Where(b => b != winner.b);
    }
}

void Part1()
{
    var (boards, numbers) = ReadInput();

    var first = NextWinner(boards, numbers).First();
    Console.WriteLine(first.b.Score() * first.n);
}

void Part2()
{
    var (boards, numbers) = ReadInput();

    var first = NextWinner(boards, numbers).Last();
    Console.WriteLine(first.b.Score() * first.n);
}


(List<Board> boards, int[] numbers) ReadInput()
{
    var numbers = File.ReadLines(filename).First().Split(',', StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();

    var boards = new List<Board>();
    var linesEnumerator = File.ReadLines(filename).Skip(1).GetEnumerator();
    Board? board = null;
    while ((board = Board.CreateFromLines(linesEnumerator)) != null)
    {
        boards.Add(board);
    }
    return (boards, numbers);
}

class Board
{
    // it use row major nad col major order matrices in order to compute faster (cache-aware) sums of rows and cols
    // but it this solution it realy doesnt matter cause these matrices fit in my cache line anyway.... so it's producting little overhead and it's over-engineered ;)
    private readonly int[] _contentRowMajor;
    private readonly int[] _contentColMajor;
    private readonly int _cols;
    private readonly int _rows;
    // it use lookup tables to speed-up searching numbers
    private readonly Dictionary<int, int> _lookupTableRowMajor = new();
    private readonly Dictionary<int, int> _lookupTableColMajor = new();

    private Board(int[] content, int cols, Dictionary<int, int> lookupTable)
    {
        _contentRowMajor = content;
        _cols = cols;
        _rows = content.Length / cols;
        _lookupTableRowMajor = lookupTable;

        _contentColMajor = new int[_rows * _cols];
        for (int i = 0; i < _cols; i++)
        {
            for (int j = 0; j < _rows; j++)
            {
                _contentColMajor[i*_rows+j] = _contentRowMajor[j*_cols+i];
                _lookupTableColMajor[_contentColMajor[i * _rows + j]] = i * _rows + j;
            }
        }
    }

    public int Score() => _contentColMajor.Where(n => n!=-1).Sum();

    public bool Mark(int number)
    {
        if (_lookupTableRowMajor.ContainsKey(number))
        {
            _contentRowMajor[_lookupTableRowMajor[number]] = -1;
            _contentColMajor[_lookupTableColMajor[number]] = -1;
        }
        return CheckIsWinning();
    }

    private bool CheckIsWinning()
    {
        for (int i = 0; i < _rows; i++)
        {
            var colSum = 0;
            for (int j = 0; j < _cols; j++)
            {
                colSum += _contentRowMajor[i * _cols + j];
            }
            if (colSum == -1 * _cols) return true;
        }

        for (int i = 0; i < _cols; i++)
        {
            var rowSum = 0;
            for (int j = 0; j < _rows; j++)
            {
                rowSum += _contentColMajor[i*_rows+j];
            }
            if (rowSum == -1 * _rows) return true;
        }

        return false;
    }

    public static Board? CreateFromLines(IEnumerator<string> linesEnumerator)
    {
        var boardLines = new List<string>();

        while (linesEnumerator.MoveNext())
        {
            if (string.IsNullOrWhiteSpace(linesEnumerator.Current))
            {
                if (boardLines.Count == 0)
                {
                    continue;
                }
                break;
            }

            boardLines.Add(linesEnumerator.Current);
        }
        if (boardLines.Count == 0) return null;

        var content = new List<int>();
        var lookupTable = new Dictionary<int, int>();
        var cols = 0;
        for (int i = 0; i < boardLines.Count; i++)
        {
            var rowArr = boardLines[i].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
            cols = rowArr.Length;

            for (int j = 0; j < rowArr.Length; j++)
            {
                lookupTable[rowArr[j]] = i * boardLines.Count + j;
            }
            content.AddRange(rowArr);
        }

        return new Board(content.ToArray(), cols, lookupTable);
    }
}


