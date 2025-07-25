# Declarative UI in F# with Raylib

## Introduction

This document describes an approach to creating declarative user interfaces in F# using the Raylib library. The provided example demonstrates building a hierarchy of UI elements through composition and modifiers.

## Core Concepts

### 1. UI Composition
Interfaces are constructed from nested components, where each component can contain other components.

### 2. Modifiers
Element properties are configured through modifier chains, ensuring flexibility and code readability.

### 3. Typed Modifier Sets
Different element types have their own specific modifier sets (Layout, Box, StackPanel, etc.)

## Code Example

```fsharp
let test =
    StackPanel.create [
        LayoutSet.modifiers [
            top 100f
            left 200f
            margin { Top = 10f; Right = 20f; Bottom = 10f; Left = 20f }
            padding { Top = 5f; Right = 5f; Bottom = 5f; Left = 5f }
        ]
        LayoutSet.height 500f >> LayoutSet.width 250f
        BoxSet.backgroundColor Color.Blue
        BoxSet.borderColor Color.DarkBlue
        BoxSet.borderWidth 10f
        BoxSet.cornerRadius { TopLeft = 0f; TopRight = 1.f; BottomLeft = 0.5f; BottomRight = 0.1f }
        StackPanelSet.orientation Orientation.Vertical
        PanelSet.children [
            Canvas.create [
                LayoutSet.modifiers [
                    top 5f
                    left 5f
                    margin { Top = 5f; Right = 5f; Bottom = 5f; Left = 5f }
                ]
                LayoutSet.height 20f >> LayoutSet.width 20f
                BoxSet.backgroundColor Color.Red
            ]
        ]
    ]
```

## Element Descriptions

### StackPanel
A container that arranges child elements in a stack (vertically or horizontally).

Modifiers:
- `StackPanelSet.orientation` - sets stack direction (Vertical/Horizontal)
- `PanelSet.children` - list of child elements

### Canvas
A simple rectangular element for rendering.

### Common Modifiers

#### LayoutSet (Elements with layout)
- `modifiers` - complex positioning parameters
    - `top`, `left` - absolute positioning
    - `margin` - outer spacing
    - `padding` - inner spacing
- `height`, `width` - element dimensions

#### BoxSet (Elements with box)
- `backgroundColor` - background color
- `borderColor` - border color
- `borderWidth` - border thickness
- `cornerRadius` - corner rounding radius (can be set individually per corner)