namespace NRayUI.Services

open System
open System.Collections.Concurrent
open Raylib_CSharp.Transformations

type ButtonState = { Bounds: Rectangle }

type UIStateService() =
    let buttonStates = ConcurrentDictionary<int64, ButtonState>()

    member this.SetButtonState(hash, state) = buttonStates[hash] <- state

    member this.TryGetButtonState(hash) =
        match buttonStates.TryGetValue(hash) with
        | true, state -> Some state
        | false, _ -> None

    interface IDisposable with
        member this.Dispose() = buttonStates.Clear()
