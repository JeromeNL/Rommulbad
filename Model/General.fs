module Model.General

open System.Text.RegularExpressions
open Thoth.Json.Net



// Guardian
// ID format: 123-ABCD
/// Every department stored is identifier by a unique identifier.
type GuardianIdentifier = private | GuardianIdentifier of string

/// Provides access to raw department identifier by pattern matching.
let (|GuardianIdentifier|) (GuardianIdentifier guardianIdentifier) = guardianIdentifier

[<RequireQualifiedAccess>]
module GuardianIdentifier =

    /// A valid GuadianIdentifer is three digits, dash, 4 capital letters
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

// Name format: "Joram Kwetters"
type PersonName = private | PersonName of string


let (|PersonName|) (PersonName personName) = personName

[<RequireQualifiedAccess>]
module PersonName =

    /// Construct a valid perosn name from a raw string or indicate that the string is not a valid employee name.
    let make rawIdentifier =
        rawIdentifier
        |> CustomValidation.nonEmpty "Person name may not be empty."
        |> Result.bind (CustomValidation.onlyLetters "Person name may contain only letters.")
        |> Result.map PersonName
   
// Candidates from same Guardian can't have same name
    let toString (PersonName name) = name
    let validPersonName = Regex("^[a-zA-Z ]+$")

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




// Minutes in session: 0 <= x <= 30

/// The amount of hours.
type MinutesAmount = private | MinutesAmount of int

/// Provides access to raw hours amount by pattern matching.
let (|MinutesAmount|) (MinutesAmount minutesAmount) = minutesAmount

[<RequireQualifiedAccess>]
module MinutesAmount =

    /// Amount cannot be negative or larger than 16
    let private minMinutesAmount = 0
    let private maxMinutesAmount = 30

    /// Construct a valid hours amount from a raw int or indicate that the int is not a valid hours amount.
    let make rawHours =
        rawHours
        |> CustomValidation.between minMinutesAmount maxMinutesAmount "Hours must be between 0 and 30 minutes"
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





























