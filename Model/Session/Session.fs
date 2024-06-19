module Model.Session.Session

open System
open Model.General

// Type for Session
type Session =
    { 
      Deep: Boolean
      Date: DateTime
      Minutes: MinutesAmount }
    
    