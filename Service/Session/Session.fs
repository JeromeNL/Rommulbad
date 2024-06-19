module Rommulbad.Session.Session

open Giraffe
open Rommulbad.Database
open Thoth.Json.Giraffe
open Rommulbad.Store
open Thoth.Json.Net
open Model.General
open Service.Session.Serializer
open Model.Session.Session

let addSession () : HttpHandler =
    fun next ctx ->
        task {
            let! session = ThothSerializer.ReadBody ctx decode

            match session with
            | Error errorMessage -> return! RequestErrors.BAD_REQUEST errorMessage next ctx
            | Ok { Name = CandidateName name; Deep = deep; Date = date; Minutes = (MinutesAmount minutes) } ->
                let store = ctx.GetService<Store>()

                // Insert the session with minutes converted to int
                InMemoryDatabase.insert (name, date) (name, deep, date, minutes) store.sessions
                |> ignore

                return! text "OK" next ctx
        }


let getSessions (name: string) : HttpHandler =
    fun next ctx ->
        task {
            let store = ctx.GetService<Store>()

            // Filter the sessions based on the candidate name and convert to Session type
            let sessions = 
                store.sessions 
                |> InMemoryDatabase.filter (fun (n, _, _, _) -> n = name)
                |> Seq.choose (fun (n, deep, date, minutes) ->
                    match (CandidateName.make n, MinutesAmount.make minutes) with
                    | (Ok name, Ok minutesAmount) ->
                        Some { Name = name; Deep = deep; Date = date; Minutes = minutesAmount }
                    | _ -> None)

            return! ThothSerializer.RespondJsonSeq sessions Session.encode next ctx
        }

let getTotalMinutes (name: string) : HttpHandler =
    fun next ctx ->
        task {
            let store = ctx.GetService<Store>()

            let total =
                InMemoryDatabase.filter (fun (n, _, _, _) -> n = name) store.sessions
                |> Seq.map (fun (_, _, _, a) -> a)
                |> Seq.sum

            return! ThothSerializer.RespondJson total Encode.int next ctx
        }


let getEligibleSessions (name: string, diploma: string) : HttpHandler =
    fun next ctx ->
        task {
            let store = ctx.GetService<Store>()

            let shallowOk =
                match diploma with
                | "A" -> true
                | _ -> false

            let minMinutes =
                match diploma with
                | "A" -> 1
                | "B" -> 10
                | _ -> 15

            let filter (n, d, _, a) = (n = name) && ((d || shallowOk) && (a >= minMinutes))

            let sessions = 
                store.sessions 
                |> InMemoryDatabase.filter filter
                |> Seq.choose (fun (n, deep, date, minutes) ->
                    match (CandidateName.make n, MinutesAmount.make minutes) with
                    | (Ok name, Ok minutesAmount) ->
                        Some { Name = name; Deep = deep; Date = date; Minutes = minutesAmount }
                    | _ -> None)

            return! ThothSerializer.RespondJsonSeq sessions Session.encode next ctx
        }


let getTotalEligibleMinutes (name: string, diploma: string) : HttpHandler =
    fun next ctx ->
        task {
            let store = ctx.GetService<Store>()

            let shallowOk =
                match diploma with
                | "A" -> true
                | _ -> false

            let minMinutes =
                match diploma with
                | "A" -> 1
                | "B" -> 10
                | _ -> 15

            let filter (n, d, _, a) = (d || shallowOk) && (a >= minMinutes)


            let total =
                InMemoryDatabase.filter filter store.sessions
                |> Seq.map (fun (_, _, _, a) -> a)
                |> Seq.sum

            return! ThothSerializer.RespondJson total Encode.int next ctx
        }
        
let routes: HttpHandler =
    choose
        [ POST >=> route "/candidate/session" >=> addSession()
          GET >=> routef "/candidate/%s/session" getSessions
          GET >=> routef "/candidate/%s/session/total" getTotalMinutes
          GET >=> routef "/candidate/%s/session/%s" getEligibleSessions
          GET >=> routef "/candidate/%s/session/%s/total" getTotalEligibleMinutes ]
