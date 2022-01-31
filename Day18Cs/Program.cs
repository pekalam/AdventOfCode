using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

var opsPart1 = new Dictionary<char,int>() { {'+', 0 }, { '-', 0 }, { '*', 0 }, { '/', 0 } };
var opsPart2 = new Dictionary<char, int>() { { '+', 1 }, { '-', 0 }, { '*', 0 }, { '/', 0 } };

Console.WriteLine(
    File.ReadLines("input18.txt").Select(s => EvaluateInfix(s, opsPart1)).Aggregate((acc, x) => acc + x)
    );

Console.WriteLine(
    File.ReadLines("input18.txt").Select(s => EvaluateInfix(s, opsPart2)).Aggregate((acc, x) => acc + x)
    );

ulong EvaluateInfix(string expr, Dictionary<char,int> ops) 
{
    return Evaluate(Onp(expr, ops), ops);
}

ulong Evaluate(string onp, Dictionary<char, int> ops)
{
    var onpSplit = onp.Split(' ');
    var numStack = new Stack<ulong>();

    foreach (var item in onpSplit)
    {
        if (ops.ContainsKey(item[0]))
        {
            var n2 = numStack.Pop();
            var n1 = numStack.Pop();
            var result = item[0] switch
            {
                '+' => n1 + n2,
                '-' => n1 - n2,
                '*' => n1 * n2,
                '/' => n1 / n2
            };
            numStack.Push(result);
        }
        else
        {
            numStack.Push(Convert.ToUInt64(item));
        }
    }

    return numStack.Pop();
}

string Onp(string expr, Dictionary<char, int> ops)
{
    var opStack = new Stack<char>();
    var outQueue = new Queue<string>();
    var state = ParseState.N;
    var num = "";
    
    expr = expr.Replace(" ", "");

    for (int i = 0; i < expr.Length; i++)
    {
        state = expr[i] == '(' ? ParseState.LParen : state;
        state = expr[i] == ')' ? ParseState.RParen : state;
        switch (state)
        {
            case ParseState.N:
                if (!char.IsDigit(expr[i]))
                {
                    throw new ArgumentException($"Unknown digit {expr[i]}");
                }
                num += expr[i];
                if (i == expr.Length - 1 || !char.IsDigit(expr[i + 1]))
                {
                    outQueue.Enqueue(num);
                    num = "";
                    state = ParseState.Op;
                }
                break;
            case ParseState.Op:
                if (!ops.ContainsKey(expr[i]))
                {
                    throw new ArgumentException($"Invalid operator {expr[i]}");
                }
                var opPriority = ops[expr[i]];
                while(opStack.Count > 0 && ops.ContainsKey(opStack.Peek()) 
                    && ops[opStack.Peek()] >= opPriority)
                {
                    outQueue.Enqueue(opStack.Pop().ToString());
                }
                opStack.Push(expr[i]);
                state = ParseState.N;
                break;
            case ParseState.LParen:
                opStack.Push(expr[i]);
                state = ParseState.N;
                break;
            case ParseState.RParen:
                while (opStack.Peek() != '(')
                {
                    outQueue.Enqueue(opStack.Pop().ToString());
                    if (opStack.Count == 0)
                    {
                        throw new ArgumentException("Invalid parenthesis");
                    }
                }
                if(opStack.Count == 0)
                {
                    throw new ArgumentException("Invalid parenthesis");
                }
                opStack.Pop();
                if(i < expr.Length - 1)
                {
                    if(char.IsDigit(expr[i + 1]))
                    {
                        throw new ArgumentException("Invalid expression");
                    }
                    state = ParseState.Op;
                }
                break;
            default:
                throw new NotImplementedException();
        }
    }

    while(opStack.Count > 0)
    {
        outQueue.Enqueue(opStack.Pop().ToString());
    }

    return outQueue.Aggregate("", (acc, x) => acc + x + " ").TrimEnd();
}

enum ParseState
{
    N,Op,LParen, RParen
}



