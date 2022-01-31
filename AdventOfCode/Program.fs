namespace Day1Program
open System.IO


module Day1 =
    let solution1 (arr: list<int>) =
        let rec nested i j =
            match arr with
            | [] -> []
            | _ when arr.Length = j -> []
            | _ when (not (i = j)) && arr.[i]+arr.[j] = 2020 -> [arr.[i]; arr.[j]]
            | _ -> nested i (j+1)

        let rec outer i =
            if i = arr.Length then
                []
            else
                let result = nested i 0
                if result.Length > 0 then
                    result
                else
                    outer (i+1)

        let nums = outer 0
        if nums.Length > 0 then
            printfn "%d" (nums.[0] * nums.[1])
        else
            ()

    let main (argv: string[]) =
        File.ReadAllLines "input1.txt" |> Seq.map int |> Seq.toList |> solution1 
        0