module NRayUI.RlGlUtils

open Raylib_CSharp
open Raylib_CSharp.Colors
open Raylib_CSharp.Rendering.Gl

let withRlGlMode (mode: DrawMode) (next: unit -> unit) =
    fun () ->
        RlGl.Begin(mode)
        try
            next()
        finally
            RlGl.End()
        
let withRlGlTexture id (next: unit -> unit) =
    fun () ->
        RlGl.SetTexture id
        try
            next()
        finally
            RlGl.SetTexture(id)
        
let withRlGlColor4ub (color: Color) (next: unit -> unit) =
    fun () ->
        RlGl.Color4ub (color.R, color.G, color.B, color.A)
        next()