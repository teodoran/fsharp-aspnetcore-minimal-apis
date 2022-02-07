open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

let args = Environment.GetCommandLineArgs().[1..]
let builder = WebApplication.CreateBuilder(args)

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer() |> ignore
builder.Services.AddSwaggerGen() |> ignore

let app = builder.Build()

// Configure the HTTP request pipeline.
if app.Environment.IsDevelopment() then
    app.UseSwagger() |> ignore
    app.UseSwaggerUI() |> ignore

app.UseHttpsRedirection() |> ignore

let summaries = [| "Freezing"; "Bracing"; "Chilly"; "Cool"; "Mild"; "Warm"; "Balmy"; "Hot"; "Sweltering"; "Scorching" |]

type WeatherForecast(date: DateTime, temperatureC: int, summary: string) =
  member _.Date = date
  member _.TemperatureC = temperatureC
  member _.Summary = temperatureC
  member _.TemperatureF = 32 + (int)((float)temperatureC / 0.5556)

app.MapGet("/weatherforecast", Func<_>((fun () ->
    let forecast =
        seq { 1 .. 5 }
        |> Seq.map (fun index ->
            WeatherForecast(
                DateTime.Now.AddDays(index),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]))
        |> Seq.toArray
    forecast)))
    .WithName("GetWeatherForecast") |> ignore

app.Run()