open NRayUI
open NRayUI.Examples.SimpleGame
open NRayUI.Examples.StackPanelExample
open NRayUI.Window
open NRayUI.UIRendering
open type Raylib_CSharp.Rendering.Graphics

let builder = UIBuilder()

%builder
    .WithWindow({
        Title = "NRayUI example"
        WindowSizePx = (800, 800)
    })
    
let app = builder.Build()

myStackPanel |> startRendering app
