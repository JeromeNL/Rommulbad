module Model.Session.Session

open System
open Model.General

type Session =
    { Deep: Boolean
      Date: DateTime
      Minutes: MinutesAmount }
    
    