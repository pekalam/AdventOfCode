namespace Day1Program
open System.IO
open System


module Day1 =

    let solution1 (numbers: seq<int>): int =
        123

    let main (argv: string[]) =
        File.ReadAllLines "input1_2021.txt" |> Seq.map int |> solution1 |> Console.WriteLine
        0
