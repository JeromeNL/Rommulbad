module Model.Guardian.Guardian

open Model.General
open Model.Candidate.Candidate

type Guardian =
    { Id: GuardianIdentifier
      Name: PersonName
      Candidates: List<Candidate> }