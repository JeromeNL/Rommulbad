module Model.General

open System.Text.RegularExpressions
open Thoth.Json.Net


// Type for GuardianID 
type GuardianId = private | GuardianId of string

// Gets ID as string from the GuardianId
let (|GuardianId|) (GuardianId guardianIdentifier) = guardianIdentifier

// All is required
[<RequireQualifiedAccess>]
module GuardianId =
    // Regex to validate GuardianId: 123-ABCD
    let private validGuardianId = Regex "^\d{3}-[A-Z]{4}$"

    // Validate it
    let make incomingId =
        incomingId
        |> CustomValidation.matchesRegex validGuardianId "ID must be in format: 123-ABCD"
        |> Result.map GuardianId
        
    // GuardianId to String
    let toString (GuardianId id) = id
    
    // String to GuardianId
    let fromString (s: string) : Result<GuardianId, string> =
        s |> CustomValidation.matchesRegex validGuardianId "ID must be in format: 123-ABCD" |> Result.map GuardianId

    // Encode GuardianId to JSON
    let encoder : Encoder<GuardianId> =
        fun (GuardianId id) -> Encode.string id

    // Decode GuardianId to Object
    let decoder : Decoder<GuardianId> =
        Decode.string
        |> Decode.andThen (fun s ->
            match fromString s with
            | Ok id -> Decode.succeed id
            | Error err -> Decode.fail err)


// Type for GuardianName
type GuardianName = private | GuardianName of string

// Gets name as string from the GuardianName
let (|GuardianName|) (GuardianName personName) = personName

[<RequireQualifiedAccess>]
module GuardianName =
    // Validate GuardianName with regex
    let validGuardianName = Regex("^[a-zA-Z]+ [a-zA-Z]+$")
    
    let make incomingName =
        incomingName
        |> CustomValidation.isNotEmpty "Guardian name may not be empty."
        |> Result.bind (CustomValidation.matchesRegex validGuardianName "Guardian name must be in format: 'Firstname Lastname'")
        |> Result.map GuardianName
   
    // GuardianName to String
    let toString (GuardianName name) = name

    // String to GuardianName
    let fromString (s: string) : Result<GuardianName, string> =
        s |> CustomValidation.matchesRegex validGuardianName "Invalid guardian name" |> Result.map GuardianName

    // Encode GuardianName
    let encoder : Encoder<GuardianName> =
        fun (GuardianName name) -> Encode.string name

    // Decode GuardianName
    let decoder : Decoder<GuardianName> =
        Decode.string
        |> Decode.andThen (fun s ->
            match fromString s with
            | Ok name -> Decode.succeed name
            | Error err -> Decode.fail err)


// Type for CandidateName 
type CandidateName = private | CandidateName of string

let (|CandidateName|) (CandidateName candidateName) = candidateName

[<RequireQualifiedAccess>]
module CandidateName =
    let validPersonName = Regex("^[a-zA-Z]+$")
   
    let make rawIdentifier =
        rawIdentifier
        |> CustomValidation.isNotEmpty "Candidate name may not be empty."
        |> Result.bind (CustomValidation.matchesRegex validPersonName "Candidate name must be in format: 'Firstname'")
        |> Result.map CandidateName
   
    // CandidateName to String
    let toString (CandidateName name) = name
    
    // String to CandidateName
    let fromString (s: string) : Result<CandidateName, string> =
        s |> CustomValidation.matchesRegex validPersonName "Invalid candidate name" |> Result.map CandidateName

    // Encode CandidateName 
    let encoder : Encoder<CandidateName> =
        fun (CandidateName name) -> Encode.string name

    // Decode CandidateName
    let decoder : Decoder<CandidateName> =
        Decode.string
        |> Decode.andThen (fun s ->
            match fromString s with
            | Ok name -> Decode.succeed name
            | Error err -> Decode.fail err)


// Type for MinutesAmount 
type MinutesAmount = private | MinutesAmount of int

let (|MinutesAmount|) (MinutesAmount minutesAmount) = minutesAmount

[<RequireQualifiedAccess>]
module MinutesAmount =
    let private minMinutesAmount = 0
    let private maxMinutesAmount = 30

    let make rawMinutes =
        rawMinutes
        |> CustomValidation.timeIsBetween minMinutesAmount maxMinutesAmount "Minutes must be between 0 and 30 minutes"
        |> Result.map MinutesAmount


// Type for Diploma 
type Diploma = private | Diploma of string

let (|Diploma|) (Diploma diploma) = diploma

[<RequireQualifiedAccess>]
module Diploma =
    
    // Diploma must be A, B, C or empty ("")
    let private validDiplomaOptions = ["A"; "B"; "C"; ""]
    
    let make rawDiploma =
        if List.contains rawDiploma validDiplomaOptions then
            Ok (Diploma rawDiploma)
        else
            Error "Diploma must be A, B, C (or empty)"

