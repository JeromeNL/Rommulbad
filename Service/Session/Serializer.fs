module Service.Session.Serializer

open Thoth.Json.Net
open Model.Session.Session
open Model.General


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
            let date = get.Required.Field "date" Decode.datetimeUtc
            let amount = get.Required.Field "amount" Decode.int
            (deep, date, amount))
        |> Decode.andThen (fun (deep, date, amount) ->
            match MinutesAmount.make amount with
            | Ok minutesAmount -> Decode.succeed { Deep = deep; Date = date; Minutes = minutesAmount }
            | Error err -> Decode.fail err)