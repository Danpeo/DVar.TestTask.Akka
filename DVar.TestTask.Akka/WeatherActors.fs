module DVar.TestTask.Akka.WeatherActors

open System
open System.Net
open System.Net.Http
open Akka.FSharp
open DVar.TestTask.Akka.WeatherModels
open DVarAkkaSystems

let private weatherApi = "https://api.weatherapi.com/v1"

let private getHandleResponseActor =
  select "akka://weather-system/user/handle-response-actor" weatherSystem

let private setUri language baseUri =
  match language with
    | None
    | Some "" -> baseUri
    | Some lang -> $"{baseUri}&lang={lang}"

let private httpGet (uri: string) =
  use httpClient = new HttpClient()
  let response = httpClient.GetAsync(uri).Result
  response

let getCurrentWeatherActor =
  spawn weatherSystem "get-current-weather-actor"
  <| fun (mailbox: Actor<GetCurrentMessage>) ->
    let handleResponseActor = getHandleResponseActor

    let rec loop () =
      actor {
        let! getCurrentMsg = mailbox.Receive()
        match getCurrentMsg with
        | GetCurrentRequest request ->
          let baseUri =
            $"{weatherApi}/current.json?q={request.SearchQuery}&key={request.ApiKey}"

          let fullUri = setUri request.Language baseUri
          handleResponseActor <! httpGet fullUri
        | CancelRequest -> printf "REQUEST CANCELLED"

        return! loop ()
      }

    loop ()

let getForecastActor =
  spawn weatherSystem "get-forecast-actor"
  <| fun (mailbox: Actor<GetForecastMessage>) ->
    let handleResponseActor = getHandleResponseActor

    let rec loop () =
      actor {
        let! getForecastMsg = mailbox.Receive()

        match getForecastMsg with
        | GetForecastRequest request ->
          let baseUri =
            $"{weatherApi}/forecast.json?q={request.GetCurrentRequest.SearchQuery}&days={request.Days}&key={request.GetCurrentRequest.ApiKey}"

          let fullUri = setUri request.GetCurrentRequest.Language baseUri
          handleResponseActor <! httpGet fullUri

        | CancelForecastRequest -> printf "REQUEST CANCELLED"

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
          let weather =
            responseMsg.Content.ReadAsStringAsync().Result

          Console.WriteLine(weather)
        | _ -> Console.WriteLine $"Received unsuccessful response with status code: {responseMsg.StatusCode}"

        return! loop ()
      }

    loop ()
