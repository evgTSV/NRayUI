namespace NRayUI

open System.Numerics
open Aether
open Aether.Operators
open Positioning
open Raylib_CSharp

module Modifier =

    type Layout = {
        Position: Vector2
        Width: float32
        Height: float32
        Margin: Margin
        Padding: Padding
        Offset: Offset
        Alignment: Alignment option
        ZIndex: int
    } with

        static member WithModifiers (modifiers: (Layout -> Layout) List) (layout: Layout) : Layout =
            modifiers |> List.fold (fun acc modifier -> modifier acc) layout

        static member Zero = {
            Position = Vector2.Zero
            Width = 0f
            Height = 0f
            Margin = Margin.zero
            Padding = Padding.zero
            Offset = Offset.zero
            Alignment = None
            ZIndex = 0
        }

    [<Interface>]
    type ILayoutProvider =
        abstract member GetLayout: Layout

    [<Interface>]
    type IWithLayout<'a> =
        inherit ILayoutProvider
        abstract member SetLayout: Layout -> 'a

    let createLayout pos width height = {
        Position = pos
        Width = width
        Height = height
        Alignment = None
        Margin = zeroSides
        Padding = zeroSides
        Offset = zeroSides
        ZIndex = 0
    }

    let withModifiers (modifiers: (Layout -> Layout) list) (layout: Layout) : Layout =
        Layout.WithModifiers modifiers layout

    let position (v: Vector2) (layout: Layout) : Layout = { layout with Position = v }

    let width (v: float32) (layout: Layout) : Layout = { layout with Width = v }

    let height (v: float32) (layout: Layout) : Layout = { layout with Height = v }

    let align (alignment: Alignment) (layout: Layout) : Layout = {
        layout with
            Alignment = Some alignment
    }

    let margin (margin: Margin) (layout: Layout) : Layout = { layout with Margin = margin }

    let marginScan (input: string) (layout: Layout) : Layout =
        let sides = Scanner.scanSidesInput input
        { layout with Margin = sides }

    let padding (padding: Padding) (layout: Layout) : Layout = { layout with Padding = padding }

    let paddingScan (input: string) (layout: Layout) : Layout =
        let sides = Scanner.scanSidesInput input
        { layout with Padding = sides }

    let offset (offset: Offset) (layout: Layout) : Layout = { layout with Offset = offset }

    let top (value: float32) (layout: Layout) : Layout = {
        layout with
            Layout.Offset.Top = value
    }

    let bottom (value: float32) (layout: Layout) : Layout = {
        layout with
            Layout.Offset.Bottom = value
    }

    let left (value: float32) (layout: Layout) : Layout = {
        layout with
            Layout.Offset.Left = value
    }

    let right (value: float32) (layout: Layout) : Layout = {
        layout with
            Layout.Offset.Right = value
    }

    let zIndex (zIndex: int) (layout: Layout) : Layout = { layout with ZIndex = zIndex }

    let private windowLayoutLazy =
        lazy
            (createLayout
                (Vector2(0f, 0f))
                (float32 (Windowing.Window.GetScreenWidth()))
                (float32 (Windowing.Window.GetScreenHeight()))
             |> withModifiers [ zIndex -1 ])

    let windowLayout = windowLayoutLazy |> _.Force()

    [<RequireQualifiedAccess>]
    module LayoutLenses =

        let private layoutLens<'a when 'a :> IWithLayout<'a>> =
            (fun (x: 'a) -> x.GetLayout), (fun (v: Layout) (x: 'a) -> x.SetLayout v)

        let position<'a when 'a :> IWithLayout<'a>> : Lens<'a, Vector2> =
            let innerLens = _.Position, position
            layoutLens >-> innerLens

        let width<'a when 'a :> IWithLayout<'a>> : Lens<'a, float32> =
            let innerLens = _.Width, width
            layoutLens >-> innerLens

        let height<'a when 'a :> IWithLayout<'a>> : Lens<'a, float32> =
            let innerLens = _.Height, height
            layoutLens >-> innerLens

        let alignment<'a when 'a :> IWithLayout<'a>> : Prism<'a, Alignment> =
            let innerPrism = _.Alignment, align
            layoutLens >-> innerPrism

        let margin<'a when 'a :> IWithLayout<'a>> : Lens<'a, Margin> =
            let innerLens = _.Margin, margin
            layoutLens >-> innerLens

        let padding<'a when 'a :> IWithLayout<'a>> : Lens<'a, Padding> =
            let innerLens = _.Padding, padding
            layoutLens >-> innerLens

        let offset<'a when 'a :> IWithLayout<'a>> : Lens<'a, Offset> =
            let innerLens = _.Offset, offset
            layoutLens >-> innerLens

        let top<'a when 'a :> IWithLayout<'a>> : Lens<'a, float32> =
            let innerLens = _.Offset.Top, top
            layoutLens >-> innerLens

        let bottom<'a when 'a :> IWithLayout<'a>> : Lens<'a, float32> =
            let innerLens = _.Offset.Bottom, bottom
            layoutLens >-> innerLens

        let left<'a when 'a :> IWithLayout<'a>> : Lens<'a, float32> =
            let innerLens = _.Offset.Left, left
            layoutLens >-> innerLens

        let right<'a when 'a :> IWithLayout<'a>> : Lens<'a, float32> =
            let innerLens = _.Offset.Right, right
            layoutLens >-> innerLens

        let zIndex<'a when 'a :> IWithLayout<'a>> : Lens<'a, int> =
            let innerLens = _.ZIndex, zIndex
            layoutLens >-> innerLens
