open NRayUI
open NRayUI.Elements
open NRayUI.Examples.Counter
open NRayUI.Examples.InputViewer
open NRayUI.Examples.SimpleGame
open NRayUI.Examples.StackPanelExample
open NRayUI.Window
open NRayUI.UIRendering

let builder = UIBuilder()

%builder
    .WithWindow({
        Title = "NRayUI example"
        WindowSizePx = (800, 800)
    })
    
let app = builder.Build()

inputViewer
|> startRendering app
