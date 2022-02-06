namespace Okapi

open System
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Builder

module Builder =
    let create () =
        let args = Environment.GetCommandLineArgs()[1..]
        WebApplication.CreateBuilder(args)

    let addServices fn (builder: WebApplicationBuilder) =
        fn builder.Services |> ignore
        builder

    let build (builder: WebApplicationBuilder) = builder.Build()

module Application =
    let configure fn (app: WebApplication) =
        fn app |> ignore
        app

    type ToAction<'a> = delegate of unit -> 'a
    let mapGet pattern name fn (app: WebApplication) =
        app.MapGet(pattern, ToAction(fn)).WithName(name) |> ignore
        app

    let run (application: WebApplication) = application.Run()
