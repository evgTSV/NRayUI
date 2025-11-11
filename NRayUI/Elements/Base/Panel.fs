namespace NRayUI.Elements

open System
open System.Numerics
open NRayUI.Elements.Elem
open NRayUI.Modifier
open NRayUI.RenderBase
open NRayUI.Field
open type Raylib_CSharp.Rendering.Graphics

[<AutoOpen>]
module Panel =

    [<Interface>]
    type IPanel<'a> =
        inherit ILayoutProvider
        inherit IWithBox<'a>

        abstract member GetChildren: IElem list
        abstract member SetChildren: IElem list -> 'a

        abstract member IsFixedSize: bool
        abstract member SetFixedSize: bool -> 'a

        abstract member CalculateChildPos:
            prevPos: Vector2 *
            prev: ILayoutProvider *
            curr: ILayoutProvider *
            i: int *
            basePos: Vector2 ->
                Vector2

    let calculateChildrenPositions (panel: IPanel<'a>) (ctx: RenderingContext) =
        let layout = (panel :> ILayoutProvider).GetLayout

        let panelBox = (panel :> IBoxProvider).GetBox
        let mutable scissor = panelBox.GetScissorRange ctx.CurrentPosition

        let basePos =
            Vector2(layout.Padding.Left + scissor.X, scissor.Y + layout.Padding.Top)

        let childrenWithLayout, childrenWithoutLayout =
            panel.GetChildren
            |> List.partition (function
                | :? ILayoutProvider -> true
                | _ -> false)

        let childrenWithLayout =
            childrenWithLayout |> List.map (fun c -> c :?> ILayoutProvider)

        let initState = {|
            PrevPos = basePos
            Width = 0f
            Height = 0f
            Ptr = 0
        |}

        // Adaptive-sized panel
        // panelPos -(padding)-> p1 <-(margin)-> p2 <-(padding)- panelPosEnd
        // size = panelPosEnd - panelPos
        let positionedChildren, state =
            (initState, childrenWithLayout)
            ||> List.mapFold (fun state child ->
                let prev =
                    if state.Ptr > 0 then
                        childrenWithLayout[state.Ptr - 1]
                    else
                        { new ILayoutProvider with
                            member this.GetLayout = Layout.Zero
                        }

                let childPos =
                    panel.CalculateChildPos(state.PrevPos, prev, child, state.Ptr, basePos)

                let newWidth =
                    Math.Max(childPos.X - basePos.X + child.GetLayout.Width, state.Width)

                let newHeight =
                    Math.Max(childPos.Y - basePos.Y + child.GetLayout.Height, state.Height)

                let state = {|
                    state with
                        PrevPos = childPos
                        Width = newWidth
                        Height = newHeight
                        Ptr = state.Ptr + 1
                |}

                childPos, state)

        positionedChildren
        @ (List.replicate childrenWithoutLayout.Length ctx.CurrentPosition),
        Vector2(
            Math.Max(layout.Padding.Left + state.Width + layout.Padding.Right, layout.Width),
            Math.Max(layout.Padding.Top + state.Height + layout.Padding.Bottom, layout.Height)
        )

    let renderPanelBase (panel: IPanel<'a>) : RenderAction =
        fun ctx ->

            let layout = (panel :> ILayoutProvider).GetLayout

            let ctx = {
                ctx with
                    CurrentPosition =
                        ctx.CurrentPosition + Vector2(layout.Margin.Left, layout.Margin.Top)
            }

            let positions, limitSize = calculateChildrenPositions panel ctx

            let panelBox =
                if panel.IsFixedSize then
                    (panel :> IBoxProvider).GetBox
                else
                    {
                        (panel :> IBoxProvider).GetBox with
                            Box.Layout.Width = limitSize.X
                            Box.Layout.Height = limitSize.Y
                    }

            (panelBox :> IElem).Render(ctx)

            let scissor = panelBox.GetScissorRange ctx.CurrentPosition

            let ctx = {
                ctx with
                    ScissorRegion = scissor <&&?> ctx.ScissorRegion
            }

            let children = panel.GetChildren

            positions
            |> List.iteri (fun i childPos ->
                let child = children[i]
                let childContext = { ctx with CurrentPosition = childPos }
                child.Render(childContext))
