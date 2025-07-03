namespace NRayUI.Elements

open System.Numerics
open Aether
open Aether.Operators
open NRayUI
open NRayUI.Modifier
open NRayUI.Positioning
open NRayUI.RenderingUtils
open Raylib_CSharp.Colors
open type Raylib_CSharp.Rendering.Graphics

[<Interface>]
type IElem =
    abstract member Render: point: Vector2 -> unit
    abstract member Update: unit -> IElem

module Elem =
   
    type Box = {
        Layout: Layout
        BackgroundColor: Color
        BorderColor: Color
        BorderWidth: float32
        CornerRadius: Corners
    } with    
        interface IElem with
            member this.Render(point: Vector2) =
                let rec_ = this.Layout.GetRec(point)
                DrawRectangleRec(rec_, this.BackgroundColor)
                DrawRectangleCustomRoundedLines 
                    rec_
                    this.CornerRadius
                    this.BorderWidth
                    10
                    this.BorderColor

            member this.Update() = this
            
        interface IWithLayout<Box> with
            member this.GetLayout = this.Layout
            member this.SetLayout(layout) = { this with Layout = layout }
            
        static member private DefaultLazy =
            lazy {
              Layout = createLayout 100.0f 100.0f
              BackgroundColor = Color.White
              BorderColor = Color.Black
              BorderWidth = 1.0f
              CornerRadius = createCorners 0f }
            
        static member Default =
            Box.DefaultLazy.Force()
    
    [<Interface>]
    type IWithBox<'a> =
        abstract member GetBox: Box
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
            
    type Div = {
        Box: Box
        Children: IElem list
    } with
        interface IElem with
            member this.Render(point: Vector2) =
                (this.Box :> IElem).Render(point)
                // TODO: Render children
                
            member this.Update() =
                { this with Children = this.Children |> List.map _.Update() }
                
        interface IWithLayout<Div> with
            member this.GetLayout = this.Box.Layout
            member this.SetLayout(layout) = { this with Box = { this.Box with Layout = layout } }
            
        interface IWithBox<Div> with
            member this.GetBox = this.Box
            member this.SetBox(box) = { this with Box = box }
            
        static member private DefaultLazy =
            lazy (
              let box = Box.Default
              { Box = box; Children = [] }
            )
            
        static member Default =
            Div.DefaultLazy.Force()
            
    [<RequireQualifiedAccess>]
    module Div =
        
        let create (attributes: (Div -> Div) list) : Div =
            attributes |> List.fold (fun acc attr -> attr acc) Div.Default