module Rommulbad.Guardian.Guardian

open Giraffe
open Rommulbad.Database
open Thoth.Json.Giraffe
open Service.Serializers
open Rommulbad.Store
open Model.Guardian.Guardian
open Model.General
open Service.Guardian.Serializer

let addGuardian : HttpHandler =
    fun next ctx ->
        task {
            let! guardianResult = ThothSerializer.ReadBody ctx Guardian.decode

            match guardianResult with
            | Error errorMessage -> return! RequestErrors.BAD_REQUEST errorMessage next ctx
            | Ok guardian ->
                let store = ctx.GetService<Store>()

                // Convert GuardianIdentifier to string
                let idStr = GuardianIdentifier.toString guardian.Id
                let nameStr = PersonName.toString guardian.Name

                // Prepare the value to insert
                let value = (idStr, nameStr)

                // Insert the guardian using the string version of GuardianIdentifier
                match InMemoryDatabase.insert idStr value store.guardians with
                | Ok () -> return! json guardian next ctx
                | Error (UniquenessError msg) -> return! RequestErrors.BAD_REQUEST msg next ctx
        }


let getGuardians: HttpHandler =
    fun next ctx ->
        task {
            let store = ctx.GetService<Store>()

            let guardians =
                InMemoryDatabase.all store.guardians
                |> Seq.choose (fun (id, name) ->
                    match (GuardianIdentifier.make id, PersonName.make name) with
                    | (Ok id, Ok name) ->
                        Some { Guardian.Id = id; Name = name; Candidates = [] }
                    | _ -> None)

            let response = guardians |> Seq.toList
            return! json response next ctx
        }
        



let routes: HttpHandler =
    choose
        [ POST >=> route "/guardian" >=> addGuardian
          GET >=> route "/guardian" >=> getGuardians
         ]