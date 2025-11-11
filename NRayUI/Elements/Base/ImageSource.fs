namespace NRayUI.Elements

open System
open System.Numerics
open NRayUI.Icons
open NRayUI.RenderBase
open Raylib_CSharp.Colors
open Raylib_CSharp.Images
open Raylib_CSharp.Textures
open type Raylib_CSharp.Rendering.Graphics

[<Interface>]
type IImageSource =
    abstract Height: float32
    abstract Width: float32
    abstract Draw: pos: Vector2 * tint: Color -> RenderAction

type TextureSource(texture: Texture2D) =
    interface IImageSource with
        member this.Draw(pos, tint) =
            fun _ -> DrawTextureV(texture, pos, tint)
            |> withScissor

        member this.Height = texture.Height |> float32
        member this.Width = texture.Width |> float32

type IconSource(icon: Icon, size: int) =
    interface IImageSource with
        member this.Draw(pos, tint) =
            {
                Icon = icon
                Size = size
                Color = tint
            }
            |> drawIcon pos
            |> withScissor

        member this.Height = (getIconAbsoluteSizes size).Y
        member this.Width = (getIconAbsoluteSizes size).X

type ImageSource(image: Image) =
    let cachedTex = lazy Texture2D.LoadFromImage(image)
    let mutable disposed = false

    interface IImageSource with
        member this.Draw(pos, tint) =
            fun _ ->
                let tex = cachedTex.Value
                DrawTextureV(tex, pos, tint)
            |> withScissor

        member this.Height = image.Height |> float32
        member this.Width = image.Width |> float32

    interface IDisposable with
        member this.Dispose() =
            if not disposed then
                disposed <- true

                if cachedTex.IsValueCreated then
                    cachedTex.Value.Unload()

                GC.SuppressFinalize(this)

    override this.Finalize() =
        try
            (this :> IDisposable).Dispose()
        with _ ->
            ()

module EmptySource =
    let emptySource =
        { new IImageSource with
            member this.Draw(_, _) = fun _ -> ()
            member this.Height = 0f
            member this.Width = 0f
        }
