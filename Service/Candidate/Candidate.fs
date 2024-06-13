module Rommulbad.Candidate.Candidate

open Giraffe
open Model.Candidate.Candidate
open Rommulbad.Database
open Thoth.Json.Giraffe
open Service.Serializers
open Rommulbad.Store
open Model.General
open System

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
        
let addCandidate : HttpHandler =
    fun next ctx ->
        task {
            let! candidateResult = ThothSerializer.ReadBody ctx Candidate.decode

            match candidateResult with
            | Error errorMessage -> return! RequestErrors.BAD_REQUEST errorMessage next ctx
            | Ok candidate ->
                let store = ctx.GetService<Store>()

                // Convert Candidate fields to strings
                let nameStr = PersonName.toString candidate.Name
                let guardianIdStr = GuardianIdentifier.toString candidate.GuardianId
                let diplomaStr = match candidate.Diploma with Diploma diploma -> diploma
                let currentDateTime = DateTime.Now

                // Prepare the value to insert
                let value = (nameStr, currentDateTime, guardianIdStr, "")
                
                // Insert the candidate
                match InMemoryDatabase.insert nameStr value store.candidates with
                | Ok () -> return! json candidate next ctx
                | Error (UniquenessError msg) -> return! RequestErrors.BAD_REQUEST msg next ctx
        }
        

let routes: HttpHandler =
    choose
        [ GET >=> route "/candidate" >=> getCandidates
          GET >=> routef "/candidate/%s" getCandidate
          POST >=> route "/candidate" >=> addCandidate
           ]