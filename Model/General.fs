module Model.General

open System.Text.RegularExpressions
open Thoth.Json.Net


type GuardianIdentifier = private | GuardianIdentifier of string

/// Provides access to raw department identifier by pattern matching.
let (|GuardianIdentifier|) (GuardianIdentifier guardianIdentifier) = guardianIdentifier

[<RequireQualifiedAccess>]
module GuardianIdentifier =

    /// A valid GuadianIdentifier is three digits, dash, 4 capital letters
    let private validDepartmentIdentifier = Regex "^\d{3}-[A-Z]{4}$"

    let make rawIdentifier =
        rawIdentifier
        |> CustomValidation.matches validDepartmentIdentifier "ID must be in format: 123-ABCD"
        |> Result.map GuardianIdentifier
        
    let toString (GuardianIdentifier id) = id
    
    let fromString (s: string) : Result<GuardianIdentifier, string> =
        s |> CustomValidation.matches validDepartmentIdentifier "ID must be in format: 123-ABCD" |> Result.map GuardianIdentifier

    let encoder : Encoder<GuardianIdentifier> =
        fun (GuardianIdentifier id) -> Encode.string id

    let decoder : Decoder<GuardianIdentifier> =
        Decode.string
        |> Decode.andThen (fun s ->
            match fromString s with
            | Ok id -> Decode.succeed id
            | Error err -> Decode.fail err)

// Name format: "Joram Kwetters" GUARDIAN
type PersonName = private | PersonName of string

let (|PersonName|) (PersonName personName) = personName

[<RequireQualifiedAccess>]
module PersonName =

    /// Construct a valid perosn name from a raw string or indicate that the string is not a valid employee name.
    let make rawIdentifier =
        let validPersonName = Regex("^[a-zA-Z]+ [a-zA-Z]+$")
        rawIdentifier
        |> CustomValidation.nonEmpty "Person name may not be empty."
        |> Result.bind (CustomValidation.matches validPersonName "Person name must be in format: 'Firstname Lastname'")
        |> Result.map PersonName
   
    let toString (PersonName name) = name
    let validPersonName = Regex("^[a-zA-Z]+ [a-zA-Z]+$")

    let fromString (s: string) : Result<PersonName, string> =
        s |> CustomValidation.matches validPersonName "Invalid person name" |> Result.map PersonName

    let encoder : Encoder<PersonName> =
        fun (PersonName name) -> Encode.string name

    let decoder : Decoder<PersonName> =
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
        |> CustomValidation.nonEmpty "Person name may not be empty."
        |> Result.bind (CustomValidation.matches validPersonName "Person name must be in format: 'Firstname'")
        |> Result.map CandidateName
   
    let toString (CandidateName name) = name
    let validPersonName = Regex("^[a-zA-Z]+$")

    let fromString (s: string) : Result<CandidateName, string> =
        s |> CustomValidation.matches validPersonName "Invalid person name" |> Result.map CandidateName

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

    let make rawHours =
        rawHours
        |> CustomValidation.between minMinutesAmount maxMinutesAmount "Minutes must be between 0 and 30 minutes"
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
    let private validDiplomaValues = ["A"; "B"; "C"; ""]

    /// Construct a valid diploma from a raw string or indicate that the string is not a valid diploma.
    let make rawDiploma =
        if List.contains rawDiploma validDiplomaValues then
            Ok (Diploma rawDiploma)
        else
            Error "Diploma must be empty, A, B, or C"





























