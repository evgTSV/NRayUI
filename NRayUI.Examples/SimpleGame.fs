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
open NRayUI.StateService
open Raylib_CSharp.Colors
open Raylib_CSharp.Interact
open Raylib_CSharp.Transformations

// Constants

let private levelSize = Vector2(500f)
let private playerSize = 5
let private coinSize = 2
let private maxVelocity = 2f
let private pointsForCoin = 10


let private getCoinPos() = Vector2 (
    Random.Shared.Next(0, levelSize.X - (getIconAbsoluteSizes coinSize).X - 1f |> int) |> float32,
    Random.Shared.Next(0, levelSize.Y - (getIconAbsoluteSizes coinSize).Y - 1f |> int) |> float32
)

type Player = {
    TopLeft: Vector2
    Velocity: Vector2
    Score: int
    Collected: int
} with
    static member Init() = {
        TopLeft = Vector2(250f)
        Velocity = Vector2.Zero
        Score = 0
        Collected = 0
    }

type GameState = {
    Player: Player
    CoinPos: Vector2
} with
    static member Init() = {
        Player = Player.Init()
        CoinPos = getCoinPos()
    }

let private handlePlayerInput (input: InputEvent) =
    match input with
    | KeyPressed k ->
        match k with
        | KeyboardKey.Up ->
            Vector2(0f, -1f)
        | KeyboardKey.Down ->
            Vector2(0f, 1f)
        | KeyboardKey.Left ->
            Vector2(-1f, 0f)
        | KeyboardKey.Right ->
            Vector2(1f, 0f)
        | _ -> Vector2.Zero
    | _ -> Vector2.Zero
    
let private checkCollision (game: GameState) (newPos: Vector2) =
    let playerSizeAbs = getIconAbsoluteSizes playerSize
    let coinSizeAbs = getIconAbsoluteSizes coinSize
    
    let playerPos = Vector2(
            Math.Clamp(newPos.X, 0f, levelSize.X - playerSizeAbs.X - 1f),
            Math.Clamp(newPos.Y, 0f, levelSize.Y - playerSizeAbs.Y - 1f))
    
    let playerRect = Rectangle(
            playerPos.X,
            playerPos.Y,
            playerSizeAbs.X,
            playerSizeAbs.Y
        )
    
    let coinRect = Rectangle(
            game.CoinPos.X,
            game.CoinPos.Y,
            coinSizeAbs.X,
            coinSizeAbs.Y
        )
    
    let game =
        match playerRect <&&> coinRect with
        | Some _ ->
            let player = 
                { game.Player with
                    Score = game.Player.Score + pointsForCoin
                    Collected = game.Player.Collected + 1 }
            { game with
                Player = player
                CoinPos = getCoinPos() }
        | None -> game
    
    let velocity =
        if newPos <> playerPos
        then Vector2.Zero
        else game.Player.Velocity
        
    { game with
        GameState.Player.Velocity = velocity
        GameState.Player.TopLeft = playerPos }

let private updatePlayer (game: GameState) (ctx: UpdateContext) =
    let player = game.Player
    
    let playerVelDelta =
        ctx.Input
        |> Array.map handlePlayerInput
        |> Array.sum
    
    let velocity = Vector2 (
        Math.Clamp(player.Velocity.X + playerVelDelta.X, -maxVelocity, maxVelocity),
        Math.Clamp(player.Velocity.Y + playerVelDelta.Y, -maxVelocity, maxVelocity)
    )
    
    let game = {
        game with
            GameState.Player.Velocity = velocity
    }
    
    let newPos = player.TopLeft + velocity
    checkCollision game newPos
    
let playerView (game: GameState) =
    ImageBox.create [
        ImageBoxSet.source (IconSource(Icon.Player, playerSize))
        ImageBoxSet.tint Color.Black
        BoxSet.backgroundColor Color.Blank
        BoxSet.borderColor Color.Red
        LayoutSet.width 10f
        LayoutSet.height 10f
        LayoutSet.position game.Player.TopLeft
        LayoutSet.zIndex 1
    ]
    
let coinView (game: GameState) =
    ImageBox.create [
        ImageBoxSet.source (IconSource(Icon.Coin, coinSize))
        ImageBoxSet.tint Color.Black
        BoxSet.backgroundColor Color.Blank
        BoxSet.borderColor Color.Red
        LayoutSet.width 10f
        LayoutSet.height 10f
        LayoutSet.position game.CoinPos
    ]
    
let hudView (game: GameState) =
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
                TextSet.content $"Score: {game.Player.Score}"
            ]
            Label.create [
                BoxSet.backgroundColor Color.Blank
                BoxSet.borderWidth 0f
                TextSet.content $"Coins: {game.Player.Collected}"
            ]
        ]
    ]

let gameField (ctx: UpdateContext) =
    let state = ctx.UseState(GameState.Init())
    
    let game = state.Current
    state.Set(updatePlayer game ctx)
    let game = state.Current
    
    Canvas.create [
        LayoutSet.modifiers [
            size levelSize
            marginScan "50"
        ]
        BoxSet.backgroundColor Color.DarkGreen
        BoxSet.borderColor Color.Black
        BoxSet.borderWidth 10f
        PanelSet.children [
            playerView game
            coinView game
            hudView game
        ]
    ]
