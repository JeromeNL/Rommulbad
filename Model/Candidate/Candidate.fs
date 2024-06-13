module Model.Candidate.Candidate

open Model.General

type Candidate =
    { Name: PersonName
      GuardianId: GuardianIdentifier
      Diploma: Diploma }