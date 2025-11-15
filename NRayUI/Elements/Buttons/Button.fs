namespace NRayUI.Elements.Buttons

open Microsoft.Extensions.DependencyInjection
open NRayUI
open NRayUI.Elements
open NRayUI.Input
open NRayUI.Input.UserInput
open NRayUI.Modifier
open NRayUI.Services
open NRayUI.Utils
open Raylib_CSharp.Interact
open Raylib_CSharp.Transformations

type Button = {
    Id: int64
    OnClick: unit -> unit
    Label: Label
} with
    
    interface IElem with
        member this.Render(ctx) =
            ctx |> (this.Label :> IElem).Render
            
            let stateService = ctx.ServiceProvider.GetRequiredService<UIStateService>()
            let bounds =
                Rectangle(
                    ctx.CurrentPosition.X,
                    ctx.CurrentPosition.Y,
                    this.Width,
                    this.Height
                )
            stateService.SetButtonState(this.Id, { Bounds = bounds })

        member this.Update ctx =    
            let stateService = ctx.ServiceProvider.GetRequiredService<UIStateService>()
            match stateService.TryGetButtonState(this.Id) with
            | Some state ->
                let bounds = state.Bounds
                
                if 
                    ctx.Input
                    |> Array.exists (function
                        | MouseClicked (key, pos)
                            when key = MouseButton.Left && bounds.Contains(pos) ->
                                true
                        | _ -> false)
                then this.OnClick()
                
                this
            | None -> this

    interface ILayoutProvider with
        member this.GetLayout =
            (this.Label :> ILayoutProvider).GetLayout

    interface IWithLayout<Button> with
        member this.SetLayout(layout) = {
            this with Button.Label = (this.Label :> IWithLayout<Label>).SetLayout(layout)
        }

    interface IBoxProvider with
        member this.GetBox = (this.Label :> IBoxProvider).GetBox

    interface IWithBox<Button> with
        member this.SetBox(box) = {
            this with Label = (this.Label :> IWithBox<Label>).SetBox(box)
        }

    interface ITextProvider with
        member this.GetText = (this.Label :> ITextProvider).GetText

    interface IWithText<Button> with
        member this.SetText(text) = {
            this with Label = (this.Label :> IWithText<Label>).SetText(text)
        }
        
    member this.Width =
        this.Label.Width
    
    member this.Height =
        this.Label.Height

    static member Default =
        let label = Label.Default
        {
            Id = SequentialIdGenerator.getNextId()
            OnClick = (fun() -> ())
            Label = label
        }

/// Button - simple button with text
[<RequireQualifiedAccess>]
module Button =
    let create (attributes: (Button -> Button) list) : Button =
        attributes |> List.fold (fun acc attr -> attr acc) Button.Default
