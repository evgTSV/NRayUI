namespace NRayUI.Elements.Panels

open System.Numerics
open NRayUI.Elements
open NRayUI.Modifier
open type Raylib_CSharp.Rendering.Graphics

type Canvas = {
    Children: IElem list
    Box: Box
} with

    interface IPanel<Canvas> with
        member this.GetChildren = this.Children
        member this.SetChildren(children) = { this with Children = children }

    interface IElem with
        member this.Render(ctx) =
            failwith "Not ready for rendering yet! (While use StackPanel)"

            (this.Box :> IElem).Render(ctx)

            let pos = ctx.CurrentPosition

            BeginScissorMode(
                int pos.X,
                int pos.Y,
                int this.Box.Layout.Width,
                int this.Box.Layout.Height
            )

            this.Children
            |> List.iter (fun child ->
                match child with
                | :? ILayoutProvider as withLayout ->
                    let childLayout = withLayout.GetLayout

                    let childPos =
                        pos
                        + childLayout.Position
                        + Vector2(this.Box.Layout.Padding.Left, this.Box.Layout.Padding.Top)

                    let childContext = { ctx with CurrentPosition = childPos }
                    child.Render(childContext)
                | _ -> child.Render(ctx))

            EndScissorMode()

        member this.Update(ctx) = {
            this with
                Children = this.Children |> List.map _.Update(ctx)
        }

    interface ILayoutProvider with
        member this.GetLayout = this.Box.Layout

    interface IWithLayout<Canvas> with
        member this.SetLayout(layout) = { this with Canvas.Box.Layout = layout }

    interface IBoxProvider with
        member this.GetBox = this.Box

    interface IWithBox<Canvas> with
        member this.SetBox(box) = { this with Box = box }

    static member private DefaultLazy =
        lazy
            (let box = Box.Default
             { Children = []; Box = box })

    static member Default = Canvas.DefaultLazy.Force()

/// Canvas - panel, where children layout by the absolute coordinated from canvas borders
[<RequireQualifiedAccess>]
module Canvas =
    let create (attributes: (Canvas -> Canvas) list) : Canvas =
        attributes |> List.fold (fun acc attr -> attr acc) Canvas.Default
