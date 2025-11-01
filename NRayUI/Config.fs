namespace NRayUI

open Microsoft.Extensions.DependencyInjection
open NRayUI.Window
open Raylib_CSharp.Colors
open Raylib_CSharp.Rendering
open Raylib_CSharp.Windowing

// TODO: top-tier API for config UI (view, view models, services, etc.): Appears in the near future

type Config = {
    Window: WindowParameters
    Services: IServiceCollection
    // Resources: Resources
    Flags: ConfigFlags
    BlendMode: BlendMode
    BackgroundColor: Color
    IsDebugMode: bool
}

// type UIBuilder(window: WindowParameters) =
//     let mutable config: Config = {
//         Window = window
//         Services = ServiceCollection()
//         
//         
//     }