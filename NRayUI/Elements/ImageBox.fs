namespace NRayUI.Elements

open System.Numerics
open NRayUI.Elements.EmptySource
open NRayUI.Modifier
open NRayUI.Field
open Raylib_CSharp.Colors
open type Raylib_CSharp.Rendering.Graphics

type ImageBox = {
    Image: IImageSource
    Box: Box
    Tint: Color
} with

    interface IElem with
        member this.Render(ctx) =
            let pos = ctx.CurrentPosition
            let layout = this.Box.Layout
            let borderOffset = this.Box.BorderWidth / 2f

            (this.Box :> IElem).Render(ctx)

            let offset =
                Vector2(layout.Padding.Left, layout.Padding.Top) + Vector2(borderOffset)

            let render = this.Image.Draw(pos + offset, this.Tint)

            {
                ctx with
                    ScissorRegion = this.Box.GetScissorRange pos <&&?> ctx.ScissorRegion
            }
            |> render

        member this.Update _ = this

    interface ILayoutProvider with
        member this.GetLayout = this.Box.Layout

    interface IWithLayout<ImageBox> with
        member this.SetLayout(layout) = {
            this with
                ImageBox.Box.Layout = layout
        }

    interface IBoxProvider with
        member this.GetBox = this.Box

    interface IWithBox<ImageBox> with
        member this.SetBox(box) = { this with Box = box }

    static member private DefaultLazy =
        lazy
            (let box = Box.Default

             {
                 Image = emptySource
                 Tint = Color.RayWhite
                 Box = box
             })

    static member Default = ImageBox.DefaultLazy.Force()

/// Represents the simple image without difficult logic (editing, scaling, etc.)
[<RequireQualifiedAccess>]
module ImageBox =
    let create (attributes: (ImageBox -> ImageBox) list) : ImageBox =
        attributes |> List.fold (fun acc attr -> attr acc) ImageBox.Default
