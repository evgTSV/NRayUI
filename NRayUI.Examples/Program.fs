
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

let test =
    StackPanel.create [
        LayoutSet.modifiers [
            position Relative
            top 10f
            left 20f
            margin { Top = 10f; Right = 20f; Bottom = 10f; Left = 20f }
            padding { Top = 5f; Right = 5f; Bottom = 5f; Left = 5f }
        ]
        LayoutSet.height 50f >> LayoutSet.width 10f
        BoxSet.backgroundColor Color.Blue
        BoxSet.cornerRadius { TopLeft = 5f; TopRight = 5f; BottomLeft = 5f; BottomRight = 5f }
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
    
Windowing.Window.Init(500, 500, "NRayUI Example")

let rec mainLoop () =
    BeginDrawing()
    ClearBackground Color.RayWhite
    UIRendering.render test
    EndDrawing()
    
    if not (Windowing.Window.ShouldClose()) then
        mainLoop ()
        
mainLoop()