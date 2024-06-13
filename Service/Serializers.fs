module Service.Serializers

open Thoth.Json.Net

open Model.Candidate.Candidate
open Model.Session.Session
open Model.General

module Candidate =
    let encode: Encoder<Candidate> =
        fun candidate ->
            Encode.object
                [ "name", Encode.string (match candidate.Name with PersonName name -> name)
                  "guardian_id", Encode.string (match candidate.GuardianId with GuardianIdentifier id -> id)
                  "diploma", Encode.string (match candidate.Diploma with Diploma diploma -> diploma) ]

    let decode: Decoder<Candidate> =
        Decode.object (fun get ->
            match (PersonName.make (get.Required.Field "name" Decode.string),
                   GuardianIdentifier.make (get.Required.Field "guardian_id" Decode.string),
                   Diploma.make (get.Required.Field "diploma" Decode.string)) with
            | (Ok name, Ok guardianId, Ok diploma) ->
                { Name = name
                  GuardianId = guardianId
                  Diploma = diploma }
            | (Error err, _, _) -> failwithf "Error decoding name: %s" err
            | (_, Error err, _) -> failwithf "Error decoding guardian_id: %s" err
            | (_, _, Error err) -> failwithf "Error decoding diploma: %s" err)

/// Swimming session registered on a specific date
///
/// A Swimming session can be in the deep or shallow pool
/// Minutes cannot be negative or larger than 30
/// Only the year, month and date of Date are used.


module Session =
    open Thoth.Json.Net
    open Model.General

    let encode: Encoder<Session> =
        fun session ->
            Encode.object
                [ "deep", Encode.bool session.Deep
                  "date", Encode.datetime session.Date
                  "amount", Encode.int (match session.Minutes with MinutesAmount minutes -> minutes) ]

    let decode: Decoder<Session> =
        Decode.object (fun get ->
            let deep = get.Required.Field "deep" Decode.bool
            let date = get.Required.Field "date" Decode.datetime
            let amount = get.Required.Field "amount" Decode.int

            match MinutesAmount.make amount with
            | Ok minutesAmount -> { Deep = deep; Date = date; Minutes = minutesAmount }
            | Error err -> failwith err)  



/// A guardian has an Id (3 digits followed by a dash and 4 letters),
/// a Name (only letters and spaces, but cannot contain two or more consecutive spaces),
/// and a list of Candidates (which may be empty)

