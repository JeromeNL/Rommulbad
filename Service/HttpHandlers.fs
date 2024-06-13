module Rommulbad.HttpHandlers

open Giraffe
open Rommulbad.Candidate
open Rommulbad.Session
open Rommulbad.Guardian


let requestHandlers: HttpHandler =
    choose [
        Candidate.routes
        Session.routes
        Guardian.routes
    ]