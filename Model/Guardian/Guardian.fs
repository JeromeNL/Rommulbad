module Model.Guardian.Guardian

open Model.Candidate.Candidate

type Guardian =
    { Id: string
      Name: string
      Candidates: List<Candidate> }