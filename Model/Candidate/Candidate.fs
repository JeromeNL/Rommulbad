module Model.Candidate.Candidate

open Model.General

// Type for Candidate 
type Candidate =
    { Name: CandidateName
      GuardianId: GuardianId
      Diploma: Diploma }