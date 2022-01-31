open System
open System.IO


type Tree =
    | Node of string * Tree list
    | Leaf of string

let t1 = Node ("x", [Leaf "x"; Leaf "x"]);

let rec walk tree =
    match tree with
    | Leaf v -> v 
    | Node (value, children) -> value + (List.fold (fun acc x -> acc + x) " " (List.map walk children))



type Cube = 
    { x: int; y: int; z: int; neighbours: Cube list }

    static member Create x y z =
        { x=x; y=y; z=z; neighbours=List.empty }
        

    member this.fillNeighbours =
        let rec _fill x y z (acc: Cube list) =
            if x < 1 then acc @ [Cube.Create x y z] @ (_fill (x+1) y z acc)
            elif y < 1 then acc @ [Cube.Create x y z] @ (_fill -1 (y+1) z acc)
            elif z < 1 then acc @ [Cube.Create x y z] @ (_fill -1 -1 (z+1) acc)
            else acc
        this.neighbours = (_fill -1 -1 -1 [])

    // x = -1, 0, 1; y = -1 0 1; z = -1, 0, 1;
    member this.neighbourAt (x: int) (y: int) (z: int) =
        //row major order
        let xi = x + 1
        let yi = y + 1
        let zi = z + 1
        this.neighbours.[9 * zi + (3 * yi + xi)]

    member this.value =
        this.neighbourAt 0 0 0


[<EntryPoint>]
let main argv =
    //File.ReadAllLines "input17.txt"
    printfn "%s" <| walk t1
    0