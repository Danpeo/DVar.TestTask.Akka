open System

open Akka.FSharp
open DVar.TestTask.Akka.WeatherActors
open DVar.TestTask.Akka.WeatherModels

Console.WriteLine(
  "  ______     __          __        __         _   _               
 |  _ \ \   / /_ _ _ __  \ \      / /__  __ _| |_| |__   ___ _ __ 
 | | | \ \ / / _` | '__|  \ \ /\ / / _ \/ _` | __| '_ \ / _ \ '__|
 | |_| |\ V / (_| | |      \ V  V /  __/ (_| | |_| | | |  __/ |   
 |____/  \_/ \__,_|_|       \_/\_/ \___|\__,_|\__|_| |_|\___|_|   
                                                                  "
)

let private lang = "en"
let apiKey = "0238369d03394013894122527240401"



while true do
  printfn
    "ENTER COMMANDS: 'current' - TO DISPLAY CURRENT WEATHER, 'forecast' - TO DISPLAY FORECAST, 'cancel' - TO CANCEL REQUEST, AND 'q' TO EXIT"

  let command = Console.ReadLine().ToLower().Trim()

  match command with
  | "q" -> Environment.Exit 69
  | "forecast" ->
    printf "ENTER FORECAST QUERY: "
    let query = Console.ReadLine().ToLower()

    let getForecastMsg =
      match query with
      | null
      | ""
      | "cancel" -> CancelForecastRequest
      | _ ->
        GetForecastRequest
          { Days = 1
            GetCurrentRequest =
              { SearchQuery = query
                ApiKey = apiKey
                Language = Some lang } }

    getForecastActor <! getForecastMsg
  | "current" ->
    printf "ENTER CURRENT QUERY: "
    let query = Console.ReadLine().ToLower()

    let getWeatherMsg =
      match query with
      | null
      | ""
      | "cancel" -> CancelRequest
      | _ ->
        GetCurrentRequest
          { SearchQuery = query
            ApiKey = apiKey
            Language = Some lang }

    getCurrentWeatherActor <! getWeatherMsg
  | _ -> ()

  Console.ReadLine() |> ignore
