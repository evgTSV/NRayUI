module NRayUI.UIRendering

open System.Numerics
open NRayUI.Components.UIConfigurator
open NRayUI.Elements
open NRayUI.Elements.Panels
open NRayUI.Field
open NRayUI.Modifier
open NRayUI.Positioning
open Raylib_CSharp.Colors
open Raylib_CSharp

let elementSize (elem: #IWithLayout<'a>) =
    ()
    
let elementPosition
    (elem: #IWithLayout<'a>)
    (parent: Layout)
    (parentPos: Vector2)=
    let layout = elem.GetLayout
    match layout.Position with
    | Position.Absolute ->
        match parent.Position with
        | Position.Relative ->
            Vector2(layout.Offset.Left, layout.Offset.Top) + parentPos
        | _ ->
            Vector2(layout.Offset.Left, layout.Offset.Top)
    | Position.Relative ->
        Vector2(layout.Offset.Left, layout.Offset.Top) + parentPos
    | Static ->
        Vector2(layout.Offset.Left, layout.Offset.Top) + Vector2(10f)
    | Fixed ->
        Vector2(layout.Offset.Left, layout.Offset.Top)

let rec renderChild
    (child: 'a when 'a :> IElem)
    (parentPos: Vector2)=
    child.Render parentPos

let rec renderChildWithLayout
    (child: 'a when 'a :> IElem and IWithLayout<'a>)
    (parent: Layout)
    (parentPos: Vector2)=
    child.Render <| elementPosition child parent parentPos

let render (elem: 'a when 'a :> IElem and IWithLayout<'a>) =
    if (Windowing.Window.IsReady() |> not) then
        failwith "Window is not ready for rendering. Please ensure the window is initialized before rendering."
        
    renderChildWithLayout elem windowLayout (Vector2(0f, 0f))