module NRayUI.Utils

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.Numerics

type Vector2Comparer() =
    interface IEqualityComparer<Vector2> with
        member _.Equals(v1, v2) =
            v1.X = v2.X && v1.Y = v2.Y
        member _.GetHashCode(v) =
            hash (v.X, v.Y)

let inline deg2rad (degrees: float32) : float32 =
    degrees * MathF.PI / 180.0f
    
let lazyMemoize<'a, 'b> (cmp: IEqualityComparer<'a>) f =
    let cache = ConcurrentDictionary<'a, Lazy<'b>>(cmp)
    fun arg ->
        cache.GetOrAdd(arg, fun a -> Lazy<'b>(fun () -> f a)).Value