namespace NRayUI.Elements.Panels

open System.Numerics
open NRayUI.Constants
open NRayUI.Elements
open NRayUI.Field
open NRayUI.Modifier

type StackPanel = {
    Orientation: Orientation
    Children: IElem list
    Spacing: float32
    Box: Box
} with

    interface IPanel<StackPanel> with
        member this.GetChildren = this.Children
        member this.SetChildren(children) = { this with Children = children }

        member this.CalculateChildPos(prev, curr, i, basePos) =
            if i > 0 then
                match this.Orientation with
                | Orientation.Vertical ->
                    let offset =
                        Vector2(
                            0f,
                            prev.Height + prev.Margin.Bottom + curr.Margin.Top + this.Spacing
                        )

                    basePos <- basePos + offset
                    offset
                | Orientation.Horizontal ->
                    let offset =
                        Vector2(
                            prev.Width + prev.Margin.Right + curr.Margin.Left + this.Spacing,
                            0f
                        )

                    basePos <- basePos + offset
                    offset
            else
                Vector2.Zero

    interface IElem with
        member this.Render(ctx) = ctx |> renderPanelBase this

        member this.Update(ctx) = {
            this with
                Children = this.Children |> List.map _.Update(ctx)
        }

    interface ILayoutProvider with
        member this.GetLayout = this.Box.Layout

    interface IWithLayout<StackPanel> with
        member this.SetLayout(layout) = {
            this with
                StackPanel.Box.Layout = layout
        }

    interface IBoxProvider with
        member this.GetBox = this.Box

    interface IWithBox<StackPanel> with
        member this.SetBox(box) = { this with Box = box }

    static member private DefaultLazy =
        lazy
            (let box = Box.Default

             {
                 Orientation = Orientation.Vertical
                 Children = []
                 Spacing = DefaultStackPanelSpacing
                 Box = box
             })

    static member Default = StackPanel.DefaultLazy.Force()

/// StackPanel - panel, where children layout by the stack order with orientation
[<RequireQualifiedAccess>]
module StackPanel =
    let create (attributes: (StackPanel -> StackPanel) list) : StackPanel =
        attributes |> List.fold (fun acc attr -> attr acc) StackPanel.Default
