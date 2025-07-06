module NRayUI.Components.UIConfigurator

open System.Runtime.InteropServices
open Aether
open NRayUI
open NRayUI.Elements
open NRayUI.Elements.Elem
open NRayUI.Elements.Panels
open NRayUI.Field
open NRayUI.Modifier
open NRayUI.Positioning
open NRayUI.Positioning.Alignment
open Raylib_CSharp.Colors

[<RequireQualifiedAccess>]
module LayoutSet =
    
    /// Allows setting multiple layout modifiers at once.
    let modifiers (modifiers: (Layout -> Layout) list) (x: #IWithLayout<'a>) =
        let layout = x.GetLayout
        x.SetLayout (Layout.WithModifiers modifiers layout)
    
    /// Sets the width of a layout.
    let width (width: float32) =
        (LayoutLenses.width, width) ||> Optic.set
        
    /// Sets the height of a layout.
    let height (height: float32) =
        (LayoutLenses.height, height) ||> Optic.set
        
    /// Sets the alignment of a layout.
    let alignment (alignment: Alignment) =
        (LayoutLenses.alignment, alignment) ||> Optic.set
        
    /// Sets the margin of a layout.
    let margin (margin: Margin) =
        (LayoutLenses.margin, margin) ||> Optic.set
        
    /// Scans margin input and sets the margin of a layout.
    let marginScan (input: string) =
        let margin = Scanner.scanSidesInput input
        (LayoutLenses.margin, margin) ||> Optic.set

    /// Sets the padding of a layout.
    let padding (padding: Padding) =
        (LayoutLenses.padding, padding) ||> Optic.set
    
    /// Sets the offset of a layout.
    let offset (offset: Offset) =
        (LayoutLenses.offset, offset) ||> Optic.set
    
    /// Sets the position of a layout.
    let position (position: Position) =
        (LayoutLenses.position, position) ||> Optic.set
    
    /// Sets the zIndex of a layout.
    let zIndex (zIndex: int) =
        (LayoutLenses.zIndex, zIndex) ||> Optic.set

    /// Sets the top offset of a layout.
    let top (value: float32) =
        (LayoutLenses.top, value) ||> Optic.set

    /// Sets the bottom offset of a layout.
    let bottom (value: float32) =
        (LayoutLenses.bottom, value) ||> Optic.set

    /// Sets the left offset of a layout.
    let left (value: float32) =
        (LayoutLenses.left, value) ||> Optic.set

    /// Sets the right offset of a layout.
    let right (value: float32) =
        (LayoutLenses.right, value) ||> Optic.set

[<RequireQualifiedAccess>]
module BoxSet =
    
    /// Sets background color of a box.
    let backgroundColor (color: Color) =
        (BoxLenses.backgroundColor, color) ||> Optic.set
        
    /// Sets border color of a box.
    let borderColor (color: Color) =
        (BoxLenses.borderColor, color) ||> Optic.set
        
    /// Sets border width of a box.
    let borderWidth (width: float32) =
        (BoxLenses.borderWidth, width) ||> Optic.set
        
    /// Sets corner radius of a box.
    let cornerRadius (corners: Corners) =
        (BoxLenses.cornerRadius, corners) ||> Optic.set
    
    /// Sets top left corner radius of a box.
    let crTopLeft (v: float32) =
        (BoxLenses.crTopLeft, v) ||> Optic.set
     
    /// Sets top right corner radius of a box.
    let crTopRight (v: float32) =
        (BoxLenses.crTopRight, v) ||> Optic.set
    
    /// Sets bottom right corner radius of a box.
    let crBottomRight (v: float32) =
        (BoxLenses.crBottomRight, v) ||> Optic.set
    
    /// Sets bottom left corner radius of a box.
    let crBottomLeft (v: float32) =
        (BoxLenses.crBottomLeft, v) ||> Optic.set
        
    /// Scans corner radius input and sets the corner radius of a box.
    let crScan (input: string) =
        let corners = Scanner.scanCornersInput input
        cornerRadius corners
        
    /// Sets the smoothness of the rect corners.
    let smoothness (smoothness: int) =
        (BoxLenses.smoothness, smoothness) ||> Optic.set
   
[<RequireQualifiedAccess>]     
module StackPanelSet =
    
    /// Adds children to a StackPanel.
    let children (children: IElem List) (panel: StackPanel) =
            { panel with Children = children }

    /// Sets the orientation of a StackPanel.
    let orientation (orientation: Orientation) (panel: StackPanel) =
        { panel with Orientation = orientation }