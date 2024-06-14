module Service.Serializers

open Thoth.Json.Net

open Model.Candidate.Candidate
open Model.Session.Session
open Model.General
open Model.Guardian.Guardian


module Candidate =
    let encode: Encoder<Candidate> =
        fun candidate ->
            Encode.object
                [ "name", Encode.string (match candidate.Name with CandidateName name -> name)
                  "guardian_id", Encode.string (match candidate.GuardianId with GuardianIdentifier id -> id)
                  "diploma", Encode.string (match candidate.Diploma with Diploma diploma -> diploma) ]

    let decode: Decoder<Candidate> =
        Decode.object (fun get ->
            match (CandidateName.make (get.Required.Field "name" Decode.string),
                   GuardianIdentifier.make (get.Required.Field "guardian_id" Decode.string),
                   Diploma.make (get.Required.Field "diploma" Decode.string)) with
            | (Ok name, Ok guardianId, Ok diploma) ->
                { Name = name
                  GuardianId = guardianId
                  Diploma = diploma }
            | (Error err, _, _) -> failwithf "Error decoding name: %s" err
            | (_, Error err, _) -> failwithf "Error decoding guardian_id: %s" err
            | (_, _, Error err) -> failwithf "Error decoding diploma: %s" err)

module Session =
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

module Guardian =
    let encode: Encoder<Guardian> =
        fun guardian ->
            Encode.object
                [ "id", Encode.string (match guardian.Id with GuardianIdentifier id -> id)
                  "name", Encode.string (match guardian.Name with PersonName name -> name) ]

    let decode: Decoder<Guardian> =
        Decode.object (fun get ->
            let id = get.Required.Field "id" Decode.string
            let name = get.Required.Field "name" Decode.string

            match GuardianIdentifier.make id, PersonName.make name with
            | Ok guardianId, Ok personName ->
                { Id = guardianId
                  Name = personName
                  Candidates = [] }  // Ignore candidates
            | Error idErr, _ -> failwith idErr
            | _, Error nameErr -> failwith nameErr )