namespace NRayUI

open Microsoft.Extensions.DependencyInjection
open NRayUI.Window
open Raylib_CSharp.Colors
open Raylib_CSharp.Rendering
open Raylib_CSharp.Windowing

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