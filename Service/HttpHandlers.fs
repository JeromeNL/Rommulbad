module Rommulbad.HttpHandlers

open Giraffe
open Rommulbad.Candidate
open Rommulbad.Session


let requestHandlers: HttpHandler =
    choose [
        Candidate.routes
        Session.routes
    ]