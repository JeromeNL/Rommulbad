module Model.Candidate.Candidate

open Model.General

type Candidate =
    { Name: CandidateName
      GuardianId: GuardianId
      Diploma: Diploma }