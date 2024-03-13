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

while true do
  printfn "ENTER SEARCH QUERY OR TYPE 'Q' TO EXIT: "

  let query = Console.ReadLine().ToLower()

  if query = "q" then
    Environment.Exit 69

  let getWeatherMsg =
    match query with
    | null
    | ""
    | "cancel" -> CancelRequest
    | _ ->
      GetCurrentRequest
        { SearchQuery = query
          ApiKey = "0238369d03394013894122527240401"
          Language = Some "en" }

  getCurrentWeatherActor <! getWeatherMsg

  Console.ReadLine() |> ignore
