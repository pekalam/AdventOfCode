open System.IO

type passwordPol = {min: int; atMost: int; letter: char; pass: string;}

let countOccurence (str: string) (c: char) =
    let rec _countOccurence str acc = 
        match str with
        | [] -> acc
        | x::xs when x = c -> _countOccurence xs (acc+1)
        | _::xs -> _countOccurence xs acc
    _countOccurence (Seq.toList str) 0

[<EntryPoint>]
let main argv =
    File.ReadAllLines("input2.txt") 
    |> Seq.map (fun (x: string) -> x.Split(' ')) 
    |> Seq.map (fun (x: string[]) -> { min = int(x.[0].Split('-').[0]); atMost = int(x.[0].Split('-').[1]); letter = char(x.[1].TrimEnd(':')); pass = x.[2]}) 
    |> Seq.filter (fun x -> (countOccurence x.pass x.letter) >= x.min && (countOccurence x.pass x.letter) <= x.atMost) 
    |> Seq.length
    |> printfn "%d"

    File.ReadAllLines("input2.txt") 
    |> Seq.map (fun (x: string) -> x.Split(' ')) 
    |> Seq.map (fun (x: string[]) -> { min = int(x.[0].Split('-').[0]); atMost = int(x.[0].Split('-').[1]); letter = char(x.[1].TrimEnd(':')); pass = x.[2]}) 
    |> Seq.filter (fun x -> (x.pass.[x.min-1] = x.letter) <> (x.pass.[x.atMost-1] = x.letter)) 
    |> Seq.length
    |> printfn "%d"

    0