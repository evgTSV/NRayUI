module NRayUI.Scanner

let separator = [|' '; ';';|]

let scanSidesInput (input: string) =
    let values =
        input.Split(separator, System.StringSplitOptions.RemoveEmptyEntries)
        |> Array.map float32
    match values with
    | [|  |] -> Positioning.zeroSides
    | [| a |] -> Positioning.createSides a
    | [| a; b |] -> Positioning.createCustomSides a a b b
    | [| a; b; c |] -> Positioning.createCustomSides a c b b
    | [| a; b; c; d |] -> Positioning.createCustomSides a c d b
    | _ -> 
        failwithf $"Invalid input: %s{input}. Expected format: 'left,top,right,bottom'"