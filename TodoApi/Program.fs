open Microsoft.AspNetCore.Builder

let args = System.Environment.GetCommandLineArgs().[1..]
let builder = WebApplication.CreateBuilder(args)
let app = builder.Build()

type Action<'a> = delegate of unit -> 'a
app.MapGet("/", Action((fun () -> "Hello World!"))) |> ignore

app.Run()