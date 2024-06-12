module Model.Session.Session

open Thoth.Json.Net
open Thoth.Json.Giraffe
open System

type Session =
    { Deep: bool
      Date: DateTime
      Minutes: int }
    
    