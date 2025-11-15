module NRayUI.StateService

open System.Collections.Concurrent
open System.Collections.Generic

type IState<'T> =
    abstract member Current: 'T
    abstract member Set: 'T -> unit
    abstract member Update: ('T -> 'T) -> unit

type State<'T>(initialValue: 'T, onChanged: 'T -> unit) =
    let mutable value = initialValue

    interface IState<'T> with
        member this.Current = value

        member this.Set(newValue: 'T) =
            if not (EqualityComparer.Default.Equals(value, newValue)) then
                value <- newValue
                onChanged newValue

        member this.Update(updater: 'T -> 'T) =
            let newValue = updater value

            if not (EqualityComparer.Default.Equals(value, newValue)) then
                value <- newValue
                onChanged newValue

type IStateManager =
    abstract member UseState<'T> : string * 'T -> IState<'T>
    abstract member ResetFrameState: unit -> unit

type StateManager() =
    let states = ConcurrentDictionary<string, obj>()
    let frameStates = ConcurrentDictionary<string, obj>()

    interface IStateManager with
        member this.UseState<'T>(key: string, initialValue: 'T) =
            let fullKey = $"{key}_{typeof<'T>.Name}"

            let state =
                match states.TryGetValue(fullKey) with
                | true, existing -> existing :?> IState<'T>
                | false, _ ->
                    let newState =
                        State<'T>(initialValue, fun newValue -> frameStates[fullKey] <- newValue)

                    states[fullKey] <- newState
                    newState

            match frameStates.TryGetValue(fullKey) with
            | true, frameValue -> state.Set(frameValue :?> 'T)
            | false, _ -> ()

            state

        member this.ResetFrameState() = frameStates.Clear()
