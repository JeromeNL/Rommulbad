module Rommulbad.HttpHandlers

open Giraffe
open Rommulbad.Session
open Rommulbad.Candidate
open Rommulbad.Guardian

// The collection of routes
let httpHandlers: HttpHandler =
    choose [
        Candidate.routes
        Session.routes
        Guardian.routes
    ]