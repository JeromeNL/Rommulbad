module Model.Guardian.Guardian

open Model.General
open Model.Candidate.Candidate

type Guardian =
    { Id: GuardianId
      Name: GuardianName
      Candidates: List<Candidate> }