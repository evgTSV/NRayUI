module NRayUI.Field

open System.Numerics
open NRayUI.Utils
open Raylib_CSharp.Transformations

[<Struct; RequireQualifiedAccess>]
type Orientation =
    | Horizontal
    | Vertical

/// <summary>
/// Find intersection of two rectangles
/// </summary>
/// <param name="rec1">First rectangle</param>
/// <param name="rec2">Second rectangle</param>
/// <returns>Rectangle that is intersection of two rectangles</returns>
/// <remarks>Some if intersection exists else None</remarks>
let inline (<&&>) (rec1: Rectangle) (rec2: Rectangle) =
    let x1 = max rec1.X rec2.X
    let y1 = max rec1.Y rec2.Y
    let x2 = min (rec1.X + rec1.Width) (rec2.X + rec2.Width)
    let y2 = min (rec1.Y + rec1.Height) (rec2.Y + rec2.Height)

    let inline intersectionExists () = x2 > x1 && y2 > y1

    if intersectionExists () then
        Some <| Rectangle(x1, y1, x2 - x1, y2 - y1)
    else
        None

/// <summary>
/// Find intersection of two rectangles
/// </summary>
/// <param name="rec1">First rectangle</param>
/// <param name="rec2">Second rectangle</param>
/// <returns>Rectangle that is intersection of two rectangles</returns>
/// <remarks>Rec1 is top-priority, if there isn't intersection then returns Rec1</remarks>
let inline (<!&&>) (rec1: Rectangle) (rec2: Rectangle) =
    rec1 <&&> rec2 |> Option.defaultValue rec1

/// <summary>
/// Find intersection of two rectangles
/// </summary>
/// <param name="rec1">First rectangle</param>
/// <param name="rec2">Second rectangle</param>
/// <returns>Rectangle that is intersection of two rectangles</returns>
/// <remarks>Rec2 is top-priority, if there isn't intersection then returns Rec2</remarks>
let inline (<&&!>) (rec1: Rectangle) (rec2: Rectangle) = rec2 <!&&> rec1

/// <summary>
/// Find intersection of two rectangles
/// </summary>
/// <param name="rec1">First rectangle</param>
/// <param name="rec2">Second rectangle or None</param>
/// <returns>Rectangle that is intersection of two rectangles or None if there isn't intersection</returns>
/// <remarks>If rec2 is None then intersection equals rec1 else equals intersection with rec2 priority</remarks>
let inline (<&&?>) (rec1: Rectangle) (rec2: Rectangle option) =
    match rec2 with
    | Some rec2 -> rec1 <&&!> rec2 |> Some
    | None -> Some rec1
