namespace NRayUI.Elements

open System.Numerics
open NRayUI.Elements.Elem
open NRayUI.Modifier

type Label = {
    Box: Box
    Text: Text
} with
    interface IElem with
        member this.Render(ctx) =
            let pos = ctx.CurrentPosition
            (this.Box :> IElem).Render(ctx)
            (this.Text :> IElem).Render(
                { ctx with
                    CurrentPosition = pos
                                      + Vector2(this.Box.Layout.Padding.Left, this.Box.Layout.Padding.Top)
                                      + Vector2(0f, (this.Box.Layout.Height - this.Text.FontSize) / 2f) })
            
        member this.Update _ = this
        
    interface ILayoutProvider with
        member this.GetLayout = this.Box.Layout
        
    interface IWithLayout<Label> with
        member this.SetLayout(layout) = { this with Label.Box.Layout = layout }
        
    interface IBoxProvider with
        member this.GetBox = this.Box
        
    interface IWithBox<Label> with
        member this.SetBox(box) = { this with Box = box }
        
    interface ITextProvider with
        member this.GetText = this.Text
        
    interface IWithText<Label> with
        member this.SetText(text) = { this with Text = text }
        
    static member private DefaultLazy =
        lazy {
          Box = Box.Default
          Text = Text.Default }
        
    static member Default =
        Label.DefaultLazy.Force()
           
[<RequireQualifiedAccess>] 
module Label =
    let create (attributes: (Label -> Label) list) : Label =
        attributes |> List.fold (fun acc attr -> attr acc) Label.Default