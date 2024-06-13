module Model.Guardian.Guardian

open Model.Candidate.Candidate
open Model.General

type Guardian =
    { Id: GuardianIdentifier
      Name: PersonName
      Candidates: List<Candidate> }