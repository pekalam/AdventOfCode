open System
open System.IO

let parseProgram (program: string[]) : list<string*int> =
    let rec _parseProgram (program: list<string>) acc =
        match program with 
            | [] -> acc
            | x::xs -> _parseProgram xs (acc@[( x.Split(' ').[0], (x.Split(' ').[1] |> Convert.ToInt32) )])
    _parseProgram (program |> Seq.toList) []


let execute (program: list<string*int>) =
    let rec _execute (acc: int) (ip: int) (loopCheck: list<int>) =
        if ip = program.Length || (List.contains ip loopCheck) then acc, ip
        else match program.[ip] with
            | ("nop", _) -> _execute acc (ip+1) (loopCheck@[ip])
            | ("acc", op) -> _execute (acc+op) (ip+1) (loopCheck@[ip])
            | ("jmp", op) -> _execute acc (ip+op) (loopCheck@[ip])
            | _ -> raise (NotImplementedException())
    _execute 0 0 []

let findJmpLoopInstr (program: list<string*int>) =
    let tryClearJmpList (ip: int) (jmpList: list<int>) =
        if jmpList.Length > 0 && ip > (List.max jmpList) then []
        else jmpList
    let rec _find (ip: int) (executed: list<int>) (jmpList: list<int>) =
        let (instr, op) = program.[ip]
        if ip = program.Length || (List.contains ip executed) then jmpList
        elif instr = "jmp" then _find (ip+op) (executed@[ip]) (jmpList@[ip])
        else _find (ip+1) (executed@[ip]) (tryClearJmpList ip jmpList)
    _find 0 [] []

let rec tryReplaceAndExec (jmpList: list<int>) (program: list<string*int>) =
    match jmpList with
    | [] -> -1, -1
    | x::xs -> match (execute (List.mapi (fun i el -> if i = x then ("nop", 0) else el) program)) with
                | (acc, ip) when ip = program.Length -> acc, ip
                | _ -> tryReplaceAndExec xs program

[<EntryPoint>]
let main argv =
    let program = File.ReadAllLines "input8.txt" |> parseProgram
    //printf "acc:%d ip:%d" <|| (execute program)
    printf "acc:%d ip:%d" <|| tryReplaceAndExec (findJmpLoopInstr program) program
    0