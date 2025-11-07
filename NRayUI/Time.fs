namespace NRayUI.Time

open System
open NRayUI.Constants
open Raylib_CSharp

[<Measure>]
type s

type FrameTime = {
    TimeElapsed: float<s>
} with

    member this.Tick = this.TimeElapsed * TicksPerSecond |> float |> Math.Abs |> uint64

    member this.AddSeconds(value: float<s>) = {
        this with
            TimeElapsed = this.TimeElapsed + value
    }

    member this.AddTicks(value: uint64) =
        let sec = 1.<s> / TicksPerSecond * float value
        this.AddSeconds(sec)

    static member Zero = { TimeElapsed = 0.<s> }

    static member OfSeconds(value: float<s>) = { TimeElapsed = value }

    static member (-)(t1: FrameTime, t2: FrameTime) =
        t1.TimeElapsed - t2.TimeElapsed |> FrameTime.OfSeconds

    static member Now() =
        Time.GetTime() * 1.<s> |> FrameTime.OfSeconds
