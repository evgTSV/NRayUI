
open System
open System.Numerics
open NRayUI
open NRayUI.Components.UIConfigurator
open NRayUI.Elements
open NRayUI.Elements.Panels
open NRayUI.Examples
open NRayUI.Examples.InputViewer
open NRayUI.Examples.SimpleGame
open NRayUI.Examples.StackPanelExample
open NRayUI.Field
open NRayUI.Icons
open NRayUI.Input
open NRayUI.Modifier
open NRayUI.RenderBase
open NRayUI.Window
open NRayUI.Positioning
open NRayUI.UIRendering
open Raylib_CSharp.Colors
open Raylib_CSharp.Interact
open type Raylib_CSharp.Rendering.Graphics
open Raylib_CSharp.Transformations

let builder = UIBuilder()

%builder
    .WithWindow({
        Title = "NRayUI example"
        WindowSizePx = (800, 800)
    })
    
let app = builder.Build()

myStackPanel |> startRendering app
