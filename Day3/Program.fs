open System.IO

[<EntryPoint>]
let main _ =
    let count_with_slope x y =
        let count = 
            (File.ReadLines("input3.txt")
            |> Seq.fold (fun (acc: int[]) line -> if acc.[1] = y then (if line.[(acc.[2]+x) % line.Length] = '#' then [|acc.[0]+1; 1; acc.[2]+x|] else [|acc.[0]; 1; acc.[2]+x|]) else [|acc.[0];acc.[1]+1;acc.[2]|]) [|0; 0; 0|] //treecount; y_lines; x_offset
            ).[0]
        int64 count
    [(1,1); (3,1); (5,1); (7,1); (1,2)] |> Seq.fold (fun acc slope -> acc*(count_with_slope (fst slope) (snd slope))) 1L |> printf "%d"
    0
