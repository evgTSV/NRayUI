namespace NRayUI

open System
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open NRayUI.Elements
open NRayUI.Modifier
open NRayUI.RenderBase
open NRayUI.Services
open NRayUI.StateService
open NRayUI.Window
open Raylib_CSharp.Colors
open Raylib_CSharp.Rendering
open Raylib_CSharp.Windowing

type DebugOptions = {
    RecordRenders: bool
    TrackStates: bool
    Logger: ILogger
}

type UIApp = {
    Window: WindowConfig
    ServiceProvider: ServiceProvider
    Flags: ConfigFlags
    BlendMode: BlendMode
    BackgroundColor: Color
    DebugOptions: DebugOptions option
} with

    interface IServiceProvider with
        member this.GetService(serviceType) =
            this.ServiceProvider.GetService serviceType

type UIBuilder() =
    let mutable app = {
        Window = {
            Title = "New Window"
            WindowSizePx = (500, 500)
        }
        ServiceProvider = Unchecked.defaultof<ServiceProvider>
        Flags =
            ConfigFlags.ResizableWindow
            ||| ConfigFlags.Msaa4XHint
            ||| ConfigFlags.VSyncHint
            ||| ConfigFlags.AlwaysRunWindow
        BlendMode = BlendMode.Alpha
        BackgroundColor = Color.White
        DebugOptions = None
    }

    let services =
        ServiceCollection()
            .AddScoped<UIStateService>()
            .AddScoped<IStateManager, StateManager>()

    member _.Services = services

    member this.WithWindow(window: WindowConfig) =
        app <- { app with Window = window }
        this

    member this.WithRenderingFlag(flags: ConfigFlags) =
        app <- { app with Flags = flags }
        this

    member this.SetBlendMode(mode: BlendMode) =
        app <- { app with BlendMode = mode }
        this

    member this.SetBackground(color: Color) =
        app <- { app with BackgroundColor = color }
        this

    member this.WithDebug(options: DebugOptions) =
        app <- { app with DebugOptions = Some options }
        this

    member _.Build() = {
        app with
            ServiceProvider = services.BuildServiceProvider()
    }

[<AutoOpen>]
module UIBuilder =

    /// Unary operator for ui building chain to ignore result
    let inline (~%) x = x |> ignore
