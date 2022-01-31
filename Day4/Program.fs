open System
open System.IO
open System.Collections.Generic


type PassFields =
    | Byr
    | Iyr
    | Eyr
    | Hgt
    | Hcl
    | Ecl
    | Pid
    | Cid

let __splitF (line: string) (fname: string) =
    match line.Split(':') with
    | l when l.Length = 2 && l.[0] = fname -> (true, l.[1])
    | _ -> (false, "")

let byr (line: string) =
    match __splitF line "byr" with
    | (true, v) -> try match Int32.Parse v with
                     | n when n >= 1920 && n <= 2002 -> true
                     | _ -> false
                   with 
                     | _ -> false
    | _ -> false

let iyr (line: string) =
    match __splitF line "iyr" with
    | (true, v) -> try match Int32.Parse v with
                     | n when n >= 2010 && n <= 2020 -> true
                     | _ -> false
                   with 
                     | _ -> false
    | _ -> false

let eyr (line: string) =
    match __splitF line "eyr" with
    | (true, v) -> try match Int32.Parse v with
                     | n when n >= 2020 && n <= 2030 -> true
                     | _ -> false
                   with 
                     | _ -> false
    | _ -> false


let hgt (line: string) =
    match __splitF line "hgt" with
    | (true, v) -> 
        if v.EndsWith("cm") then try match Int32.Parse v.[0 .. (v.IndexOf("cm")-1)] with
                                    | n when n >= 150 && n <= 193 -> true
                                    | _ -> false
                                 with
                                    | _ -> false
        elif v.EndsWith("in") then try match Int32.Parse v.[0 .. (v.IndexOf("in")-1)] with
                                    | n when n >= 59 && n <= 76 -> true
                                    | _ -> false
                                   with
                                    | _ -> false
        else false
    | _ -> false

let hcl (line: string) =
    match __splitF line "hcl" with
    | (true, v) when v.StartsWith("#") && (Seq.forall Char.IsLetterOrDigit v.[1..]) -> true
    | _ -> false


let ecl (line: string) =
    let allowed = ["amb"; "blu"; "brn"; "gry"; "grn"; "hzl"; "oth"]
    match __splitF line "ecl" with
    | (true, v) when Seq.contains v allowed -> true
    | _ -> false

let pid (line: string) =
    match __splitF line "pid" with
    | (true, v) when (Seq.forall Char.IsDigit v) && v.Length = 9 -> true
    | _ -> false

let cid (line: string) =
    match __splitF line "cid" with
    | (true, _) -> true
    | _ -> false


let countValidPassports (lines: string IEnumerator) =
    let validFields = [[Byr; Iyr; Eyr; Hgt; Hcl; Ecl; Pid;]; [Byr; Iyr; Eyr; Hgt; Hcl; Ecl; Pid; Cid;]]

    let appendFields (splitFieldVal: string []) (currFields: PassFields list) = 
        let rec _appendFields (splitFields: string list) acc =
            match splitFields with
                | x::s when byr x -> _appendFields s (acc@[Byr])
                | x::s when iyr x -> _appendFields s (acc@[Iyr])
                | x::s when eyr x -> _appendFields s (acc@[Eyr])
                | x::s when hgt x -> _appendFields s (acc@[Hgt])
                | x::s when hcl x -> _appendFields s (acc@[Hcl])
                | x::s when ecl x -> _appendFields s (acc@[Ecl])
                | x::s when pid x -> _appendFields s (acc@[Pid])
                | x::s when cid x -> _appendFields s (acc@[Cid])
                | x::s when String.IsNullOrEmpty x -> []
                | _ -> acc
        currFields @ _appendFields (splitFieldVal |> Seq.toList) []

    let rec _count (currFields: PassFields list) (validFields: PassFields list list) acc =
        match lines.MoveNext() with
            | true -> 
                match appendFields (lines.Current.Split(' ')) currFields with 
                            | newFields when newFields.Length = currFields.Length -> _count [] validFields acc //blank line
                            | newFields when Seq.contains (newFields |> Seq.sort |> Seq.toList) validFields -> _count [] validFields acc+1
                            | newFields -> _count newFields validFields acc
            | false -> acc
    _count [] validFields 0



[<EntryPoint>]
let main argv =
    printfn "%d" <| countValidPassports (File.ReadLines("input4.txt").GetEnumerator())
    0