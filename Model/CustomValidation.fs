module Model.CustomValidation

open System.Text.RegularExpressions
open System

/// Validate that the given string matches the provided regular expression or indicate otherwise by returning the
/// provided value.
let matches (re : Regex) invalid (s: string) = if re.IsMatch s then Ok s else Error invalid

// Validate that the given int is between the provided lower and upper bound (inclusive) or indicate otherwise by
// returning the provided value.
let between (lower: int) (upper: int) invalid (i: int) = if i >= lower && i <= upper then Ok i else Error invalid

/// Validate that the given string is not empty or indicate that the value is invalid by returning the provided value.
let nonEmpty invalid s = if String.IsNullOrWhiteSpace s then Error invalid else Ok s

/// Validate that the given string contains only letters or indicate otherwise by returning the provided
/// value.
let onlyLetters invalid s = if String.forall Char.IsLetter s then Ok s else Error invalid