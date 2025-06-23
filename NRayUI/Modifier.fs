namespace NRayUI

open Positioning
open Raylib_CSharp
open Alignment

module Modifier =
    
    type Layout = {
        Width: int
        Height: int
        Alignment: Alignment
        Margin: Margin
        Padding: Padding
        Offset: Offset option
        Position: Position
        ZIndex: int
    } with
        static member withModifiers (modifiers: (Layout -> Layout) List) (layout: Layout) : Layout =
            modifiers
            |> List.fold (fun acc modifier -> modifier acc) layout
    
    let createLayout width height = {
        Width = width
        Height = height
        Alignment = Alignment.TopStart
        Margin = Positioning.zeroSides
        Padding = Positioning.zeroSides
        Offset = None
        Position = Positioning.Static(0.0f, 0.0f)
        ZIndex = 0
    }
    
    let windowLayoutLazy =
        lazy createLayout
            (Windowing.Window.GetScreenWidth())
            (Windowing.Window.GetScreenHeight())
    
    let windowLayout =
        windowLayoutLazy
        |> _.Force()
    
    let align (alignment: Alignment) (layout: Layout) : Layout =   
        { layout with Alignment = alignment }
        
    let margin (margin: Margin) (layout: Layout) : Layout =
        { layout with Margin = margin }
        
    let marginScan (input: string) (layout: Layout) : Layout =
        let sides = Scanner.scanSidesInput input
        { layout with Margin = sides }
        
    let padding (padding: Padding) (layout: Layout) : Layout =
        { layout with Padding = padding }
        
    let paddingScan (input: string) (layout: Layout) : Layout =
        let sides = Scanner.scanSidesInput input
        { layout with Padding = sides }
        
    let offset (offset: Offset) (layout: Layout) : Layout =
        { layout with Offset = Some offset }
        
    let position (position: Position) (layout: Layout) : Layout =
        { layout with Position = position }
        
    let zIndex (zIndex: int) (layout: Layout) : Layout =
        { layout with ZIndex = zIndex }