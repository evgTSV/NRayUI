namespace NRayUI.Elements

open NRayUI.Elements.Elem
open type Raylib_CSharp.Rendering.Graphics

[<AutoOpen>]
module Panel =
    type IPanel<'a> =
        inherit IWithBox<'a>
        abstract member GetChildren: IElem list
        abstract member SetChildren: IElem list -> 'a
