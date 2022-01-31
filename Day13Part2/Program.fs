open System
open System.IO


[<EntryPoint>]
let main argv =
    let timestamps = (File.ReadAllLines "input13.txt" |> Seq.skip 1 |> Seq.exactlyOne).Split(',')
    let numIndList = List.zip (timestamps |> Seq.toList) (seq {0 .. (timestamps.Length - 1)} |> Seq.toList) |> Seq.filter (fun x -> not (fst x = "x")) |> Seq.map (fun x -> ((fst x |> Convert.ToInt64), (snd x |> Convert.ToInt64))) |> Seq.toArray

    let frstMatch = Seq.find (fun t -> t % fst numIndList.[0] = 0L && (t + snd numIndList.[1]) % fst numIndList.[1] = 0L) (seq {0L .. Int64.MaxValue})
    let step = (fst numIndList.[0] * fst numIndList.[1])

    let lowestMatch = Seq.fold (fun acc x -> [|acc.[0]+(Seq.find (fun t -> ((acc.[0] + t * acc.[1]) + snd x) % fst x = 0L) (seq {0L .. Int64.MaxValue}) )*acc.[1]; 
                                                acc.[1]*(fst x)|]) [|frstMatch; step|] (numIndList |> Seq.skip 2)
    
    printf "%d" lowestMatch.[0]
    0