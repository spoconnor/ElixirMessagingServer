namespace ErsharpClient

open System
open Otp


// Start server with
// iex --sname servernode --cookie cookie mathserver.ex                             


// TODO - This should be in another file...

type public Pid = { Node     : string;
                    Id       : int;
                    Serial   : int;
                    Creation : int }
             
type public Port = { Node     : string;
                     Id       : int;
                     Creation : int }
              
type public Ref = { Node     : string;
                    Creation : int;
                    Ids      : int list }             
              
type public Term =
    | Double of float
    | Integer of bigint
    | List of Term list
    | Tuple of Term list
    | Atom of string
    | Binary of byte[]
    | Pid of Pid
    | Port of Port
    | Ref of Ref

module Wrapper =
    
    let rec ToTerm (term:Erlang.Object) =
        match term with
        | :? Erlang.Atom as atom -> Term.Atom <| atom.atomValue()
        | :? Erlang.Binary as binary -> Term.Binary <| binary.binaryValue()
        | :? Erlang.Double as double -> Term.Double <| double.doubleValue()
        | :? Erlang.List as list -> 
          match list.elements() with
          | null -> 
              Term.List []
          | _ ->
              let contents = list.elements()
                             |> Seq.map ToTerm
                             |> Seq.toList
              Term.List contents
        | :? Erlang.Long as long -> Term.Integer <| bigint (long.longValue())
        | :? Erlang.Pid as pid -> Term.Pid {Node = pid.node(); Id = pid.id(); Serial = pid.serial(); Creation = pid.creation()}
        | :? Erlang.Port as port -> Term.Port {Node = port.node(); Id = port.id(); Creation = port.creation()}
        | :? Erlang.Ref as ref -> Term.Ref {Node = ref.node(); Creation = ref.creation(); Ids =  ref.ids() |> Array.toList }
        | :? Erlang.Tuple as tuple ->
          let contents = tuple.elements()
                         |> Seq.map ToTerm
                         |> Seq.toList
          Term.Tuple contents
          
        | _ -> raise <| new System.InvalidOperationException(term.ToString())
        
    let rec ToOtpObject (term : Term) =
      match term with
      | Term.Atom atom -> new Erlang.Atom(atom) :> Erlang.Object
      | Term.Binary binary -> new Erlang.Binary(binary) :> Erlang.Object
      | Term.Double double -> new Erlang.Double(double) :> Erlang.Object
      | Term.Integer long -> new Erlang.Long(int64 long) :> Erlang.Object
      | Term.List list -> new Erlang.List( list 
                                           |> Seq.map ToOtpObject
                                           |> Seq.toArray ) :> Erlang.Object
      | Term.Pid pid -> new Erlang.Pid(pid.Node, pid.Id, pid.Serial, pid.Creation) :> Erlang.Object
      | Term.Port port -> new Erlang.Port(port.Node, port.Id, port.Creation) :> Erlang.Object
      | Term.Ref ref -> new Erlang.Ref(ref.Node, ref.Ids |> List.toArray, ref.Creation) :> Erlang.Object
      | Term.Tuple tuple -> new Erlang.Tuple ( tuple
                                           |> Seq.map ToOtpObject
                                           |> Seq.toArray ) :> Erlang.Object



module Main =
         
  let ConnectTo server cookie =
    let clientNode = new OtpSelf("clientnode", cookie)
    let serverNode = new OtpPeer(server)
    clientNode.connect(serverNode)

  let ReceiveRPC (connection : OtpConnection) =
    Wrapper.ToTerm <| connection.receiveRPC()
    
  let MakeRPC (connection : OtpConnection) (moduleName:string) (functionName:string) (arguments: Term seq) =
     connection.sendRPC(moduleName, functionName, arguments |> Seq.map Wrapper.ToOtpObject |> Seq.toArray)
     ReceiveRPC connection
    
  let StartGenServer (connection : OtpConnection)  =
    match MakeRPC connection "Elixir.Mathserver" "start_link" [] with
    | Term.Tuple [ Term.Atom "ok" ; pid ] -> pid
    | Term.Tuple [ Term.Atom "error"; Term.Tuple [ Term.Atom "already_started" ; pid2 ]] -> pid2
    | start -> raise (new System.InvalidOperationException(start.ToString()))
    
  [<EntryPoint>]
  let Main arguments =
    let connection = ConnectTo "servernode@zen" "cookie"

    let pid = StartGenServer connection
    
    let args = Term.Tuple[ Term.Pid pid ; Term.Integer 6I ; Term.Integer 9I]
    
    match MakeRPC connection "Elixir.Mathserver" "multiply" args with
    | Term.Tuple [ Term.Atom "ok" ; Term.Integer value ] -> 
      Console.WriteLine("Return Value:" + value.ToString())
    | result -> Console.WriteLine("Failure:" + result.ToString())
    
    0

