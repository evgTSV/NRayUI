
open System
open NRayUI.Components.UIConfigurator
open NRayUI.Elements
open NRayUI.Elements.Panels
open NRayUI.Field
open NRayUI.Icons
open NRayUI.Modifier
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
            let labelMargin = margin { Margin.zero with Bottom = 10f }
            Label.create [
                TextSet.content "Hello!"
                TextSet.color Color.Red
                TextSet.fontSize 60f
                LayoutSet.modifiers [
                    paddingScan "0 0 0 20"
                    labelMargin
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
                    labelMargin
                    width 100f >> height 100f
                ]
            ]
            Label.create [
                TextSet.content $"L{String('o', 100)}ng"
                TextSet.color Color.Black
                LayoutSet.modifiers [
                    paddingScan "0 0 0 30"
                    labelMargin
                    width 2000f >> height 70f
                ]
                BoxSet.backgroundColor Color.White
                BoxSet.crScan "1"
            ]
            ImageBox.create [
                BoxSet.backgroundColor Color.Blank
                BoxSet.borderColor Color.Blank
                ImageBoxSet.texture (ctx.Resources.LoadTexture @"C:\Users\EVGENII\Downloads\nrayui_logo.png")
                ImageBoxSet.tint Color.RayWhite
                LayoutSet.modifiers [
                    labelMargin
                    width 100f >> height 100f
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