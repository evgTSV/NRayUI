module NRayUI.Window

open Raylib_CSharp.Windowing
open type Raylib_CSharp.Windowing.Window

type WindowConfig = {
    Title: string
    WindowSizePx: struct(int * int)
} 

type Window with
    static member Init(config: WindowConfig) =
        let struct (width, height) = config.WindowSizePx
        Window.Init(width, height, config.Title)

type WindowParameters = {
    mutable WindowSizePx: struct(int * int)
    mutable RenderTargetSize: struct(int * int)
    mutable ScaleFactor: float32
} with
    static member DefaultWindowSizeWithoutScale: struct(int*int) = struct(600, 400)

    static member Init(): WindowParameters =
        let instance = {
            WindowSizePx = (0, 0)
            RenderTargetSize = (0, 0)
            ScaleFactor = 1.0f
        }
        instance.Update()
        instance

    member this.Update(): unit =
        let windowWidth, windowHeight = GetScreenWidth(), GetScreenHeight()
        let scaleFactor = GetScaleDPI()
        let struct (unscaledRenderTargetWidth, unscaledRenderTargetHeight) = WindowParameters.DefaultWindowSizeWithoutScale
        let renderTargetSize = struct(
            int(float32 unscaledRenderTargetWidth * scaleFactor.X),
            int(float32 unscaledRenderTargetHeight * scaleFactor.Y)
        )
        this.WindowSizePx <- struct(windowWidth, windowHeight)
        this.ScaleFactor <- max scaleFactor.X scaleFactor.Y
        this.RenderTargetSize <- renderTargetSize

    member this.Scale(units: float32): float32 =
        units * this.ScaleFactor