
open NRayUI
open NRayUI.Components.UIConfigurator
open NRayUI.Elements.Elem
open NRayUI.Elements.Panels
open NRayUI.Field
open NRayUI.Modifier
open NRayUI.Positioning
open Raylib_CSharp
open Raylib_CSharp.Colors
open type Raylib_CSharp.Rendering.Graphics
open Raylib_CSharp.Windowing

let test =
    StackPanel.create [
        LayoutSet.modifiers [
            position Relative
            top 100f
            left 200f
            margin { Top = 10f; Right = 20f; Bottom = 10f; Left = 20f }
            padding { Top = 5f; Right = 5f; Bottom = 5f; Left = 5f }
        ]
        LayoutSet.height 500f >> LayoutSet.width 250f
        BoxSet.backgroundColor Color.Blue
        BoxSet.borderWidth 50f
        BoxSet.smoothness 10
        BoxSet.cornerRadius { TopLeft = 0f; TopRight = 1.f; BottomLeft = 0f; BottomRight = 0.1f }
        StackPanelSet.orientation Orientation.Vertical
        StackPanelSet.children [
            Div.create [
                LayoutSet.modifiers [
                    position Relative
                    top 5f
                    left 5f
                    margin { Top = 5f; Right = 5f; Bottom = 5f; Left = 5f }
                ]
                LayoutSet.height 20f >> LayoutSet.width 20f
                BoxSet.backgroundColor Color.Red
            ]
        ]
    ]
    
let config =
     ConfigFlags.ResizableWindow |||
     ConfigFlags.Msaa4XHint |||
     ConfigFlags.VSyncHint |||
     ConfigFlags.AlwaysRunWindow
    
Raylib.SetConfigFlags(config)
Window.Init(800, 800, "NRayUI Example")

let rec mainLoop () =
    BeginDrawing()
    ClearBackground Color.RayWhite
    UIRendering.render test
    EndDrawing()
    
    if not (Window.ShouldClose()) then
        mainLoop ()
        
mainLoop()