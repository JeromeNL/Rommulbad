module Model.Session.Session

open Model.General
open Thoth.Json.Net
open Thoth.Json.Giraffe
open System

type Session =
    { Deep: Boolean
      Date: DateTime
      Minutes: MinutesAmount }
    
    