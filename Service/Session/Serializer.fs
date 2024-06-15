module Service.Session.Serializer

open Thoth.Json.Net
open Model.Candidate.Candidate
open Model.Session.Session
open Model.General
open Model.Guardian.Guardian


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