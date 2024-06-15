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
