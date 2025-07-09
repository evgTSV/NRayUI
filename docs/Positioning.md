# Positioning in NRayUI

NRayUI provides four types of element positioning, similar to CSS:

- **Static** (default)
- **Relative**
- **Absolute**
- **Fixed**

## Static (default)

Elements with `Static` positioning are placed in the document flow one after another, with no offsets. This is the default behavior for all elements unless specified otherwise.

## Relative

An element with `Relative` positioning is positioned relative to its normal position in the flow. You can set offsets (`top`, `left`, `right`, `bottom`) that will be applied to its original position.

## Absolute

An element with `Absolute` positioning is positioned relative to the nearest ancestor with a positioning other than `Static`. If there is no such ancestor, it is positioned relative to the window. Offsets (`top`, `left`, `right`, `bottom`) define its exact position.

## Fixed

An element with `Fixed` positioning is positioned relative to the application window (the entire scene), independent of scrolling or other elements. Use offsets to set its position.

## Offset Properties

For positioning, you can use the following functions:
- `top v`
- `left v`
- `right v`
- `bottom v`

Values are set in pixels.

## Combining Example

```fsharp
StackPanel.create [
    LayoutSet.modifiers [
        position Relative
        top 10
        left 20
        marginScan "10 9 8 7"
        paddingScan "20"
    ]
    BoxSet.backgroundColor Color.Blue
    BoxSet.crScan "10 9 8 7"
    StackPanelSet.orientation Orientation.Vertical
    StackPanelSet.children [
        TextBox.create [ 
            LayoutSet.modifiers [ 
                position Static
                left 50
                top 10 
                margin {Top = 10; Right = 20; Bottom = 30; Left = 40} 
            ]
            TextBoxSet.text "Hello, World!" 
        ]
        Button.create [
            LayoutSet.modifiers [ 
                position Relative
                top 20
                left 30 
            ]
            ButtonSet.text "Click Me"
            ButtonSet.onClick (fun _ -> printfn "Button clicked!")
        ]
    ]
]
```

## Window Positioning

Window is the main container for all elements in NRayUI. You can set its size. Default zindex is -1, so it is always behind other elements.