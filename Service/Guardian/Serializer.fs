module Service.Guardian.Serializer

open Thoth.Json.Net
open Model.General
open Model.Guardian.Guardian


module Guardian =
    // Encode Guardian to JSON
    let encode: Encoder<Guardian> =
        fun guardian ->
            Encode.object
                [ "id", Encode.string (match guardian.Id with GuardianId id -> id)
                  "name", Encode.string (match guardian.Name with GuardianName name -> name) ]

     // Decode JSON to a Guardian (with validation)
    let decode: Decoder<Guardian> =
        Decode.object (fun get ->
            let id = get.Required.Field "id" Decode.string
            let name = get.Required.Field "name" Decode.string
            (id, name))
        |> Decode.andThen (fun (id, name) ->
            match GuardianId.make id, GuardianName.make name with
            | Ok guardianId, Ok personName ->
                Decode.succeed { Id = guardianId; Name = personName; Candidates = [] } // Ignore candidates
            | Error idErr, _ -> Decode.fail idErr
            | _, Error nameErr -> Decode.fail nameErr)