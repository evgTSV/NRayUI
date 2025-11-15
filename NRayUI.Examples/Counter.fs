module NRayUI.Examples.Counter

open NRayUI.Components.UIConfigurator
open NRayUI.Elements
open NRayUI.Elements.Buttons
open NRayUI.Elements.Panels
open NRayUI.Modifier
open NRayUI.RenderBase
open Raylib_CSharp.Colors

let counterView (ctx: UpdateContext) =
    let state = ctx.UseState(0)
    StackPanel.create [
        BoxSet.backgroundColor Color.Black
        BoxSet.borderWidth 0f
        StackPanelSet.spacing 1f
        PanelSet.children [
            Label.create [
                LayoutSet.modifiers [
                    paddingScan "250"
                ]
                BoxSet.backgroundColor Color.Blank
                TextSet.color Color.White
                TextSet.content (string state.Current)
                TextSet.fontSize 100f
            ]
            Button.create [
                LayoutSet.modifiers [
                    padding { Top = 5f; Bottom = 5f; Left = 250f; Right = 250f }
                ]
                BoxSet.backgroundColor Color.DarkGray
                ButtonSet.onClick (fun() -> state.Set(state.Current + 1))
                TextSet.color Color.White
                TextSet.content "+"
                TextSet.fontSize 100f
            ]
            Button.create [
                LayoutSet.modifiers [
                    padding { Top = 5f; Bottom = 5f; Left = 250f; Right = 250f }
                ]
                BoxSet.backgroundColor Color.DarkGray
                ButtonSet.onClick (fun() -> state.Set(state.Current - 1))
                TextSet.color Color.White
                TextSet.content "-"
                TextSet.fontSize 100f
            ]
        ]
    ]
