module Model.CustomValidation

open System.Text.RegularExpressions
open System


let timeIsBetween (lower: int) (upper: int) invalid (i: int) = if i >= lower && i <= upper then Ok i else Error invalid

let matchesRegex (re : Regex) invalid (s: string) = if re.IsMatch s then Ok s else Error invalid

let isNotEmpty invalid s = if String.IsNullOrWhiteSpace s then Error invalid else Ok s

let lettersOnly invalid s = if String.forall Char.IsLetter s then Ok s else Error invalid