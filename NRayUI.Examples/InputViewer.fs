module NRayUI.Examples.InputViewer

open NRayUI.Components.UIConfigurator
open NRayUI.Elements
open NRayUI.Elements.Panels
open NRayUI.Input
open NRayUI.Field
open NRayUI.Modifier
open NRayUI.RenderBase
open Raylib_CSharp.Colors

let private inputToText (input: InputEvent) =
    match input with
    | KeyPressed k ->
        k.ToString()
    | _ ->
        "None"
        
let mutable private inputs: IElem list = []

let private addToFixedSizeList (lst: 'a list) (items: 'a list) : 'a list =
    items @ lst
    |> List.truncate 7
    
let tip = Label.create [
    TextSet.content "Start typing"
    TextSet.color Color.Black
    TextSet.fontSize 30f
    LayoutSet.modifiers [
        paddingScan "0 0 0 20"
    ]
    BoxSet.backgroundColor Color.Blank
    BoxSet.borderWidth 0f
]

let private processInput (ctx: UpdateContext): IElem list =
    let newLabels = 
        ctx.Input
        |> Array.toList
        |> List.filter _.IsKeyPressed
        |> List.map (fun i ->
            Label.create [
                TextSet.content (inputToText i)
                TextSet.color Color.Red
                TextSet.fontSize 30f
                LayoutSet.modifiers [
                    paddingScan "0 20 0 20"
                ]
                BoxSet.crScan "1"
            ] :> IElem)
        
    newLabels
    |> addToFixedSizeList inputs

let inputViewer (ctx: UpdateContext) =
    inputs <- processInput ctx
    StackPanel.create [
        LayoutSet.modifiers [
            top 100f >> left 200f
            margin { Top = 100f; Right = 20f; Bottom = 10f; Left = 50f }
            padding { Top = 25f; Right = 40f; Bottom = 25f; Left = 40f }
        ]
        BoxSet.backgroundColor Color.Blue
        BoxSet.borderColor Color.DarkBlue
        BoxSet.borderWidth 10f
        StackPanelSet.orientation Orientation.Vertical
        PanelSet.children
            (if inputs.IsEmpty then
                [tip]
            else
                inputs)
    ]
