module DVar.TestTask.Akka.WeatherActors

open System
open System.Net
open System.Net.Http
open System.Net.Http.Json
open Akka.FSharp
open DVar.TestTask.Akka.WeatherModels
open DVarAkkaSystems

let private weatherApi = "https://api.weatherapi.com/v1"

let getCurrentWeatherActor =
  spawn weatherSystem "get-current-weather-actor"
  <| fun (mailbox: Actor<GetCurrentMessage>) ->
    let handleResponseActor =
      select "akka://weather-system/user/handle-response-actor" weatherSystem

    let rec loop () =
      actor {
        let! getCurrentMsg = mailbox.Receive()

        match getCurrentMsg with
        | GetCurrentRequest request ->
          let baseUri =
            $"{weatherApi}/current.json?q={request.SearchQuery}&key={request.ApiKey}"

          let fullUri =
            match request.Language with
            | None
            | Some "" -> baseUri
            | Some lang -> $"{baseUri}&lang={lang}"

          use httpClient = new HttpClient()
          let response = httpClient.GetAsync(fullUri).Result
          handleResponseActor <! response
        | CancelRequest -> printf "REQUEST CANCELLED"

        return! loop ()
      }

    loop ()

let handleResponseActor =
  spawn weatherSystem "handle-response-actor"
  <| fun (mailbox: Actor<HttpResponseMessage>) ->
    let rec loop () =
      actor {
        let! responseMsg = mailbox.Receive()

        match responseMsg.StatusCode with
        | HttpStatusCode.OK ->
          let currentWeather =
            responseMsg.Content.ReadFromJsonAsync<GetCurrentResponse>().Result

          Console.WriteLine(currentWeather)
        | _ -> Console.WriteLine $"Received unsuccessful response with status code: {responseMsg.StatusCode}"

        return! loop ()
      }

    loop ()
