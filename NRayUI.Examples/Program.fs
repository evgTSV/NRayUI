
open System
open System.Numerics
open NRayUI
open NRayUI.Components.UIConfigurator
open NRayUI.Elements
open NRayUI.Elements.Panels
open NRayUI.Field
open NRayUI.Icons
open NRayUI.Input
open NRayUI.Modifier
open NRayUI.RenderBase
open NRayUI.Window
open NRayUI.Positioning
open NRayUI.UIRendering
open Raylib_CSharp.Colors
open Raylib_CSharp.Interact
open type Raylib_CSharp.Rendering.Graphics
open Raylib_CSharp.Transformations

let test (ctx: UpdateContext) =
    StackPanel.create [
        LayoutSet.modifiers [
            top 100f >> left 200f
            margin { Top = 100f; Right = 20f; Bottom = 10f; Left = 50f }
            padding { Top = 25f; Right = 5f; Bottom = 25f; Left = 20f }
        ]
        LayoutSet.height 500f >> LayoutSet.width 250f
        BoxSet.backgroundColor Color.Blue
        BoxSet.borderColor Color.DarkBlue
        BoxSet.borderWidth 10f
        BoxSet.cornerRadius { TopLeft = 0f; TopRight = 1f; BottomLeft = 0.5f; BottomRight = 0.1f }
        StackPanelSet.orientation Orientation.Vertical
        PanelSet.children [
            Label.create [
                TextSet.content "Hello!"
                TextSet.color Color.Red
                TextSet.fontSize 60f
                LayoutSet.modifiers [
                    paddingScan "0 0 0 20"
                    width 200f >> height 120f
                ]
                BoxSet.cornerRadius { Corners.zero with TopRight = 1f }
            ]
            ImageBox.create [
                BoxSet.backgroundColor Color.Blank
                BoxSet.borderWidth 5f
                ImageBoxSet.source (IconSource(Icon.CPU, 5))
                ImageBoxSet.tint Color.RayWhite
                LayoutSet.modifiers [
                    padding { Padding.zero with Top = 8f; Left = 8f }
                    width 100f >> height 100f
                ]
            ]
            Label.create [
                TextSet.content $"L{String('o', 50)}ng"
                TextSet.color Color.Black
                LayoutSet.modifiers [
                    paddingScan "0 0 0 30"
                    width 800f >> height 70f
                ]
                BoxSet.backgroundColor Color.White
                BoxSet.crScan "1"
            ]
            ImageBox.create [
                BoxSet.backgroundColor Color.Blank
                BoxSet.borderColor Color.Red
                ImageBoxSet.texture (ctx.Resources.LoadTexture @"./Assets/nrayui_logo_100x100.png")
                ImageBoxSet.tint Color.RayWhite
                LayoutSet.modifiers [
                    paddingScan "10"
                    width 120f >> height 120f
                ]
            ]
        ]
    ]
    
let inputToText (input: InputEvent) =
    match input with
    | KeyPressed k ->
        k.ToString()
    | _ ->
        "None"

let mutable inputs: IElem list = []

let addToFixedSizeList (lst: 'a list) (items: 'a list) : 'a list =
    items @ lst
    |> List.truncate 7

let processInput (ctx: UpdateContext): IElem list =
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
                    paddingScan "0 0 0 20"
                    width 150f >> height 50f
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
            padding { Top = 25f; Right = 25f; Bottom = 25f; Left = 40f }
        ]
        LayoutSet.height 500f >> LayoutSet.width 250f
        BoxSet.backgroundColor Color.Blue
        BoxSet.borderColor Color.DarkBlue
        BoxSet.borderWidth 10f
        StackPanelSet.orientation Orientation.Vertical
        PanelSet.children
            inputs
    ]
    
let levelSize = Vector2(500f)
let playerSize = 5
let coinSize = 2

let mutable playerPos = Vector2(0f, 0f)

let getCoinPos() = Vector2(Random.Shared.Next(0, levelSize.X - (getIconAbsoluteSizes coinSize).X - 1f |> int) |> float32)
let mutable coinPos = getCoinPos()
    
let mutable velocity = Vector2(0f, 0f)
let mutable score = 0


let handlePlayerInput (input: InputEvent) =
    match input with
    | KeyPressed k ->
        match k with
        | KeyboardKey.Up -> velocity <- velocity - Vector2(0f, 1f)
        | KeyboardKey.Down -> velocity <- velocity + Vector2(0f, 1f)
        | KeyboardKey.Left -> velocity <- velocity - Vector2(1f, 0f)
        | KeyboardKey.Right -> velocity <- velocity + Vector2(1f, 0f)
        | _ -> ()
    | _ -> ()
    
let checkCollision (newPos: Vector2) =
    let playerSizeAbs = getIconAbsoluteSizes playerSize
    let coinSizeAbs = getIconAbsoluteSizes coinSize
    
    playerPos <- Vector2(
            Math.Clamp(newPos.X, 0f, levelSize.X - playerSizeAbs.X - 1f),
            Math.Clamp(newPos.Y, 0f, levelSize.Y - playerSizeAbs.Y - 1f))
    
    let playerRect = Rectangle(
            playerPos.X,
            playerPos.Y,
            playerSizeAbs.X,
            playerSizeAbs.Y
        )
    
    let coinRect = Rectangle(
            coinPos.X,
            coinPos.Y,
            coinSizeAbs.X,
            coinSizeAbs.Y
        )
    
    match playerRect <&&> coinRect with
    | Some _ ->
        score <- score + 10
        coinPos <- getCoinPos()
    | None -> ()
    
    if newPos <> playerPos then
        velocity <- Vector2.Zero

let updatePlayer (ctx: UpdateContext) =
    ctx.Input
    |> Array.iter handlePlayerInput
    
    let newPos = playerPos + velocity
    checkCollision newPos
    
    ImageBox.create [
        ImageBoxSet.source (IconSource(Icon.Player, playerSize))
        ImageBoxSet.tint Color.Black
        BoxSet.backgroundColor Color.Blank
        BoxSet.borderColor Color.Red
        LayoutSet.width 10f
        LayoutSet.height 10f
        LayoutSet.position playerPos
        LayoutSet.zIndex 1
    ]

let gameField (ctx: UpdateContext) =
    let player = updatePlayer ctx
    Canvas.create [
        LayoutSet.modifiers [
            size levelSize
            marginScan "50"
        ]
        BoxSet.backgroundColor Color.DarkGreen
        BoxSet.borderColor Color.Black
        BoxSet.borderWidth 10f
        PanelSet.children [
            player
            ImageBox.create [
                ImageBoxSet.source (IconSource(Icon.Coin, coinSize))
                ImageBoxSet.tint Color.Black
                BoxSet.backgroundColor Color.Blank
                BoxSet.borderColor Color.Red
                LayoutSet.width 10f
                LayoutSet.height 10f
                LayoutSet.position coinPos
            ]
            Label.create [
                LayoutSet.modifiers [
                    height 20f
                    position (Vector2(10f))
                ]
                LayoutSet.zIndex 2
                TextSet.content $"Score: {score}"
            ]
        ]
    ]

let builder = UIBuilder()

%builder
    .WithWindow({
        Title = "NRayUI example"
        WindowSizePx = (800, 800)
    })
    
let app = builder.Build()

gameField |> startRendering app
