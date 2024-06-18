module Service.Candidate.Serializer

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
            let nameResult = CandidateName.make (get.Required.Field "name" Decode.string)
            let guardianIdResult = GuardianIdentifier.make (get.Required.Field "guardian_id" Decode.string)
            let diplomaResult = Diploma.make (get.Required.Field "diploma" Decode.string)
            (nameResult, guardianIdResult, diplomaResult))
        |> Decode.andThen (fun (nameResult, guardianIdResult, diplomaResult) ->
            match (nameResult, guardianIdResult, diplomaResult) with
            | (Ok name, Ok guardianId, Ok diploma) ->
                Decode.succeed { Name = name; GuardianId = guardianId; Diploma = diploma }
            | (Error nameErr, _, _) -> Decode.fail nameErr
            | (_, Error guardianIdErr, _) -> Decode.fail guardianIdErr
            | (_, _, Error diplomaErr) -> Decode.fail diplomaErr)
