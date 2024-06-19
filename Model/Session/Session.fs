module Model.Session.Session

open System
open Model.General

// Type for Session
type Session =
    { Name: CandidateName
      Deep: Boolean
      Date: DateTime
      Minutes: MinutesAmount }
    
    