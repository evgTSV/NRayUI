module NRayUI.Field

open Raylib_CSharp.Colors

[<Struct>]
type Orientation =
    | Horizontal
    | Vertical

[<Struct>]
type Point =
    | Point of int * int

    static member (+) (Point(x1, y1), Point(x2, y2)): Point = Point(x1 + x2, y1 + y2)
    static member (-) (Point(x1, y1), Point(x2, y2)): Point = Point(x1 - x2, y1 - y2)
    
    static member (%) (Point(x1, y1), Point(x2, y2)): Point =
        let modulo a b = ((a % b) + b) % b
        Point(modulo x1 x2, modulo y1 y2)

    member this.X: int = let (Point(x, _)) = this in x
    member this.Y: int = let (Point(_, y)) = this in y
    

[<Struct>]
type Box = {
    BackgroundColor: Color
    BorderColor: Color
    BorderWidth: float32
    CornerRadius: float32 * float32 * float32 * float32
}