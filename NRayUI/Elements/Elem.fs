namespace NRayUI.Elements

open System
open FSharpPlus
open NRayUI.Modifier
open Raylib_CSharp.Fonts
open Raylib_CSharp.Textures
open type Raylib_CSharp.Rendering.Graphics

type IElem =
    abstract member Layout: Layout with get, set
    abstract member Render: parent: IElem -> unit
    abstract member Update: unit -> unit
    
module Elem = ()