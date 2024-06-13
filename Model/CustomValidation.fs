module Model.CustomValidation

open System.Text.RegularExpressions
open System


let between (lower: int) (upper: int) invalid (i: int) = if i >= lower && i <= upper then Ok i else Error invalid

let matches (re : Regex) invalid (s: string) = if re.IsMatch s then Ok s else Error invalid

let nonEmpty invalid s = if String.IsNullOrWhiteSpace s then Error invalid else Ok s

let onlyLetters invalid s = if String.forall Char.IsLetter s then Ok s else Error invalid