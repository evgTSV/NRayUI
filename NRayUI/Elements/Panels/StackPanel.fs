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

    interface IElem with
        member this.Render(ctx) =
            let layout = (this :> ILayoutProvider).GetLayout

            let ctx = {
                ctx with
                    CurrentPosition =
                        ctx.CurrentPosition + Vector2(layout.Margin.Left, layout.Margin.Top)
            }

            (this.Box :> IElem).Render(ctx)

            let borderOffset = this.Box.BorderWidth
            let mutable pos = ctx.CurrentPosition + Vector2() + Vector2(borderOffset)

            let ctx = {
                ctx with
                    ScissorRegion = this.Box.GetScissorRange pos <&&?> ctx.ScissorRegion
            }

            for i in 0 .. this.Children.Length - 1 do
                let child = this.Children[i]

                match child with
                | :? ILayoutProvider as withLayout ->
                    let prevLayout =
                        if i > 0 then
                            let prevChild = this.Children[i - 1]

                            match prevChild with
                            | :? ILayoutProvider as prevWithLayout -> prevWithLayout.GetLayout
                            | _ -> layout
                        else
                            layout

                    let childLayout = withLayout.GetLayout

                    let childPos =
                        pos
                        + Vector2(layout.Padding.Left, layout.Padding.Top)
                        + if i > 0 then
                              match this.Orientation with
                              | Orientation.Vertical ->
                                  let offset =
                                      Vector2(
                                          0f,
                                          prevLayout.Height
                                          + prevLayout.Margin.Bottom
                                          + childLayout.Margin.Top
                                          + this.Spacing
                                      )

                                  pos <- pos + offset
                                  offset
                              | Orientation.Horizontal ->
                                  let offset =
                                      Vector2(
                                          prevLayout.Width
                                          + prevLayout.Margin.Right
                                          + childLayout.Margin.Left
                                          + this.Spacing,
                                          0f
                                      )

                                  pos <- pos + offset
                                  offset
                          else
                              Vector2.Zero

                    let childContext = { ctx with CurrentPosition = childPos }
                    child.Render(childContext)
                | _ -> child.Render(ctx)

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
