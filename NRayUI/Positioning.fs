module NRayUI.Positioning

type Sides = {
    Top: float32
    Right: float32
    Bottom: float32
    Left: float32
}

type Corners = {
    TopLeft: float32
    TopRight: float32
    BottomRight: float32
    BottomLeft: float32
}

module Alignment =
    [<Struct>]
    type Vertical =
        | Top
        | CenterV
        | Bottom
        | Custom of float32
        
        member this.ToFloat() =
            match this with
            | Top -> 0.0f
            | CenterV -> 0.5f
            | Bottom -> 1.0f
            | Custom(f) -> f

    [<Struct>]
    type Horizontal =
        | Start
        | CenterH
        | End
        | Custom of float32
        
        member this.ToFloat() =
            match this with
            | Start -> 0.0f
            | CenterH -> 0.5f
            | End -> 1.0f
            | Custom(f) -> f

    [<Struct>]
    type Alignment = {
        Vertical: Vertical
        Horizontal: Horizontal
    }
    
    let TopStart = { Vertical = Top; Horizontal = Start }
    let TopCenter = { Vertical = Top; Horizontal = CenterH }
    let TopEnd = { Vertical = Top; Horizontal = End }
    let CenterStart = { Vertical = CenterV; Horizontal = Start }
    let Center = { Vertical = CenterV; Horizontal = CenterH }
    let CenterEnd = { Vertical = CenterV; Horizontal = End }
    let BottomStart = { Vertical = Bottom; Horizontal = Start }
    let BottomCenter = { Vertical = Bottom; Horizontal = CenterH }
    let BottomEnd = { Vertical = Bottom; Horizontal = End }
    let CustomAlignment v h =
        { Vertical = Vertical.Custom v; Horizontal = Horizontal.Custom h }

type Offset = Sides
type Margin = Sides
type Padding = Sides

let createSides v = {
    Top = v
    Bottom = v
    Left = v
    Right = v
}

let createCorners v = {
    TopLeft = v
    TopRight = v
    BottomRight = v
    BottomLeft = v
}

let zeroSides = createSides 0.0f
let zeroCorners = createCorners 0.0f