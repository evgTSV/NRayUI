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
    IsFixedSize: bool
} with

    interface IPanel<StackPanel> with
        member this.GetChildren = this.Children
        member this.SetChildren children = { this with Children = children }

        member this.IsFixedSize = this.IsFixedSize
        member this.SetFixedSize v = { this with IsFixedSize = v }

        member this.CalculateChildPos(prevPos, prev, curr, i, _) =
            let prev = prev.GetLayout
            let curr = curr.GetLayout

            let spacing = if i = 0 then 0f else this.Spacing

            match this.Orientation with
            | Orientation.Vertical ->
                Vector2(
                    prevPos.X,
                    prevPos.Y + prev.Height + prev.Margin.Bottom + curr.Margin.Top + spacing
                )
            | Orientation.Horizontal ->
                Vector2(
                    prevPos.X + prev.Width + prev.Margin.Right + curr.Margin.Left + spacing,
                    prevPos.Y
                )

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

    static member Default =
        let box = Box.Default

        {
            Orientation = Orientation.Vertical
            Children = []
            Spacing = DefaultStackPanelSpacing
            Box = box
            IsFixedSize = false
        }

/// StackPanel - panel, where children layout by the stack order with orientation
[<RequireQualifiedAccess>]
module StackPanel =
    let create (attributes: (StackPanel -> StackPanel) list) : StackPanel =
        attributes |> List.fold (fun acc attr -> attr acc) StackPanel.Default
