module DVar.TestTask.Akka.Systems

open Akka.Configuration
open Akka.FSharp

let weatherSystem = ConfigurationFactory.Default() |> System.create "weather-system"
