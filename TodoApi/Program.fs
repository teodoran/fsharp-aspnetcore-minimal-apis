open System
open Microsoft.AspNetCore.Builder

let args = Environment.GetCommandLineArgs()[1..]
let builder = WebApplication.CreateBuilder(args)
let app = builder.Build()

app.MapGet("/", Func<string>((fun () -> "Hello World!"))) |> ignore

app.Run()