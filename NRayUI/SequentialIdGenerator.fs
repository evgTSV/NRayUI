module NRayUI.SequentialIdGenerator

open System.Threading

let mutable private currentId = 0L

/// <summary>
/// Gets the next sequential ID
/// </summary>
let getNextId () = Interlocked.Add(&currentId, 1L)

/// <summary>
/// Gets the current ID without incrementing the counter
/// </summary>
let getCurrentId () = Interlocked.Read(&currentId)

/// <summary>
/// Resets the ID sequence to the specified value (default: 0)
/// </summary>
let reset () =
    Interlocked.Exchange(&currentId, 0L) |> ignore
