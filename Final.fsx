#time "on"
#r "nuget: Akka.Fsharp"
#r "nuget: Akka.TestKit"


open System
open System.Configuration
open System.Security.Cryptography
open System.Text
open Akka.FSharp
open Akka.Actor
open System.Collections.Generic

type Progression =
    | Begin
    | Continue

type Message =
    | BossMess of NoOfZero: int
    | OutputMess of bitcoinString: string * HashCode: string

let timer =
    System.Diagnostics.Stopwatch()

let outputActor message =
    match box message with
    | :? Message as checknow ->
        match checknow with
        | OutputMess (bitcoinString, HashCode) ->
            printfn "%s ==> %s" bitcoinString HashCode
            


let ChildrenActor (mailbox: Actor<_>) message =

    let randomStr = 
        let size: int = 50
        let rd = Random()
        let charOpt = Array.concat([[|'a' .. 'z'|];[|'A' .. 'Z'|];[|'0' .. '9'|]])
        let charsLen = Array.length charOpt in
        String(Array.init size (fun _ -> charOpt.[rd.Next charsLen]))     

    let usrnm = "apoorvkhosla;"

    let bitcoinString = usrnm + randomStr
    

        

    let SHAandZero (strToChange: string,ZerodVal: int) =
        let isCoin s numZeros = 
            let mutable count = 0
            let mutable b = false
            let mutable btc = false
            while (b = false) do 
                //printfn "Checking Char %s" (string (string s).[count])
                if (((string s).[count]).Equals('0')) then
                    //printfn "Got an Initial Zero"
                    count <- count + 1
                else
                    //printfn "Found a Non-Zero... B is being set to false"
                    b <- true
                    // b.Equals 'true'

                if (count = numZeros) then
                    b <- true
                    btc <- true
            btc



        let HexToBin (s: string) =
            let mutable a = string ""
            for i in s do
                if (string i).Equals(string "-") then
                    None
                else
                    a <- a + (string i) 
                    None

            //printfn "%s" a
            a


        let StringToSHA (s : string) =
            let res = () in 
            s 
            |> Encoding.ASCII.GetBytes 
            |> (new SHA256Managed()).ComputeHash 
            |> System.BitConverter.ToString 
        
        // let fString = HexToBin(StringToSHA(s))
        let TextToCoinCheck s numZeros = 
            // let a = isCoin (HexToBin (StringToSHA s)) numZeros
            if isCoin (HexToBin (StringToSHA s)) numZeros then
                select "/user/outputActor" mailbox.Context.System
                    <! OutputMess (s, StringToSHA s)
                printfn "%s\t%s" s (HexToBin (StringToSHA s)) 
            // a


            //let a = StringToSHA "adobra;123aa"
            //let b = HexToBin a

            //printfn "%s" a
            //printfn "%s" b
            //isCoin b 3
            //isCoin (HexToBin (StringToSHA "adobra;123aa")) 1
        let test = TextToCoinCheck bitcoinString ZerodVal
        // TextToCoinCheck "adobra;kjsdfk11" ZerodVal
        // printfn "%b" test
        // printfn ""
        test
    // SHAandZero(RSV_value, cmdInt)


    match box message with
                | :? Message as bootBoss ->
                    match bootBoss with
                    | BossMess (cmdZero) ->
                        SHAandZero(bitcoinString, cmdZero)
                        mailbox.Self <! message




let BossActor (mailbox: Actor<_>) message =
    let expectOP(ZeroCt: int) =
        let actorTop = 100000l
        let bossList = [for temp in 1l .. actorTop do yield (spawn mailbox.Context ("completeWorker" + string temp) (actorOf2 ChildrenActor))]
        for temp in 0l .. (actorTop-1l) do
            bossList.Item(temp |> int) <! BossMess (ZeroCt)


    let rec fetchCmdLine() =
        let cmdArgs: string array = fsi.CommandLineArgs |> Array.tail
        let cmdInt = cmdArgs.[0] |> int
        expectOP(cmdInt)

    match box message with
    | :? Progression as startnow ->
        fetchCmdLine ()

timer.Start()
let ActorSystem = System.create "ActorSystem" (Configuration.load ())

let bossAgent = spawn ActorSystem "bossAgent" (actorOf2 BossActor)
// let bossAgent1 = spawn ActorSystem "bossAgent1" (actorOf2 BossActor)
let childAgent = spawn ActorSystem "childAgent" (actorOf outputActor)


bossAgent <! Begin


ActorSystem.Terminate()  
timer.Stop()
printfn "Elapsed Milliseconds: %i" timer.ElapsedMilliseconds
// ActorSystem.WhenTerminated.Wait ()
