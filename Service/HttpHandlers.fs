module Rommulbad.HttpHandlers

open Giraffe
open Rommulbad.Candidate
open Rommulbad.Session
open Rommulbad.Guardian
open Service.Serializers


let requestHandlers: HttpHandler =
    choose [
        Candidate.routes
        Session.routes
        Guardian.routes
    ]