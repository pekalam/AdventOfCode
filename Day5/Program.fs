open System
open System.IO

let id (line: char list) =
    let rec _row_col (line: char list) r_l r_u c_l c_u =
        match line with
        | [] -> (r_l, c_l)
        | x::xs when x = 'F' -> _row_col xs r_l ((r_l+r_u)/2) c_l c_u
        | x::xs when x = 'B' -> _row_col xs ((r_l+r_u)/2+1) r_u c_l c_u
        | x::xs when x = 'L' -> _row_col xs r_l r_u c_l ((c_l+c_u)/2)
        | x::xs when x = 'R' -> _row_col xs r_l r_u ((c_l+c_u)/2+1) c_u
    let (r,c) = _row_col line 0 127 0 7
    r * 8 + c


[<EntryPoint>]
let main argv =
    File.ReadLines("input5.txt") |> Seq.map Seq.toList |> Seq.map id |> Seq.max |> Console.WriteLine
    0 // return an integer exit code