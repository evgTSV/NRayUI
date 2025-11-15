namespace NRayUI.Input

open System.Numerics
open Raylib_CSharp.Interact
open NRayUI.Time

type InputEvent =
    | KeyPressed of KeyboardKey
    | KeyReleased of KeyboardKey
    | MouseClicked of key: MouseButton * pos: Vector2
    | MouseMoved of delta: Vector2 * endPoint: Vector2
    | MouseScrolled of delta: float32

type FrameInput = {
    Inputs: InputEvent[]
    Time: FrameTime
} with

    static member Zero = { Inputs = [||]; Time = FrameTime.Zero }

type InputRecorder = { Records: FrameInput list }

module UserInput =

    let mutable private keyboardKeyPressed: InputEvent option = None

    let getKeyPressed () =
        let key = enum<KeyboardKey> <| Input.GetKeyPressed()

        match key with
        | KeyboardKey.Null -> None
        | _ ->
            let event = Some <| KeyPressed key
            keyboardKeyPressed <- event
            event

    let getKeyReleased () =
        match keyboardKeyPressed with
        | Some(KeyPressed k) when Input.IsKeyReleased k ->
            keyboardKeyPressed <- None
            Some <| KeyReleased k
        | _ -> None

    let mousePressed (button: MouseButton) =
        if Input.IsMouseButtonPressed button then
            let pos = Input.GetMousePosition()
            Some <| MouseClicked(button, pos)
        else
            None

    // TODO: Can raylib determine a few clicks? If not, then use some workarounds
    let getMousePressed () =
        let events = MouseButton.GetValues() |> Array.map mousePressed
        events

    let getMouseScrolling () =
        let delta = Input.GetMouseWheelMove()
        if delta = 0f then None else Some <| MouseScrolled delta

    let getMouseMovement () =
        let delta = Input.GetMouseDelta()

        if delta = Vector2.Zero then
            None
        else
            let pos = Input.GetMousePosition()
            Some <| MouseMoved(delta, pos)

    let handleInput (time: FrameTime) : FrameInput = {
        Inputs =
            [|
                yield getKeyPressed ()
                yield getKeyReleased ()
                yield getMouseScrolling ()
                yield getMouseMovement ()
                yield! getMousePressed ()
            |]
            |> Array.choose id
        Time = time
    }

    let getMousePosition() =
        Input.GetMousePosition()
