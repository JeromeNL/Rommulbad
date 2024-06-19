module Model.CustomValidation

open System.Text.RegularExpressions
open System

// Helper functions to validate rules

// Calculate duration between two times
let timeIsBetween (lower: int) (upper: int) invalid (i: int) = if i >= lower && i <= upper then Ok i else Error invalid

// Match a regex
let matchesRegex (re : Regex) invalid (s: string) = if re.IsMatch s then Ok s else Error invalid

// Check whether a string is empty or not
let isNotEmpty invalid s = if String.IsNullOrWhiteSpace s then Error invalid else Ok s

// Check whether a string contains letters only or not
let lettersOnly invalid s = if String.forall Char.IsLetter s then Ok s else Error invalid