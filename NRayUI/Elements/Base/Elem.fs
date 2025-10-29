﻿namespace NRayUI.Elements

open System
open System.Numerics
open Aether
open Aether.Operators
open Microsoft.Extensions.DependencyInjection
open NRayUI
open NRayUI.Loader
open NRayUI.Modifier
open NRayUI.Positioning
open NRayUI.RenderingUtils
open NRayUI.Utils
open Raylib_CSharp.Camera.Cam2D
open Raylib_CSharp.Colors
open type Raylib_CSharp.Rendering.Graphics
open type Raylib_CSharp.Fonts.TextManager
open Raylib_CSharp.Fonts
open Raylib_CSharp.Transformations

[<Interface>]
type IElem =
    abstract member Render: RenderingContext -> unit
    abstract member Update: UpdateContext -> IElem
and RenderingContext = {
    Camera: Camera2D
    RenderTargetSize: Vector2
    CurrentPosition: Vector2
    ClipRegion: Rectangle option
    IsDebugMode: bool
    Resources: Resources
    Services: ServiceProvider
}
and UpdateContext = {
    Event: EventHandler // TODO: Implement event system
}

[<AutoOpen>]
module Elem =
   
    type Box = {
        Layout: Layout
        BackgroundColor: Color
        BorderColor: Color
        BorderWidth: float32
        CornerRadius: Corners
        Smoothness: int
    } with
        interface IElem with
            member this.Render(ctx) =
                let pos = ctx.CurrentPosition
                let rec_ = Rectangle(pos.X, pos.Y, this.Layout.Width, this.Layout.Height)
                DrawRectangleCustomRounded
                    rec_
                    this.CornerRadius
                    this.Smoothness
                    this.BackgroundColor
                DrawRectangleCustomRoundedLines 
                    rec_
                    this.CornerRadius
                    this.BorderWidth
                    this.Smoothness
                    this.BorderColor

            member this.Update _ = this
            
        interface ILayoutProvider with
            member this.GetLayout = this.Layout
            
        interface IWithLayout<Box> with
            member this.SetLayout(layout) = { this with Layout = layout }
            
        static member private DefaultLazy =
            lazy {
              Layout = createLayout (Vector2(0f, 0f)) 100.0f 100.0f
              BackgroundColor = Color.White
              BorderColor = Color.Black
              BorderWidth = 1.0f
              CornerRadius = createCorners 0f
              Smoothness = Constants.DefaultSmoothCircleSegments }
            
        static member Default =
            Box.DefaultLazy.Force()
    
    [<Interface>]
    type IBoxProvider =
        abstract member GetBox: Box
    
    [<Interface>]
    type IWithBox<'a> =
        inherit IBoxProvider
        abstract member SetBox: Box -> 'a
        
    [<RequireQualifiedAccess>]
    module BoxLenses =
        
        let private boxLens<'a when 'a :> IWithBox<'a>> =
            (fun (x: 'a) -> x.GetBox), (fun (v: Box) (x: 'a) -> x.SetBox v)
            
        let layout<'a when 'a :> IWithBox<'a>> : Lens<'a, Layout> =
            let innerLens =
                (fun (x: Box) -> x.Layout),
                (fun (v: Layout) (x: Box) -> { x with Layout = v })
            boxLens >-> innerLens

        let backgroundColor<'a when 'a :> IWithBox<'a>> : Lens<'a, Color> =
            let innerLens =
                (fun (x: Box) -> x.BackgroundColor),
                (fun (v: Color) (x: Box) -> { x with BackgroundColor = v })
            boxLens >-> innerLens
            
        let borderColor<'a when 'a :> IWithBox<'a>> : Lens<'a, Color> =
            let innerLens =
                (fun (x: Box) -> x.BorderColor),
                (fun (v: Color) (x: Box) -> { x with BorderColor = v })
            boxLens >-> innerLens
            
        let borderWidth<'a when 'a :> IWithBox<'a>> : Lens<'a, float32> =
            let innerLens =
                (fun (x: Box) -> x.BorderWidth),
                (fun (v: float32) (x: Box) -> { x with BorderWidth = v })
            boxLens >-> innerLens
            
        let cornerRadius<'a when 'a :> IWithBox<'a>> : Lens<'a, Corners> =
            let innerLens =
                (fun (x: Box) -> x.CornerRadius),
                (fun (v: Corners) (x: Box) -> { x with CornerRadius = v })
            boxLens >-> innerLens
            
        let crTopLeft<'a when 'a :> IWithBox<'a>> : Lens<'a, float32> =
            let innerLens =
                (fun (x: Box) -> x.CornerRadius.TopLeft),
                (fun (v: float32) (x: Box) -> { x with CornerRadius.TopLeft = v })
            boxLens >-> innerLens
         
        let crTopRight<'a when 'a :> IWithBox<'a>> : Lens<'a, float32> =
            let innerLens =
                (fun (x: Box) -> x.CornerRadius.TopRight),
                (fun (v: float32) (x: Box) -> { x with CornerRadius.TopRight = v })
            boxLens >-> innerLens
            
        let crBottomRight<'a when 'a :> IWithBox<'a>> : Lens<'a, float32> =
            let innerLens =
                (fun (x: Box) -> x.CornerRadius.BottomRight),
                (fun (v: float32) (x: Box) -> { x with CornerRadius.BottomRight = v })
            boxLens >-> innerLens
            
        let crBottomLeft<'a when 'a :> IWithBox<'a>> : Lens<'a, float32> =
            let innerLens =
                (fun (x: Box) -> x.CornerRadius.BottomLeft),
                (fun (v: float32) (x: Box) -> { x with CornerRadius.BottomLeft = v })
            boxLens >-> innerLens
            
        let smoothness<'a when 'a :> IWithBox<'a>> : Lens<'a, int> =
            let innerLens =
                (fun (x: Box) -> x.Smoothness),
                (fun (v: int) (x: Box) -> { x with Smoothness = v })
            boxLens >-> innerLens
            
    type Text = {
        Content: string
        Font: Font option
        FontSize: float32
        Color: Color
        BackgroundColor: Color
        Spacing: float32
    } with
        member private this.GetFont() =
            this.Font
            |> Option.defaultValue (Font.GetDefault())
            
        member private this.CreateBoxMem =
            lazyMemoize
                (Vector2Comparer())
                (fun (pos: Vector2) ->
                    let textMeasure = MeasureTextEx(this.GetFont(), this.Content, this.FontSize, this.Spacing)
                    let layout = createLayout pos textMeasure.X textMeasure.Y
                    { Box.Default with
                        Layout = layout
                        BackgroundColor = this.BackgroundColor
                        BorderColor = Color.Blank })
        
        interface IElem with
            member this.Render(ctx) =
                let pos = ctx.CurrentPosition
                let box = this.CreateBoxMem pos
                (box :> IElem).Render(ctx)
                DrawTextEx(this.GetFont(), this.Content, pos, this.FontSize, this.Spacing, this.Color)

            member this.Update _ = this
            
        static member private DefaultLazy =
            lazy {
              Content = "Some text"
              Font = None
              FontSize = 20.0f
              Color = Color.Black
              BackgroundColor = Color.Blank
              Spacing = 1.0f }
            
        static member Default =
            Text.DefaultLazy.Force()
    
    [<Interface>]
    type ITextProvider =
        abstract member GetText: Text
    
    [<Interface>]
    type IWithText<'a> =
        inherit ITextProvider
        abstract member SetText: Text -> 'a
    
    [<RequireQualifiedAccess>]         
    module TextLenses =
        
        let textLens<'a when 'a :> IWithText<'a>> =
            (fun (x: 'a) -> x.GetText), (fun (v: Text) (x: 'a) -> x.SetText v)
    
        let content<'a when 'a :> IWithText<'a>> : Lens<'a, string> =
            let innerLens =
                (fun (x: Text) -> x.Content),
                (fun (content: string) (x: Text) -> { x with Content = content })
            textLens >-> innerLens
            
        let fontSize<'a when 'a :> IWithText<'a>> : Lens<'a, float32> =
            let innerLens =
                (fun (x: Text) -> x.FontSize),
                (fun (fontSize: float32) (x: Text) -> { x with FontSize = fontSize })
            textLens >-> innerLens
            
        let color<'a when 'a :> IWithText<'a>> : Lens<'a, Color> =
            let innerLens  =  
                (fun (x: Text) -> x.Color),
                (fun (color: Color) (x: Text) -> { x with Color = color })
            textLens >-> innerLens
            
        let font<'a when 'a :> IWithText<'a>> : Prism<'a, Font> =
            let innerLens =
                (fun (x: Text) -> x.Font),
                (fun (font: Font) (x: Text) -> { x with Font = Some font })
            textLens >-> innerLens
            
        let spacing<'a when 'a :> IWithText<'a>> : Lens<'a, float32> =
            let innerLens =
                (fun (x: Text) -> x.Spacing),
                (fun (spacing: float32) (x: Text) -> { x with Spacing = spacing })
            textLens >-> innerLens
            
        let backgroundColor<'a when 'a :> IWithText<'a>> : Lens<'a, Color> =
            let innerLens =
                (fun (x: Text) -> x.BackgroundColor),
                (fun (color: Color) (x: Text) -> { x with BackgroundColor = color })
            textLens >-> innerLens
            
    module Text =
        let create (attributes: (Text -> Text) list) : Text =
            attributes |> List.fold (fun acc attr -> attr acc) Text.Default