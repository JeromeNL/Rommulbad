module Service.Guardian.Serializer

open Thoth.Json.Net
open Model.Candidate.Candidate
open Model.Session.Session
open Model.General
open Model.Guardian.Guardian


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
            (id, name))
        |> Decode.andThen (fun (id, name) ->
            match GuardianIdentifier.make id, PersonName.make name with
            | Ok guardianId, Ok personName ->
                Decode.succeed { Id = guardianId; Name = personName; Candidates = [] } // Ignore candidates
            | Error idErr, _ -> Decode.fail idErr
            | _, Error nameErr -> Decode.fail nameErr)