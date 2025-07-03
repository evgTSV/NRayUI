module NRayUI.RenderingUtils

open System
open System.Numerics
open NRayUI.Field
open NRayUI.Positioning
open Raylib_CSharp
open Raylib_CSharp.Colors
open Raylib_CSharp.Textures
open Raylib_CSharp.Rendering
open Raylib_CSharp.Rendering.Gl
open type Raylib_CSharp.Rendering.Graphics

let DrawTexturePoly
    (texture: Texture2D)
    (center: Vector2)
    (pointCoords: Vector2[])
    (texCoords: Vector2[])
    (tint: Color)
    : unit =
    let pointCount = pointCoords.Length
    if pointCount <> texCoords.Length then
        failwith "point count != texcoord count"
    if pointCount = 0 then
        ()

    Graphics.BeginBlendMode(BlendMode.AlphaPremultiply)
    RlGl.Begin(DrawMode.Triangles)
    
    RlGl.SetTexture(texture.Id)
    RlGl.Color4ub(tint.R, tint.G, tint.B, tint.A)
    
    for i in 0..pointCount-2 do
        RlGl.TexCoord2F(0.5f, 0.5f)
        RlGl.Vertex2F(center.X, center.Y)

        RlGl.TexCoord2F(texCoords[i].X, texCoords[i].Y)
        RlGl.Vertex2F(pointCoords[i].X + center.X, pointCoords[i].Y + center.Y)
        
        RlGl.TexCoord2F(texCoords[i + 1].X, texCoords[i + 1].Y)
        RlGl.Vertex2F(pointCoords[i + 1].X + center.X, pointCoords[i + 1].Y + center.Y)
        
    RlGl.End()
    RlGl.SetTexture(0u)
    
    Graphics.EndBlendMode()
    
let DrawRectanglePoly
    (center: Vector2)
    (pointCoords: Vector2[])
    (tint: Color)
    : unit =
    let pointCount = pointCoords.Length
    if pointCount < 3 then
        failwith "point count < 3"

    let texCoords = Array.init pointCount (fun i -> Vector2(float32 i / float32 (pointCount - 1), 0.5f))
    DrawTexturePoly (Texture2D()) center pointCoords texCoords tint
    
let DrawRectangleCustomRoundedLines 
    (rec_: Transformations.Rectangle) 
    (cornerRadius: Corners) 
    (thickness: float32)
    (segments: int)
    (color: Color) 
    : unit =
        
    let point = Vector2(rec_.X, rec_.Y)
    let width = rec_.Width
    let height = rec_.Height
    
    let topLine = (
        Vector2(point.X + cornerRadius.TopLeft, point.Y),
        Vector2(point.X + width - cornerRadius.TopRight, point.Y))
    let bottomLine = (
        Vector2(point.X + cornerRadius.BottomLeft, point.Y + height),
        Vector2(point.X + width - cornerRadius.BottomRight, point.Y + height))
    let leftLine = (
        Vector2(point.X, point.Y + cornerRadius.TopLeft),
        Vector2(point.X, point.Y + height - cornerRadius.BottomLeft))
    let rightLine = (
        Vector2(point.X + width, point.Y + cornerRadius.TopRight),
        Vector2(point.X + width, point.Y + height - cornerRadius.BottomRight))
    
    [| topLine; bottomLine; leftLine; rightLine |]
    |> Array.iter (fun (start, end_) ->
        DrawLineEx(start, end_, thickness, color)
    )
    
    [|
        (cornerRadius.TopLeft, Vector2(point.X + cornerRadius.TopLeft, point.Y + cornerRadius.TopLeft), 180.0f, 270.0f)
        (cornerRadius.TopRight, Vector2(point.X + width - cornerRadius.TopRight, point.Y + cornerRadius.TopRight), 270.0f, 360.0f)
        (cornerRadius.BottomRight, Vector2(point.X + width - cornerRadius.BottomRight, point.Y + height - cornerRadius.BottomRight), 0.0f, 90.0f)
        (cornerRadius.BottomLeft, Vector2(point.X + cornerRadius.BottomLeft, point.Y + height - cornerRadius.BottomLeft), 90.0f, 180.0f)
    |] 
    |> Array.iter (fun (radius, center, startAngle, endAngle) ->
        if radius > 0.0f then
            DrawCircleSectorLines(center, radius, startAngle, endAngle, segments, color)
    )