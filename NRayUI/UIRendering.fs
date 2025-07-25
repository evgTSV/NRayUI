module NRayUI.UIRendering

open System.Numerics
open Microsoft.Extensions.DependencyInjection
open NRayUI.Camera
open NRayUI.Components.UIConfigurator
open NRayUI.Elements
open NRayUI.Elements.Panels
open NRayUI.Field
open NRayUI.Modifier
open NRayUI.Positioning
open Raylib_CSharp.Colors
open Raylib_CSharp
open type Raylib_CSharp.Rendering.Graphics

let rec renderChildWithLayout
    (child: 'a when 'a :> IElem and IWithLayout<'a>)
    (context: RenderingContext)=
    child.Render context

let render (elem: 'a when 'a :> IElem and IWithLayout<'a>) =
    if (Windowing.Window.IsReady() |> not) then
        failwith "Window is not ready for rendering. Please ensure the window is initialized before rendering."
        
    let camera = 
        ConfigureCamera {
            WindowSizePx = struct (Windowing.Window.GetScreenWidth(), Windowing.Window.GetScreenHeight())
            RenderTargetSize = struct (Windowing.Window.GetScreenWidth(), Windowing.Window.GetScreenHeight())
            ScaleFactor = 1.0f
        }
        
    BeginDrawing()
    BeginMode2D(camera)
    ClearBackground Color.RayWhite
        
    renderChildWithLayout elem {
        Camera = camera
        RenderTargetSize = Vector2.Zero
        CurrentPosition = Vector2.Zero + Vector2(elem.GetLayout.Margin.Left, elem.GetLayout.Margin.Top)
        ClipRegion = None
        IsDebugMode = false
        Services = ServiceCollection().BuildServiceProvider()
    }
    
    EndMode2D()
    EndDrawing()