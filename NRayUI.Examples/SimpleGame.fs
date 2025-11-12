module NRayUI.Examples.SimpleGame

open System
open System.Numerics
open NRayUI.Components.UIConfigurator
open NRayUI.Elements
open NRayUI.Elements.Panels
open NRayUI.Icons
open NRayUI.Input
open NRayUI.Field
open NRayUI.Modifier
open NRayUI.RenderBase
open Raylib_CSharp.Colors
open Raylib_CSharp.Interact
open Raylib_CSharp.Transformations

let private levelSize = Vector2(500f)
let private playerSize = 5
let private coinSize = 2

let private maxVelocity = 2f

let mutable private playerPos = Vector2(200f, 200f)

let private getCoinPos() = Vector2 (
    Random.Shared.Next(0, levelSize.X - (getIconAbsoluteSizes coinSize).X - 1f |> int) |> float32,
    Random.Shared.Next(0, levelSize.Y - (getIconAbsoluteSizes coinSize).Y - 1f |> int) |> float32
)

let mutable private coinPos = getCoinPos()
    
let mutable private velocity = Vector2(0f, 0f)
let mutable private score = 0
let mutable private collected = 0


let private handlePlayerInput (input: InputEvent) =
    match input with
    | KeyPressed k ->
        match k with
        | KeyboardKey.Up ->
            velocity <- velocity - Vector2(0f, 1f)
        | KeyboardKey.Down ->
            velocity <- velocity + Vector2(0f, 1f)
        | KeyboardKey.Left ->
            velocity <- velocity - Vector2(1f, 0f)
        | KeyboardKey.Right ->
            velocity <- velocity + Vector2(1f, 0f)
        | _ -> ()
    | _ -> ()
    
let private checkCollision (newPos: Vector2) =
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
        collected <- collected + 1
        coinPos <- getCoinPos()
    | None -> ()
    
    if newPos <> playerPos then
        velocity <- Vector2.Zero

let private updatePlayer (ctx: UpdateContext) =
    ctx.Input
    |> Array.iter handlePlayerInput
    
    velocity <- Vector2 (
        Math.Clamp(velocity.X,  -maxVelocity, maxVelocity),
        Math.Clamp(velocity.Y,  -maxVelocity, maxVelocity)
    )
    
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
    
let coin() =
    ImageBox.create [
        ImageBoxSet.source (IconSource(Icon.Coin, coinSize))
        ImageBoxSet.tint Color.Black
        BoxSet.backgroundColor Color.Blank
        BoxSet.borderColor Color.Red
        LayoutSet.width 10f
        LayoutSet.height 10f
        LayoutSet.position coinPos
    ]
    
let hud() =
    StackPanel.create [
        LayoutSet.modifiers [
            position (Vector2(10f))
            padding { Top = 10f; Bottom = 10f; Left = 20f; Right = 20f }
        ]
        LayoutSet.zIndex 2
        BoxSet.crScan "0.5"
        BoxSet.backgroundColor (Color.FromHex(0x07FFFF1Fu))
        StackPanelSet.spacing 100f
        StackPanelSet.orientation Orientation.Horizontal
        PanelSet.children [
            Label.create [
                BoxSet.backgroundColor Color.Blank
                BoxSet.borderWidth 0f
                TextSet.content $"Score: {score}"
            ]
            Label.create [
                BoxSet.backgroundColor Color.Blank
                BoxSet.borderWidth 0f
                TextSet.content $"Coins: {collected}"
            ]
        ]
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
            coin()
            hud()
        ]
    ]
