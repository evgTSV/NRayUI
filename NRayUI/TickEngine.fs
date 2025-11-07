namespace NRayUI

open NRayUI.Time

type TickEngine = {
    LastProcessedTickTime: FrameTime
    TicksProcessed: uint64
} with

    static member Create(start: FrameTime) = {
        LastProcessedTickTime = start
        TicksProcessed = 0UL
    }

    member this.Update(time: FrameTime, state: 'state, update: 'state -> 'state) =
        let deltaFromLastUpdate = time - this.LastProcessedTickTime
        let ticksPassed = deltaFromLastUpdate.Tick
        let newTime = ticksPassed |> time.AddTicks

        let rec processTicks (tick: uint64) (acc: 'state) =
            if tick <= 0UL then
                acc
            else
                processTicks (tick - 1UL) (update acc)

        processTicks ticksPassed state,
        {
            LastProcessedTickTime = newTime
            TicksProcessed = this.TicksProcessed + ticksPassed
        }
