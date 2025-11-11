namespace NRayUI.Control

open NRayUI.Elements
open NRayUI.Modifier
open NRayUI.RenderBase
open NRayUI.Time

type View<'a when 'a :> IElem and IWithLayout<'a>> = UpdateContext -> 'a

type UIMsg<'a when 'a :> IElem and IWithLayout<'a>> =
    | Pause of pauseView: View<'a>
    | Unpause
    | Delay of time: FrameTime
    | Fail of text: string

type Dispatcher<'a when 'a :> IElem and IWithLayout<'a>> = { UICmd: UIMsg<'a> }
