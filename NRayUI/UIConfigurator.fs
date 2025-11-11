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

/// <summary>
/// Helpers for configuring layout-related properties on elements that expose a <c>Layout</c>.
/// Use these helpers to set position, size, alignment, margin, padding and other layout modifiers
/// in a functional style (they return a function suitable for optics-based updates).
/// </summary>
[<RequireQualifiedAccess>]
module LayoutSet =

    /// <summary>
    /// Applies multiple layout modifier functions at once to an element that exposes a layout.
    /// </summary>
    /// <param name="modifiers">A list of functions that take and return a modified <c>Layout</c>.</param>
    /// <param name="x">Any element implementing <c>IWithLayout</c> (the target of the modifiers).</param>
    /// <returns>The element with its layout updated by the provided modifiers.</returns>
    let modifiers (modifiers: (Layout -> Layout) list) (x: #IWithLayout<'a>) =
        let layout =
            x.GetLayout

        x.SetLayout(Layout.WithModifiers modifiers layout)

    /// <summary>
    /// Sets the absolute position of the element's layout.
    /// </summary>
    /// <param name="position">The 2D position (X,Y) to set.</param>
    let position (position: Vector2) =
        (LayoutLenses.position, position) ||> Optic.set

    /// <summary>
    /// Sets the explicit width of the layout.
    /// </summary>
    /// <param name="width">Width in pixels (float32).</param>
    let width (width: float32) =
        (LayoutLenses.width, width) ||> Optic.set

    /// <summary>
    /// Sets the explicit height of the layout.
    /// </summary>
    /// <param name="height">Height in pixels (float32).</param>
    let height (height: float32) =
        (LayoutLenses.height, height) ||> Optic.set

    /// <summary>
    /// Sets both width and height of the layout from a <see cref="System.Numerics.Vector2"/>.
    /// This composes the <c>width</c> and <c>height</c> setters.
    /// </summary>
    /// <param name="value">Vector2 where X is width and Y is height.</param>
    let size (value: Vector2) =
        width value.X >> height value.Y

    /// <summary>
    /// Sets the alignment of content inside the layout.
    /// </summary>
    /// <param name="alignment">An <c>Alignment</c> describing horizontal/vertical alignment.</param>
    let alignment (alignment: Alignment) =
        (LayoutLenses.alignment, alignment) ||> Optic.set

    /// <summary>
    /// Sets the margin (space outside the element) on the layout.
    /// </summary>
    /// <param name="margin">The <c>Margin</c> value to apply.</param>
    let margin (margin: Margin) =
        (LayoutLenses.margin, margin) ||> Optic.set

    /// <summary>
    /// Parses a margin string and sets the resulting <c>Margin</c> on the layout.
    /// The string format is parsed by <c>Scanner.scanSidesInput</c> (e.g. "10" or "10 5").
    /// </summary>
    /// <param name="input">Text input describing margin sides.</param>
    let marginScan (input: string) =
        let margin =
            Scanner.scanSidesInput input

        (LayoutLenses.margin, margin) ||> Optic.set

    /// <summary>
    /// Sets the padding (space inside the element) on the layout.
    /// </summary>
    /// <param name="padding">The <c>Padding</c> value to apply.</param>
    let padding (padding: Padding) =
        (LayoutLenses.padding, padding) ||> Optic.set

    /// <summary>
    /// Sets an additional offset for the layout position.
    /// Useful when applying relative shifts without modifying the base position.
    /// </summary>
    /// <param name="offset">The <c>Offset</c> to set.</param>
    let offset (offset: Offset) =
        (LayoutLenses.offset, offset) ||> Optic.set

    /// <summary>
    /// Sets the rendering order (z-index) of the layout.
    /// Higher values render above lower values.
    /// </summary>
    /// <param name="zIndex">Integer z-index.</param>
    let zIndex (zIndex: int) =
        (LayoutLenses.zIndex, zIndex) ||> Optic.set

    /// <summary>
    /// Sets the top offset property of the layout.
    /// </summary>
    /// <param name="value">Top offset in pixels.</param>
    let top (value: float32) =
        (LayoutLenses.top, value) ||> Optic.set

    /// <summary>
    /// Sets the bottom offset property of the layout.
    /// </summary>
    /// <param name="value">Bottom offset in pixels.</param>
    let bottom (value: float32) =
        (LayoutLenses.bottom, value) ||> Optic.set

    /// <summary>
    /// Sets the left offset property of the layout.
    /// </summary>
    /// <param name="value">Left offset in pixels.</param>
    let left (value: float32) =
        (LayoutLenses.left, value) ||> Optic.set

    /// <summary>
    /// Sets the right offset property of the layout.
    /// </summary>
    /// <param name="value">Right offset in pixels.</param>
    let right (value: float32) =
        (LayoutLenses.right, value) ||> Optic.set

/// <summary>
/// Helpers for configuring box drawing properties (background, border, corner radius, etc.).
/// </summary>
[<RequireQualifiedAccess>]
module BoxSet =

    /// <summary>
    /// Sets the background color of a box element.
    /// </summary>
    /// <param name="color">Raylib <c>Color</c> used as background.</param>
    let backgroundColor (color: Color) =
        (BoxLenses.backgroundColor, color) ||> Optic.set

    /// <summary>
    /// Sets the border color of a box element.
    /// </summary>
    /// <param name="color">Border color.</param>
    let borderColor (color: Color) =
        (BoxLenses.borderColor, color) ||> Optic.set

    /// <summary>
    /// Sets the border width in pixels for a box.
    /// </summary>
    /// <param name="width">Border width (float32).</param>
    let borderWidth (width: float32) =
        (BoxLenses.borderWidth, width) ||> Optic.set

    /// <summary>
    /// Sets corner radii for each corner of a box.
    /// </summary>
    /// <param name="corners">A <c>Corners</c> record specifying individual radii.</param>
    let cornerRadius (corners: Corners) =
        (BoxLenses.cornerRadius, corners) ||> Optic.set

    /// <summary>
    /// Sets top-left corner radius.
    /// </summary>
    /// <param name="v">Radius in pixels.</param>
    let crTopLeft (v: float32) =
        (BoxLenses.crTopLeft, v) ||> Optic.set

    /// <summary>
    /// Sets top-right corner radius.
    /// </summary>
    /// <param name="v">Radius in pixels.</param>
    let crTopRight (v: float32) =
        (BoxLenses.crTopRight, v) ||> Optic.set

    /// <summary>
    /// Sets bottom-right corner radius.
    /// </summary>
    /// <param name="v">Radius in pixels.</param>
    let crBottomRight (v: float32) =
        (BoxLenses.crBottomRight, v) ||> Optic.set

    /// <summary>
    /// Sets bottom-left corner radius.
    /// </summary>
    /// <param name="v">Radius in pixels.</param>
    let crBottomLeft (v: float32) =
        (BoxLenses.crBottomLeft, v) ||> Optic.set

    /// <summary>
    /// Parses a corners specification string and applies it as corner radii.
    /// The parsing is delegated to <c>Scanner.scanCornersInput</c>.
    /// </summary>
    /// <param name="input">Corner radii input string (e.g. "5" or "5 3").</param>
    let crScan (input: string) =
        let corners =
            Scanner.scanCornersInput input

        cornerRadius corners

    /// <summary>
    /// Sets corner smoothness used when drawing rounded corners.
    /// Larger values produce smoother curves.
    /// </summary>
    /// <param name="smoothness">Smoothness sample count (int).</param>
    let smoothness (smoothness: int) =
        (BoxLenses.smoothness, smoothness) ||> Optic.set

/// <summary>
/// Helpers for configuring text elements: content, font, color and related properties.
/// </summary>
[<RequireQualifiedAccess>]
module TextSet =

    /// <summary>
    /// Sets the visible text content.
    /// </summary>
    /// <param name="text">Text to display.</param>
    let content (text: string) =
        (TextLenses.content, text) ||> Optic.set

    /// <summary>
    /// Sets the font size used to measure and draw the text.
    /// </summary>
    /// <param name="fontSize">Font size in pixels.</param>
    let fontSize (fontSize: float32) =
        (TextLenses.fontSize, fontSize) ||> Optic.set

    /// <summary>
    /// Sets the text color.
    /// </summary>
    /// <param name="color">Raylib <c>Color</c> for glyphs.</param>
    let color (color: Color) =
        (TextLenses.color, color) ||> Optic.set

    /// <summary>
    /// Sets the font resource used for rendering text.
    /// </summary>
    /// <param name="font">A Raylib <c>Font</c> object.</param>
    let font (font: Font) =
        (TextLenses.font, font) ||> Optic.set

    /// <summary>
    /// Sets the background color behind the text glyphs.
    /// </summary>
    /// <param name="color">Background color.</param>
    let backgroundColor (color: Color) =
        (TextLenses.backgroundColor, color) ||> Optic.set

    /// <summary>
    /// Sets extra spacing between characters or glyphs.
    /// </summary>
    /// <param name="spacing">Additional spacing in pixels (float32).</param>
    let spacing (spacing: float32) =
        (TextLenses.spacing, spacing) ||> Optic.set

/// <summary>
/// Helpers for configuring <c>Panel</c> instances (container elements that can have children).
/// </summary>
[<RequireQualifiedAccess>]
module PanelSet =

    /// <summary>
    /// Replaces the children collection of a panel with the supplied list.
    /// </summary>
    /// <param name="children">List of child elements to set on the panel.</param>
    /// <param name="panel">Target panel instance.</param>
    let children (children: IElem List) (panel: IPanel<'a>) =
        panel.SetChildren children

    /// <summary>
    /// Controls whether a panel uses explicit fixed size (Width/Height) or adapts to its children.
    /// </summary>
    /// <param name="value">true to use fixed explicit sizing; false to calculate size from children.</param>
    /// <param name="panel">Target panel to configure.</param>
    let fixedSize (value: bool) (panel: IPanel<'a>) =
        panel.SetFixedSize value

/// <summary>
/// Helpers specific to <c>StackPanel</c> configuration such as orientation and spacing.
/// </summary>
[<RequireQualifiedAccess>]
module StackPanelSet =

    /// <summary>
    /// Sets the stacking orientation for the <c>StackPanel</c> (horizontal or vertical).
    /// </summary>
    /// <param name="orientation">Desired <c>Orientation</c> value.</param>
    /// <param name="panel">The <c>StackPanel</c> to update.</param>
    /// <returns>The modified <c>StackPanel</c>.</returns>
    let orientation (orientation: Orientation) (panel: StackPanel) = {
        panel with
            Orientation = orientation
    }

    /// <summary>
    /// Sets the spacing (gap) between adjacent children inside the <c>StackPanel</c>.
    /// </summary>
    /// <param name="value">Spacing in pixels (float32).</param>
    /// <param name="panel">The <c>StackPanel</c> to update.</param>
    /// <returns>The modified <c>StackPanel</c>.</returns>
    let spacing (value: float32) (panel: StackPanel) = {
        panel with
            Spacing = value
    }

/// <summary>
/// Helpers for configuring <c>ImageBox</c> elements (image sources, tinting, etc.).
/// </summary>
[<RequireQualifiedAccess>]
module ImageBoxSet =

    /// <summary>
    /// Sets the image source for an <c>ImageBox</c>.
    /// </summary>
    /// <param name="source">An implementation of <c>IImageSource</c> to use as the content.</param>
    /// <param name="imageBox">Target <c>ImageBox</c>.</param>
    /// <returns>The updated <c>ImageBox</c>.</returns>
    let source (source: IImageSource) (imageBox: ImageBox) = {
        imageBox with
            Image = source
    }

    /// <summary>
    /// Convenience helper that sets a <c>Texture2D</c> as the image source by wrapping it into a <c>TextureSource</c>.
    /// </summary>
    /// <param name="tex">Raylib <c>Texture2D</c> instance to use as the image.</param>
    let texture (tex: Texture2D) =
        let img =
            TextureSource(tex)

        source img

    /// <summary>
    /// Sets a tint color that will be multiplied with the image when drawn.
    /// </summary>
    /// <param name="value">Tint <c>Color</c> to apply.</param>
    /// <param name="imageBox">Target <c>ImageBox</c>.</param>
    let tint (value: Color) (imageBox: ImageBox) = {
        imageBox with
            Tint = value
    }
