open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Okapi

type WeatherForecast =
    { Date: DateTime
      TemperatureC: int
      TemperatureF: int
      Summary: string }

module RandomWeatherForecast =
    let private summaries = [| "Freezing"; "Bracing"; "Chilly"; "Cool"; "Mild"; "Warm"; "Balmy"; "Hot"; "Sweltering"; "Scorching" |]

    let ocurringAfter (noOfDays: int) =
        let temperatureC = Random.Shared.Next(-20, 55)
        { Date = DateTime.Now.AddDays(noOfDays)
          TemperatureC = temperatureC
          TemperatureF = 32 + (int)((float)temperatureC / 0.5556)
          Summary = summaries[Random.Shared.Next(summaries.Length)] }

let fiveRandomForecasts () = seq { for noOfDays in 1 .. 5 do RandomWeatherForecast.ocurringAfter noOfDays }

Builder.create ()
|> Builder.addServices (fun services ->
    services
        .AddEndpointsApiExplorer()
        .AddSwaggerGen())
|> Builder.build
|> Application.configure (fun app ->
    if app.Environment.IsDevelopment() then
        app.UseSwagger().UseSwaggerUI() |> ignore
    app.UseHttpsRedirection())
|> Application.mapGet "/weatherforecast" "GetWeatherForecast" (fiveRandomForecasts>>Seq.toArray)
|> Application.run