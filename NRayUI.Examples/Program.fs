
open System.Numerics
open NRayUI
open NRayUI.Components.UIConfigurator
open NRayUI.Elements
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
            top 100f
            left 200f
            margin { Top = 10f; Right = 20f; Bottom = 10f; Left = 20f }
            padding { Top = 25f; Right = 15f; Bottom = 5f; Left = 5f }
        ]
        LayoutSet.height 500f >> LayoutSet.width 250f
        BoxSet.backgroundColor Color.Blue
        BoxSet.borderColor Color.DarkBlue
        BoxSet.borderWidth 10f
        BoxSet.cornerRadius { TopLeft = 0f; TopRight = 1.f; BottomLeft = 0.5f; BottomRight = 0.1f }
        StackPanelSet.orientation Orientation.Vertical
        PanelSet.children [
            Label.create [
                TextSet.content "Hello, World!"
                TextSet.color Color.Red
                TextSet.fontSize 20f
                LayoutSet.modifiers [
                    margin { Top = 10f; Right = 10f; Bottom = 10f; Left = 10f }
                ]
            ]
            Label.create [
                TextSet.color Color.Red
                TextSet.fontSize 20f
                LayoutSet.modifiers [
                    margin { Top = 10f; Right = 10f; Bottom = 10f; Left = 10f }
                ]
                BoxSet.backgroundColor Color.Green
            ]
        ]
    ]
let test2 =
    Canvas.create [
        LayoutSet.modifiers [
            margin { Top = 25f; Right = 25f; Bottom = 25f; Left = 25f }
            padding { Top = 10f; Right = 10f; Bottom = 10f; Left = 10f }
        ]
        LayoutSet.height 400f >> LayoutSet.width 250f
        BoxSet.backgroundColor Color.Black
        PanelSet.children [
            Label.create [
                TextSet.content "Hello, World!"
                TextSet.color Color.Red
                TextSet.fontSize 20f
                LayoutSet.modifiers [
                    position (Vector2(100f, 10f))
                ]
            ]
            Label.create [
                TextSet.color Color.Red
                TextSet.fontSize 20f
                LayoutSet.modifiers [
                    position (Vector2(100f, 50f))
                ]
                BoxSet.backgroundColor Color.Green
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
    if not (Window.ShouldClose()) then
        UIRendering.render test
        mainLoop ()
        
mainLoop()

Window.Close()