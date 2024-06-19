module Model.Guardian.Guardian

open Model.General
open Model.Candidate.Candidate

// Type for Guardian
type Guardian =
    { Id: GuardianId
      Name: GuardianName
      Candidates: List<Candidate> }