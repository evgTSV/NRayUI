module NRayUI.UIRendering

open NRayUI.Components.UIConfigurator
open NRayUI.Elements
open NRayUI.Elements.Panels
open NRayUI.Field
open NRayUI.Modifier
open NRayUI.Positioning
open Raylib_CSharp.Colors
open Raylib_CSharp

let elementPosition (elem: #IWithLayout<'a>) =
    let layout = elem.GetLayout
    match layout.Position with
    | Position.Absolute -> failwith "todo"
    | Position.Relative -> failwith "todo"
    | Static -> failwith "todo"
    | Fixed -> failwith "todo"

let elementSize (elem: #IWithLayout<'a>) =
    ()

let rec renderChildren (children: IElem list) (parent: Layout) =
    ()

let render (elem: IElem) =
    if (Windowing.Window.IsReady() |> not) then
        failwith "Window is not ready for rendering. Please ensure the window is initialized before rendering."
        
    renderChildren [elem] windowLayout