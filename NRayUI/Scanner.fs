module NRayUI.Scanner

let separator = [|' '; ';';|]

let scanSidesInput (input: string) =
    let values =
        input.Split(separator, System.StringSplitOptions.RemoveEmptyEntries)
        |> Array.map float32
    match values with
    | [|  |] ->
        Positioning.zeroSides
    | [| a |] ->
        Positioning.createSides a
    | [| a; b |] -> {
            Top = a
            Right = b
            Bottom = a
            Left = b
        }
    | [| a; b; c |] -> {
            Top = a
            Right = c
            Bottom = b
            Left = c
        }
    | [| a; b; c; d |] -> {
            Top = a
            Right = b
            Bottom = c
            Left = d
        }
    | _ -> 
        failwithf $"Invalid input: %s{input}. Expected format: 'top,right,bottom,left'"
        
let scanCornersInput (input: string) =
    let values =
        input.Split(separator, System.StringSplitOptions.RemoveEmptyEntries)
        |> Array.map float32
    match values with
    | [|  |] ->
        Positioning.zeroCorners
    | [| a |] ->
        Positioning.createCorners a
    | [| a; b |] -> {
            TopLeft = a
            TopRight = b
            BottomRight = a
            BottomLeft = b
        }
    | [| a; b; c |] -> {
            TopLeft = a
            TopRight = c
            BottomRight = b
            BottomLeft = c
        }
    | [| a; b; c; d |] -> {
            TopLeft = a
            TopRight = b
            BottomRight = c
            BottomLeft = d
        }
    | _ -> 
        failwithf $"Invalid input: %s{input}. Expected format: 'topLeft,topRight,bottomRight,bottomLeft'"