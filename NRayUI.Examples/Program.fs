
open System
open NRayUI.Components.UIConfigurator
open NRayUI.Elements
open NRayUI.Elements.Panels
open NRayUI.Field
open NRayUI.Icons
open NRayUI.Modifier
open NRayUI.RenderBase
open NRayUI.Window
open NRayUI.Positioning
open NRayUI.UIRendering
open Raylib_CSharp.Colors
open type Raylib_CSharp.Rendering.Graphics
open Raylib_CSharp.Windowing

let test (ctx: RenderingContext) =
    StackPanel.create [
        LayoutSet.modifiers [
            top 100f >> left 200f
            margin { Top = 100f; Right = 20f; Bottom = 10f; Left = 50f }
            padding { Top = 25f; Right = 5f; Bottom = 25f; Left = 20f }
        ]
        LayoutSet.height 500f >> LayoutSet.width 250f
        BoxSet.backgroundColor Color.Blue
        BoxSet.borderColor Color.DarkBlue
        BoxSet.borderWidth 10f
        BoxSet.cornerRadius { TopLeft = 0f; TopRight = 1f; BottomLeft = 0.5f; BottomRight = 0.1f }
        StackPanelSet.orientation Orientation.Vertical
        PanelSet.children [
            Label.create [
                TextSet.content "Hello!"
                TextSet.color Color.Red
                TextSet.fontSize 60f
                LayoutSet.modifiers [
                    paddingScan "0 0 0 20"
                    width 200f >> height 120f
                ]
                BoxSet.cornerRadius { Corners.zero with TopRight = 1f }
            ]
            ImageBox.create [
                BoxSet.backgroundColor Color.Blank
                BoxSet.borderWidth 5f
                ImageBoxSet.source (IconSource(Icon.CPU, 5))
                ImageBoxSet.tint Color.RayWhite
                LayoutSet.modifiers [
                    padding { Padding.zero with Top = 8f; Left = 8f }
                    width 100f >> height 100f
                ]
            ]
            Label.create [
                TextSet.content $"L{String('o', 50)}ng"
                TextSet.color Color.Black
                LayoutSet.modifiers [
                    paddingScan "0 0 0 30"
                    width 800f >> height 70f
                ]
                BoxSet.backgroundColor Color.White
                BoxSet.crScan "1"
            ]
            ImageBox.create [
                BoxSet.backgroundColor Color.Blank
                BoxSet.borderColor Color.Red
                ImageBoxSet.texture (ctx.Resources.LoadTexture @"./Assets/nrayui_logo_100x100.png")
                ImageBoxSet.tint Color.RayWhite
                LayoutSet.modifiers [
                    paddingScan "10"
                    width 120f >> height 120f
                ]
            ]
        ]
    ]
    
let config =
         ConfigFlags.ResizableWindow |||
         ConfigFlags.Msaa4XHint |||
         ConfigFlags.VSyncHint |||
         ConfigFlags.AlwaysRunWindow

let window = {
    Title = "NRayUI Example"
    WindowSizePx = (800, 800)
}
         
test |> startRendering window config