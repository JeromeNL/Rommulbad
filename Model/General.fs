module Model.General

open System.Text.RegularExpressions
open Thoth.Json.Net


type GuardianId = private | GuardianId of string

/// Provides access to raw department identifier by pattern matching.
let (|GuardianId|) (GuardianId guardianIdentifier) = guardianIdentifier

[<RequireQualifiedAccess>]
module GuardianId =

    /// A valid GuardianId is three digits, dash, 4 capital letters
    let private validGuardianId = Regex "^\d{3}-[A-Z]{4}$"

    let make incomingId =
        incomingId
        |> CustomValidation.matchesRegex validGuardianId "ID must be in format: 123-ABCD"
        |> Result.map GuardianId
        
    let toString (GuardianId id) = id
    
    let fromString (s: string) : Result<GuardianId, string> =
        s |> CustomValidation.matchesRegex validGuardianId "ID must be in format: 123-ABCD" |> Result.map GuardianId

    let encoder : Encoder<GuardianId> =
        fun (GuardianId id) -> Encode.string id

    let decoder : Decoder<GuardianId> =
        Decode.string
        |> Decode.andThen (fun s ->
            match fromString s with
            | Ok id -> Decode.succeed id
            | Error err -> Decode.fail err)

// Name format: "Joram Kwetters" GUARDIAN
type GuardianName = private | GuardianName of string

let (|GuardianName|) (GuardianName personName) = personName

[<RequireQualifiedAccess>]
module GuardianName =

    /// Construct a valid perosn name from a raw string or indicate that the string is not a valid employee name.
    let make rawIdentifier =
        let validGuardianName = Regex("^[a-zA-Z]+ [a-zA-Z]+$")
        rawIdentifier
        |> CustomValidation.isNotEmpty "Guardian name may not be empty."
        |> Result.bind (CustomValidation.matchesRegex validGuardianName "Guardian name must be in format: 'Firstname Lastname'")
        |> Result.map GuardianName
   
    let toString (GuardianName name) = name
    let validGuardianName = Regex("^[a-zA-Z]+ [a-zA-Z]+$")

    let fromString (s: string) : Result<GuardianName, string> =
        s |> CustomValidation.matchesRegex validGuardianName "Invalid guardian name" |> Result.map GuardianName

    let encoder : Encoder<GuardianName> =
        fun (GuardianName name) -> Encode.string name

    let decoder : Decoder<GuardianName> =
        Decode.string
        |> Decode.andThen (fun s ->
            match fromString s with
            | Ok name -> Decode.succeed name
            | Error err -> Decode.fail err)

// Name format: "Joram" CANDIDATE
type CandidateName = private | CandidateName of string

let (|CandidateName|) (CandidateName candidateName) = candidateName

[<RequireQualifiedAccess>]
module CandidateName =

    /// Construct a valid perosn name from a raw string or indicate that the string is not a valid employee name.
    let make rawIdentifier =
        let validPersonName = Regex("^[a-zA-Z]+$")
        rawIdentifier
        |> CustomValidation.isNotEmpty "Candidate name may not be empty."
        |> Result.bind (CustomValidation.matchesRegex validPersonName "Candidate name must be in format: 'Firstname'")
        |> Result.map CandidateName
   
    let toString (CandidateName name) = name
    let validPersonName = Regex("^[a-zA-Z]+$")

    let fromString (s: string) : Result<CandidateName, string> =
        s |> CustomValidation.matchesRegex validPersonName "Invalid candidate name" |> Result.map CandidateName

    let encoder : Encoder<CandidateName> =
        fun (CandidateName name) -> Encode.string name

    let decoder : Decoder<CandidateName> =
        Decode.string
        |> Decode.andThen (fun s ->
            match fromString s with
            | Ok name -> Decode.succeed name
            | Error err -> Decode.fail err)

type MinutesAmount = private | MinutesAmount of int

/// Provides access to raw minutes amount by pattern matching.
let (|MinutesAmount|) (MinutesAmount minutesAmount) = minutesAmount

[<RequireQualifiedAccess>]
module MinutesAmount =
    let private minMinutesAmount = 0
    let private maxMinutesAmount = 30

    let make rawMinutes =
        rawMinutes
        |> CustomValidation.timeIsBetween minMinutesAmount maxMinutesAmount "Minutes must be between 0 and 30 minutes"
        |> Result.map MinutesAmount


// Diploma
// Options: Nothing, A, B, C

/// The diploma type
type Diploma = private | Diploma of string

/// Provides access to raw diploma value by pattern matching.
let (|Diploma|) (Diploma diploma) = diploma

[<RequireQualifiedAccess>]
module Diploma =

    /// Valid diploma values
    let private validDiplomaOptions = ["A"; "B"; "C"; ""]

    /// Construct a valid diploma from a raw string or indicate that the string is not a valid diploma.
    let make rawDiploma =
        if List.contains rawDiploma validDiplomaOptions then
            Ok (Diploma rawDiploma)
        else
            Error "Diploma must be A, B, C (or empty)"





























