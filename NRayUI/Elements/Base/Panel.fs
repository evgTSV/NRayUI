namespace NRayUI.Elements

open System.Numerics
open NRayUI.Elements.Elem
open NRayUI.Field
open NRayUI.Modifier
open NRayUI.Positioning
open Raylib_CSharp.Transformations
open type Raylib_CSharp.Rendering.Graphics

[<AutoOpen>]
module Panel =
    type IPanel<'a> =
        inherit IWithBox<'a>
        abstract member GetChildren: IElem list
        abstract member SetChildren: IElem list -> 'a