namespace NRayUI.Elements

open NRayUI.Field

module Panels =
    open Raylib_CSharp.Colors
    open NRayUI.Positioning.Alignment
    open NRayUI.Modifier

    type StackPanel = {
        Layout: Layout
        Orientation: Orientation
        Box: Box
    }
    