module NRayUI.UIRendering

open System.Numerics
open JetBrains.Lifetimes
open Microsoft.Extensions.DependencyInjection
open NRayUI.Camera
open NRayUI.Elements
open NRayUI.Loader
open NRayUI.Modifier
open NRayUI.RenderBase
open NRayUI.Window
open Raylib_CSharp.Colors
open Raylib_CSharp
open Raylib_CSharp.Rendering
open type Raylib_CSharp.Rendering.Graphics
open Raylib_CSharp.Windowing

type View<'a when 'a :> IElem and IWithLayout<'a>> = RenderingContext -> 'a

let inline renderPrologue (ctx: RenderingContext) =
    BeginDrawing()
    BeginMode2D(ctx.Camera)
    BeginBlendMode(BlendMode.Alpha)
    ClearBackground Color.RayWhite

let inline renderEpilogue () =
    EndBlendMode()
    EndMode2D()
    EndDrawing()

let inline renderChildWithLayout (child: View<'a>) (ctx: RenderingContext) = (child ctx).Render ctx

let render (view: View<'a>) (ctx: RenderingContext) =
    renderPrologue ctx
    renderChildWithLayout view ctx
    renderEpilogue ()

let startRendering (windowConfig: WindowConfig) (config: ConfigFlags) (view: View<'a>) =

    use lifetimeDef = new LifetimeDefinition()
    let lifetime = lifetimeDef.Lifetime

    Raylib.SetConfigFlags(config)
    Window.Init(windowConfig)

    if (Window.IsReady() |> not) then
        failwith
            "Window is not ready for rendering. Please ensure the window is initialized before rendering."

    let camera =
        ConfigureCamera {
            WindowSizePx = struct (Window.GetScreenWidth(), Window.GetScreenHeight())
            RenderTargetSize = struct (Window.GetScreenWidth(), Window.GetScreenHeight())
            ScaleFactor = 1.0f
        }

    let ctx = {
        Camera = camera
        RenderTargetSize = Vector2.Zero
        CurrentPosition = Vector2.Zero
        ScissorRegion = None
        IsDebugMode = false
        Resources = Resources(lifetime)
        Services = ServiceCollection().BuildServiceProvider()
    }

    let rec loop () =
        if not (Window.ShouldClose()) then
            render view ctx
            loop ()

    loop ()

    lifetimeDef.Terminate()

    Window.Close()
