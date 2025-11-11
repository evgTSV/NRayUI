namespace NRayUI.Elements.Panels

open System.Numerics
open NRayUI.Elements
open NRayUI.Modifier
open type Raylib_CSharp.Rendering.Graphics

type Canvas = {
    Children: IElem list
    Box: Box
    IsFixedSize: bool
} with

    interface IPanel<Canvas> with
        member this.GetChildren = this.Children
        member this.SetChildren children = { this with Children = children }

        member this.IsFixedSize = this.IsFixedSize

        member this.SetFixedSize v = {
            this with
                IsFixedSize = this.IsFixedSize
        }

        member this.CalculateChildPos(_, prev, curr, i, basePoint) =
            let prev = prev.GetLayout
            let curr = curr.GetLayout

            Vector2(
                basePoint.X + curr.Position.X + prev.Margin.Right + curr.Margin.Left,
                basePoint.Y + curr.Position.Y + prev.Margin.Bottom + curr.Margin.Top
            )

    interface IElem with
        member this.Render(ctx) = ctx |> renderPanelBase this

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

             {
                 Children = []
                 Box = box
                 IsFixedSize = true
             })

    static member Default = Canvas.DefaultLazy.Force()

/// Canvas - panel, where children layout by the absolute coordinated from canvas borders
[<RequireQualifiedAccess>]
module Canvas =
    let create (attributes: (Canvas -> Canvas) list) : Canvas =
        let canvas = attributes |> List.fold (fun acc attr -> attr acc) Canvas.Default

        {
            canvas with
                Children =
                    canvas.Children
                    |> List.sortBy (function
                        | :? ILayoutProvider as c -> Some c.GetLayout.ZIndex
                        | _ -> None)
        }
