module NRayUI.UIRendering

open System.Numerics
open JetBrains.Lifetimes
open NRayUI.Camera
open NRayUI.Input
open NRayUI.Input.UserInput
open NRayUI.Loader
open NRayUI.RenderBase
open NRayUI.Time
open NRayUI.Window
open Raylib_CSharp.Colors
open Raylib_CSharp
open Raylib_CSharp.Rendering
open type Raylib_CSharp.Rendering.Graphics
open Raylib_CSharp.Windowing

let inline renderPrologue (ctx: RenderingContext) =
    BeginDrawing()
    BeginMode2D(ctx.Camera)
    BeginBlendMode(BlendMode.Alpha)
    ClearBackground Color.RayWhite

let inline renderEpilogue () =
    EndBlendMode()
    EndMode2D()
    EndDrawing()

let inline update (uCtx: UpdateContext) (view: View<'a>) =

    let upd (ctx: UpdateContext) =
        let input = handleInput (ctx.TickEngine.LastProcessedTickTime)

        {
            ctx with
                Input = input.Inputs |> Array.append ctx.Input
        }

    let uCtx, tickEngine = uCtx.TickEngine.Update(FrameTime.Now(), uCtx, upd)

    let uCtx = { uCtx with TickEngine = tickEngine }

    view uCtx

let inline renderView (rCtx: RenderingContext) (uCtx: UpdateContext) (view: View<'a>) =

    let updated = view |> update uCtx
    updated.Render rCtx

let render (rCtx: RenderingContext) (uCtx: UpdateContext) (view: View<'a>) =

    renderPrologue rCtx
    view |> renderView rCtx uCtx
    renderEpilogue ()

let startRendering (app: UIApp) (view: View<'a>) =

    use lifetimeDef = new LifetimeDefinition()
    let lifetime = lifetimeDef.Lifetime

    Raylib.SetConfigFlags(app.Flags)
    Window.Init(app.Window)

    if (Window.IsReady() |> not) then
        failwith
            "Window is not ready for rendering. Please ensure the window is initialized before rendering."

    let camera =
        ConfigureCamera {
            WindowSizePx = struct (Window.GetScreenWidth(), Window.GetScreenHeight())
            RenderTargetSize = struct (Window.GetScreenWidth(), Window.GetScreenHeight())
            ScaleFactor = 1.0f
        }

    let res = Resources(lifetime)

    let rCtx = {
        Camera = camera
        RenderTargetSize = Vector2.Zero
        CurrentPosition = Vector2.Zero
        ScissorRegion = None
        IsDebugMode = false
        Resources = res
        ServiceProvider = app.ServiceProvider // TODO: Use different scopes for render and update
    }

    let uCtx = {
        Input = [||]
        Resources = res
        ServiceProvider = app.ServiceProvider // TODO: Use different scopes for render and update
        TickEngine = TickEngine.Create(FrameTime.Now())
    }

    let rec loop () =
        if not (Window.ShouldClose()) then
            view |> render rCtx uCtx
            loop ()

    loop ()

    lifetimeDef.Terminate()

    Window.Close()
