namespace NRayUI.Elements

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

        /// <remarks>Let inheritor change base pos thought ref</remarks>
        abstract member CalculateChildPos:
            prev: Layout * curr: Layout * i: int * basePos: byref<Vector2> -> Vector2

    let renderPanelBase (panel: IPanel<'a>) : RenderAction =
        fun ctx ->

            let layout = (panel :> ILayoutProvider).GetLayout

            let ctx = {
                ctx with
                    CurrentPosition =
                        ctx.CurrentPosition + Vector2(layout.Margin.Left, layout.Margin.Top)
            }

            let panelBox = (panel :> IBoxProvider).GetBox

            (panelBox :> IElem).Render(ctx)

            let mutable scissor = panelBox.GetScissorRange ctx.CurrentPosition
            let mutable basePos = Vector2(scissor.X, scissor.Y)

            let ctx = {
                ctx with
                    ScissorRegion = scissor <&&?> ctx.ScissorRegion
            }

            let children = panel.GetChildren

            for i in 0 .. children.Length - 1 do
                let child = children[i]

                match child with
                | :? ILayoutProvider as withLayout ->
                    let prevLayout =
                        if i > 0 then
                            let prevChild = children[i - 1]

                            match prevChild with
                            | :? ILayoutProvider as prevWithLayout -> prevWithLayout.GetLayout
                            | _ -> layout
                        else
                            layout

                    let childLayout = withLayout.GetLayout

                    let childContext = {
                        ctx with
                            CurrentPosition =
                                basePos
                                + Vector2(layout.Padding.Left, layout.Padding.Top)
                                + panel.CalculateChildPos(prevLayout, childLayout, i, &basePos)
                    }

                    child.Render(childContext)
                | _ -> child.Render(ctx)
