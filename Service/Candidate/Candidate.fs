module Rommulbad.Candidate.Candidate

open Giraffe
open Model.Candidate.Candidate
open Rommulbad.Database
open Thoth.Json.Giraffe
open Service.Serializers
open Rommulbad.Store
open Model.General

let getCandidates: HttpHandler =
    fun next ctx ->
        task {
            let store = ctx.GetService<Store>()

            let candidates =
                InMemoryDatabase.all store.candidates
                |> Seq.choose (fun (name, _, gId, dpl) ->
                    match (PersonName.make name, GuardianIdentifier.make gId, Diploma.make dpl) with
                    | (Ok name, Ok gId, Ok dpl) ->
                        Some { Candidate.Name = name; GuardianId = gId; Diploma = dpl }
                    | _ -> None)

            return! ThothSerializer.RespondJsonSeq candidates Candidate.encode next ctx
        }


let getCandidate (name: string) : HttpHandler =
    fun next ctx ->
        task {
            let store = ctx.GetService<Store>()

            let candidate = InMemoryDatabase.lookup name store.candidates

            match candidate with
            | None -> return! RequestErrors.NOT_FOUND "Employee not found!" next ctx
            | Some(name, _, gId, dpl) ->
                match (PersonName.make name, GuardianIdentifier.make gId, Diploma.make dpl) with
                | (Ok name, Ok gId, Ok dpl) ->
                    return!
                        ThothSerializer.RespondJson
                            { Name = name
                              GuardianId = gId
                              Diploma = dpl }
                            Candidate.encode
                            next
                            ctx
                | _ -> return! RequestErrors.BAD_REQUEST "Invalid candidate data" next ctx
        }

        
        
let routes: HttpHandler =
    choose
        [ GET >=> route "/candidate" >=> getCandidates
          GET >=> routef "/candidate/%s" getCandidate
           ]