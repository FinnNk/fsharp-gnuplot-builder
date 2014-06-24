// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open System
open System.Diagnostics
open System.IO

type Command = Plot of string | SPlot of string | Contour | Hidden3D
type Commands = Commands of Command list

let plot commands =  
    let gp = new ProcessStartInfo
                (FileName = "gnuplot", UseShellExecute = false, 
                    CreateNoWindow = true, RedirectStandardInput = true) 
                |> Process.Start

    commands
        |> fun (Commands x) -> x
        |> Seq.iter (function
                    | Plot f -> gp.StandardInput.WriteLine ("plot " + f)
                    | SPlot f -> gp.StandardInput.WriteLine ("splot " + f)
                    | Contour -> gp.StandardInput.WriteLine "set contour surface"
                    | Hidden3D -> gp.StandardInput.WriteLine "set hidden3d")

    Console.ReadLine() |> ignore

type GNUPlotBuilder() =
    
    [<CustomOperation("plot")>]
    member x.Plot(Commands c, func) = Commands(c @ [Plot func])

    [<CustomOperation("splot")>]
    member x.SPlot(Commands c, func) = Commands(c @ [SPlot func])

    [<CustomOperation("contour")>]
    member x.Contour(Commands c) = Commands(c @ [Contour])

    [<CustomOperation("hidden3d")>]
    member x.Hidden3D(Commands c) = Commands(c @ [Hidden3D])

    member x.Yield(()) = Commands[]
    
    member x.Bind(Commands c1, f) =
        let (Commands c2) = f () in Commands(c1 @ c2)

    member x.For(c, f) = x.Bind(c, f)

    member x.Return(a) = x.Yield(a)

[<EntryPoint>]
let main argv = 

    let gnuplot = GNUPlotBuilder()

    gnuplot {
        //plot "x + 1"
        //hidden3d
        contour
        splot "sin(x) * cos(y)"        
    } |> plot


    
    0

