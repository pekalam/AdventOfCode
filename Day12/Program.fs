open System
open System.IO

[<Struct>]
type Coords = {E: int; N: int; F: int}

[<Struct>]
type Coords2 = {E: int; N: int; WayP: Coords}


[<EntryPoint>]
let main argv =
    let result = File.ReadLines "input12.txt" |> Seq.fold (fun acc x -> 
                                                    match (x.[0], (x.[1..] |> Convert.ToInt32)) with
                                                    | ('E', n) -> {acc with E=(acc.E+n)} : Coords
                                                    | ('W', n) -> {acc with E=(acc.E-n)}
                                                    | ('N', n) -> {acc with N=(acc.N+n)}
                                                    | ('S', n) -> {acc with N=(acc.N-n)}
                                                    | ('R', n) -> {acc with F=(match n with
                                                                                      | 90 -> (acc.F + 1) % 4
                                                                                      | 180 -> (acc.F + 2) % 4
                                                                                      | 270 -> (acc.F + 3) % 4
                                                                                      | _ -> acc.F
                                                                                      )}
                                                    | ('L', n) -> {acc with F=(match n with
                                                                                      | 90 -> (acc.F + 2 + 1) % 4
                                                                                      | 180 -> (acc.F + 2) % 4
                                                                                      | 270 -> (acc.F + 2 + 3) % 4
                                                                                      | _ -> acc.F
                                                                                      )}
                                                    | ('F', n) -> match acc.F with 
                                                                  | 0 -> {acc with E=(acc.E+n)}
                                                                  | 1 -> {acc with N=(acc.N-n)}
                                                                  | 2 -> {acc with E=(acc.E-n)}
                                                                  | 3 -> {acc with N=(acc.N+n)}
                                                    ) {E = 0; N = 0; F = 0}
    printfn "%d" ((abs result.E) + (abs result.N))

    let result2 = File.ReadLines "input12.txt" |> Seq.fold (fun acc x -> 
                                                    match (x.[0], (x.[1..] |> Convert.ToInt32)) with
                                                    | ('E', n) -> {acc with WayP={acc.WayP with E=acc.WayP.E+n}} : Coords2
                                                    | ('W', n) -> {acc with WayP={acc.WayP with E=acc.WayP.E-n}}
                                                    | ('N', n) -> {acc with WayP={acc.WayP with N=acc.WayP.N+n}}
                                                    | ('S', n) -> {acc with WayP={acc.WayP with N=acc.WayP.N-n}}
                                                    | ('R', n) -> {acc with WayP=(match n with
                                                                                      | 90 -> {acc.WayP with E=acc.WayP.N; N=(-acc.WayP.E)} : Coords
                                                                                      | 180 -> {acc.WayP with E=(-acc.WayP.E); N=(-acc.WayP.N)}
                                                                                      | 270 -> {acc.WayP with E=(-acc.WayP.N); N=acc.WayP.E}
                                                                                      | _ -> acc.WayP
                                                                                      )}
                                                    | ('L', n) -> {acc with WayP=(match n with
                                                                                      | 90 -> {acc.WayP with E=(-acc.WayP.N); N=acc.WayP.E} : Coords
                                                                                      | 180 -> {acc.WayP with E=(-acc.WayP.E); N=(-acc.WayP.N)}
                                                                                      | 270 -> {acc.WayP with E=acc.WayP.N; N=(-acc.WayP.E)}
                                                                                      | _ -> acc.WayP
                                                                                      )}
                                                    | ('F', n) -> {acc with E=acc.E+n*acc.WayP.E; N=acc.N+n*acc.WayP.N}
                                                    ) {E = 0; N = 0; WayP = {E = 10; N = 1; F = 0;}}
    printfn "%d" ((abs result2.E) + (abs result2.N))

    0