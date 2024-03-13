module DVar.TestTask.Akka.Tests.WeatherActorTests

open System
open DVar.TestTask.Akka.WeatherModels
open Akka.FSharp
open Akka.TestKit.Xunit2
open DVarAkkaSystems
open Xunit

type WeatherActorTests() =
  inherit TestKit()

  [<Fact>]
  member this.TestActor() =
    let actor =
      select "akka://weather-system/user/get-current-weather-actor" weatherSystem

    let message =
      GetCurrentRequest
        { SearchQuery = "London"
          ApiKey = "0238369d03394013894122527240401"
          Language = Some "en" }

    actor <! message
    this.ExpectMsg<GetCurrentMessage>()


  [<Fact>]
  member this.TestCancelRequest() =
    let actor =
      select "akka://weather-system/user/get-current-weather-actor" weatherSystem

    let cancelMessage = CancelRequest

    actor <! cancelMessage

    this.Within(
      this.Dilated(TimeSpan.FromSeconds(5.0)),
      fun () ->
        let receivedMessage = this.ExpectMsg<GetCurrentMessage>()

        match receivedMessage with
        | CancelRequest -> ()
        | _ -> Assert.True(false, "Received unexpected message type.")

    )
