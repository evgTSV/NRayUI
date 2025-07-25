namespace NRayUI.Elements

open System.Numerics
open Aether
open NRayUI
open NRayUI.Elements.Elem
open NRayUI.Field
open NRayUI.Modifier
open NRayUI.Positioning
open Raylib_CSharp.Transformations
open type Raylib_CSharp.Rendering.Graphics

module Panels =
    type IPanel<'a> =
        inherit IWithBox<'a>
        abstract member GetChildren: IElem list
        abstract member SetChildren: IElem list -> 'a
        
    type Canvas = {
        Children: IElem list
        Box: Box
    } with
        interface IPanel<Canvas> with    
            member this.GetChildren = this.Children
            member this.SetChildren(children) =
                { this with Children = children }
            
        interface IElem with
            member this.Render(context) =
                (this.Box :> IElem).Render(context)
                
                let pos = context.CurrentPosition

                BeginScissorMode(int pos.X, int pos.Y, int this.Box.Layout.Width, int this.Box.Layout.Height)   
                this.Children
                |> List.iter (fun child ->
                    match child with
                    | :? ILayoutProvider as withLayout ->
                        let childLayout = withLayout.GetLayout
                        let childPos = pos + childLayout.Position + Vector2(this.Box.Layout.Padding.Left, this.Box.Layout.Padding.Top)
                        let childContext = 
                            { context with
                                CurrentPosition = childPos }
                        child.Render(childContext)
                    | _ ->
                        child.Render(context))
                EndScissorMode()

            member this.Update(context) =
                { this with
                    Children =
                       this.Children
                       |> List.map _.Update(context)
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
            lazy (
                let box = Box.Default
                { Children = []
                  Box = box }
            )
        static member Default = 
            Canvas.DefaultLazy.Force()
    
    /// Canvas - panel, where children layout by the absolute coordinated from canvas borders
    [<RequireQualifiedAccess>]
    module Canvas =
        let create (attributes: (Canvas -> Canvas) list) : Canvas =
            attributes |> List.fold (fun acc attr -> attr acc) Canvas.Default
        
    type StackPanel = {
        Orientation: Orientation
        Children: IElem list
        Box: Box
    } with
        interface IPanel<StackPanel> with    
            member this.GetChildren = this.Children
            member this.SetChildren(children) =
                { this with Children = children }
            
        interface IElem with
            member this.Render(context) =
                (this.Box :> IElem).Render(context)
                
                let pos = context.CurrentPosition
                let context = 
                    { context with
                        ClipRegion = 
                            Some <| Rectangle(pos.X, pos.Y, this.Box.Layout.Width, this.Box.Layout.Height) }
                for i in 0 .. this.Children.Length - 1 do
                    let child = this.Children[i]
                    match child with
                    | :? ILayoutProvider as withLayout ->
                        let childLayout = withLayout.GetLayout
                        let childPos = pos + (Vector2(childLayout.Margin.Left, childLayout.Margin.Top) * Vector2(float32 i)) +
                                        Vector2(this.Box.Layout.Padding.Left, this.Box.Layout.Padding.Top)
                        let childContext = 
                            { context with
                                CurrentPosition = childPos }
                        child.Render(childContext)
                    | _ ->
                        child.Render(context)

            member this.Update(context) =
                { this with
                    Children =
                       this.Children
                       |> List.map _.Update(context)
                }
                
        interface ILayoutProvider with
            member this.GetLayout = this.Box.Layout
            
        interface IWithLayout<StackPanel> with
            member this.SetLayout(layout) = { this with StackPanel.Box.Layout = layout }
            
        interface IBoxProvider with 
            member this.GetBox = this.Box
        
        interface IWithBox<StackPanel> with
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

    /// StackPanel - panel, where children layout by the stack order with orientation
    [<RequireQualifiedAccess>]
    module StackPanel = 
        let create (attributes: (StackPanel -> StackPanel) list) : StackPanel =
            attributes |> List.fold (fun acc attr -> attr acc) StackPanel.Default