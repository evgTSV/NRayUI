module NRayUI.Components.UIConfigurator

open System.Numerics
open Aether
open NRayUI
open NRayUI.Elements
open NRayUI.Elements.Panels
open NRayUI.Field
open NRayUI.Modifier
open NRayUI.Positioning
open Raylib_CSharp.Colors
open Raylib_CSharp.Fonts
open Raylib_CSharp.Textures

[<RequireQualifiedAccess>]
module LayoutSet =
    
    /// Allows setting multiple layout modifiers at once.
    let modifiers (modifiers: (Layout -> Layout) list) (x: #IWithLayout<'a>) =
        let layout = x.GetLayout
        x.SetLayout (Layout.WithModifiers modifiers layout)
        
    /// Sets the position of a layout.
    let position (position: Vector2) =
        (LayoutLenses.position, position) ||> Optic.set
    
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
module TextSet =
    
    /// Sets the content of a text.
    let content (text: string) =
        (TextLenses.content, text) ||> Optic.set
    
    /// Sets the font of a text.
    let fontSize (fontSize: float32) =
        (TextLenses.fontSize, fontSize) ||> Optic.set
    
    /// Sets the color of a text. 
    let color (color: Color) =
        (TextLenses.color, color) ||> Optic.set
        
    /// Sets the font of a text.
    let font (font: Font) =
        (TextLenses.font, font) ||> Optic.set
        
    /// Sets the background color of a text.
    let backgroundColor (color: Color) =
        (TextLenses.backgroundColor, color) ||> Optic.set
        
    /// Sets the spacing of a text.
    let spacing (spacing: float32) =
        (TextLenses.spacing, spacing) ||> Optic.set
        
[<RequireQualifiedAccess>]
module PanelSet =
    
    /// Sets children to a Panel.
    let children (children: IElem List) (panel: IPanel<'a>) =
            panel.SetChildren children
   
[<RequireQualifiedAccess>]     
module StackPanelSet =
    
    /// Sets the orientation of a StackPanel.
    let orientation (orientation: Orientation) (panel: StackPanel) =
        { panel with Orientation = orientation }
        
    /// Sets the distance between continuous elements of a StackPanel.
    let spacing (value: float32) (panel: StackPanel) =
        { panel with Spacing = value }
      
[<RequireQualifiedAccess>]  
module ImageBoxSet =
    
    /// Sets the source of image.
    let source (source: IImageSource) (imageBox: ImageBox) =
        { imageBox with Image = source }
        
    /// <summary>
    /// Same as <see cref="source"/>, 
    /// but set Texture2D as source image
    /// </summary>
    let texture (tex: Texture2D) =
        let img = TextureSource(tex)
        source img
        
    let tint (value: Color) (imageBox: ImageBox) =
        { imageBox with Tint = value }