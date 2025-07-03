namespace NRayUI.Elements

open System.Numerics
open Aether
open NRayUI
open NRayUI.Elements.Elem
open NRayUI.Field
open NRayUI.Modifier

module Panels =
    type StackPanel = {
        Orientation: Orientation
        Children: IElem List
        Box: Box
    } with
        interface IElem with
            member this.Render(point: Vector2) =
                (this.Box :> IElem).Render(point)

            member this.Update() =
                { this with
                    Children = this.Children |> List.map _.Update()
                }
                
        interface IWithLayout<StackPanel> with
            member this.GetLayout = this.Box.Layout
            member this.SetLayout(layout) = this |> Optic.set BoxLenses.layout layout
            
        interface IWithBox<StackPanel> with
            member this.GetBox = this.Box
            member this.SetBox(box) = { this with Box = box }
            
        static member private DefaultLazy =
            lazy (
                let box = Box.Default
                { Orientation = Orientation.Vertical
                  Children = []
                  Box = box }
            )
        static member Default = 
            StackPanel.DefaultLazy.Force()

    [<RequireQualifiedAccess>]
    module StackPanel = 
        let create (attributes: (StackPanel -> StackPanel) list) : StackPanel =
            attributes |> List.fold (fun acc attr -> attr acc) StackPanel.Default