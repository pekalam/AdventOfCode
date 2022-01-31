var lines = File.ReadLines("input3_2021.txt");
var length = lines.First().Length;

int[] oneOccurencesOnPos = new int[length];
int[] zeroOccurencesOnPos = new int[length];


foreach (var line in lines)
{
    for (int i = 0; i < oneOccurencesOnPos.Length; i++)
    {
        int[] toInc = (line[i] == '0' ? zeroOccurencesOnPos : oneOccurencesOnPos);
        toInc[i]++;
    }
}

var gammaRate = oneOccurencesOnPos.Select((v, i) => v > zeroOccurencesOnPos[i] ? 1 : 0).ToDec();
var epsRate = oneOccurencesOnPos.Select((v, i) => v > zeroOccurencesOnPos[i] ? 0 : 1).ToDec();

Console.WriteLine(gammaRate*epsRate);


var bits = new List<int[]>();
foreach (var line in lines)
{
    bits.Add(line.Select(i => i-48).ToArray());
}

var oxy = SelectNumber(bits, true);
var co2 = SelectNumber(bits, false);
Console.WriteLine(oxy*co2);

int SelectNumber(List<int[]> bitsToProcess, bool greater)
{
    var ones = new List<int[]>(bitsToProcess.Count);
    var zeros = new List<int[]>(bitsToProcess.Count);

    List<int[]> BitCriteria(List<int[]> bitsToProcess, int pos)
    {
        ones.Clear();
        zeros.Clear();

        foreach (var bits in bitsToProcess)
        {
            if (bits[pos] == 1) ones.Add(bits);
            else zeros.Add(bits);
        }

        return greater ? (ones.Count >= zeros.Count ? ones : zeros) :
            (ones.Count >= zeros.Count ? zeros : ones);
    }

    int pos = 0;
    while(bitsToProcess.Count != 1)
    {
        bitsToProcess = BitCriteria(bitsToProcess, pos++).ToList();
    }

    return bitsToProcess[0].ToDec();
}

static class EnumerableExt
{
    public static int ToDec(this IEnumerable<int> enumerable)
    {
        var p = (int)Math.Pow(2, enumerable.Count()-1);
        var result = 0;
        foreach (var num in enumerable)
        {
            result += p * num;
            p /= 2;
        }
        return result;
    }
}