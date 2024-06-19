module Rommulbad.Candidate.Candidate

open Giraffe
open Model.Candidate.Candidate
open Rommulbad.Database
open Thoth.Json.Giraffe
open Rommulbad.Store
open Model.General
open System
open Model.Candidate
open Service.Candidate.Serializer

// Get all (valid) candidates
let getCandidates: HttpHandler =
    fun next ctx ->
        task {
            let store = ctx.GetService<Store>()

            let candidates =
                InMemoryDatabase.all store.candidates
                |> Seq.choose (fun (name, _, gId, dpl) ->
                    match (CandidateName.make name, GuardianId.make gId, Diploma.make dpl) with
                    | (Ok name, Ok gId, Ok dpl) ->
                        Some { Candidate.Name = name; GuardianId = gId; Diploma = dpl }
                    | _ -> None)

            return! ThothSerializer.RespondJsonSeq candidates Candidate.encode next ctx
        }

// Get one candidate
let getCandidate (name: string) : HttpHandler =
    fun next ctx ->
        task {
            let store = ctx.GetService<Store>()

            let candidate = InMemoryDatabase.lookup name store.candidates

            match candidate with
            | None -> return! RequestErrors.NOT_FOUND "Candidate not found!" next ctx
            | Some(name, _, gId, dpl) ->
                match (CandidateName.make name, GuardianId.make gId, Diploma.make dpl) with
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
        
// Add a new candidate by Name and GuardianId
let addCandidate : HttpHandler =
    fun next ctx ->
        task {
            let! candidateResult = ThothSerializer.ReadBody ctx Candidate.decode

            match candidateResult with
            | Error errorMessage -> return! RequestErrors.BAD_REQUEST errorMessage next ctx
            | Ok candidate ->
                let store = ctx.GetService<Store>()

                // Convert Candidate fields to strings
                let nameStr = CandidateName.toString candidate.Name
                let guardianIdStr = GuardianId.toString candidate.GuardianId
                let currentDateTime = DateTime.Now

                // Prepare the value to insert
                let value = (nameStr, currentDateTime, guardianIdStr, "")
                
                // Insert the candidate
                match InMemoryDatabase.insert nameStr value store.candidates with
                | Ok () -> return! json candidate next ctx
                | Error (UniquenessError msg) -> return! RequestErrors.BAD_REQUEST msg next ctx
        }
        
        
 // Function to check if candidate can get the diploma
let canGetDiploma diploma sessions =
    let totalMinutes = sessions |> List.sumBy (fun (_, _, _, min) -> min)
    let validSessions = sessions |> List.filter (fun (_, _, _, min) -> min >= match diploma with
                                                                              | "A" -> 1
                                                                              | "B" -> 10
                                                                              | "C" -> 15
                                                                              | _ -> Int32.MaxValue)
    match diploma with
    | "A" -> totalMinutes >= 120 && validSessions |> List.sumBy (fun (_, _, _, min) -> min) >= 120
    | "B" -> totalMinutes >= 150 && validSessions |> List.sumBy (fun (_, _, _, min) -> min) >= 150
    | "C" -> totalMinutes >= 180 && validSessions |> List.sumBy (fun (_, _, _, min) -> min) >= 180
    | _ -> false
        
// Update the current (highest) diploma of a candidate (with check)
let updateCandidateDiploma : HttpHandler =
    fun next ctx ->
        task {
            let! candidateUpdateResult = ThothSerializer.ReadBody ctx Candidate.decode

            match candidateUpdateResult with
            | Error errorMessage -> return! RequestErrors.BAD_REQUEST errorMessage next ctx
            | Ok candidateUpdate ->
                let store = ctx.GetService<Store>()

                // Convert Candidate fields to strings
                let nameStr = Model.General.CandidateName.toString candidateUpdate.Name
                let newDiplomaStr = match candidateUpdate.Diploma with Diploma diploma -> diploma
                
                // Retrieve the existing candidate sessions
                let candidateSessions = 
                    InMemoryDatabase.filter (fun (n, _, _, _) -> n = nameStr) store.sessions
                    |> Seq.toList
                
                // Check if candidate is eligible for the new diploma
                if canGetDiploma newDiplomaStr candidateSessions then
                    // Retrieve the existing candidate
                    match InMemoryDatabase.lookup nameStr store.candidates with
                    | Some (n, dateTime, guardianId, _) ->
                        // Prepare the updated value
                        let updatedValue = (n, dateTime, guardianId, newDiplomaStr)
                        
                        // Update the candidate in the store
                        InMemoryDatabase.update nameStr updatedValue store.candidates
                        
                        // Return the updated candidate
                        return! json candidateUpdate next ctx
                    | None -> return! RequestErrors.BAD_REQUEST "Candidate not found" next ctx
                else
                    return! RequestErrors.BAD_REQUEST "Candidate does not meet the requirements for the new diploma" next ctx
        }

// Routes of candidate
let routes: HttpHandler =
    choose
        [ GET >=> route "/candidate" >=> getCandidates
          GET >=> routef "/candidate/%s" getCandidate
          POST >=> route "/candidate" >=> addCandidate
          POST >=> route "/candidate/diploma" >=> updateCandidateDiploma
           ]