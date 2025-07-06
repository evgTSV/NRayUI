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
    
let DrawCircleSectorLinesEx (center: Vector2) (radius: float32) (startAngle: float32) (endAngle: float32) (segments: int) (thickness: float32) (color: Color) =
    let segments =
        if segments < 4 then
            let th = Math.Acos(2. * (Math.Pow(1f - Constants.SmoothCircleErrorRate |> float, 2.) - 1.))
            Math.Ceiling((endAngle - startAngle |> float) / th) |> int
        else segments
    
    let step = (endAngle - startAngle) / float32 segments
    
    for i in 0..(segments - 1) do
        let line =
            Array.init 2 (fun j ->
                let angle = startAngle + float32 (i + j) * step
                let rad = angle * MathF.PI / 180.0f
                Vector2(
                    center.X + MathF.Cos(rad) * radius,
                    center.Y + MathF.Sin(rad) * radius))
            
        DrawLineEx(line[0], line[1], thickness, color)

let DrawRectangleCustomRounded
    (rec_: Transformations.Rectangle) 
    (cornerRadius: Corners) 
    (segments: int)
    (color: Color) 
    : unit =
        
    DrawRectangleRounded(rec_, cornerRadius.TopRight, segments, color)

let DrawRectangleCustomRoundedLines 
    (rec_: Transformations.Rectangle) 
    (roundness: Corners) 
    (thickness: float32)
    (segments: int)
    (color: Color) 
    : unit =
        
    let point = Vector2(rec_.X, rec_.Y)
    let width = rec_.Width
    let height = rec_.Height
    
    let rTopLeft = Math.Clamp(roundness.TopLeft, 0f, 1f) * Math.Min(width, height) / 2f
    let rTopRight = Math.Clamp(roundness.TopRight, 0f, 1f) * Math.Min(width, height) / 2f
    let rBottomRight = Math.Clamp(roundness.BottomRight, 0f, 1f) * Math.Min(width, height) / 2f
    let rBottomLeft = Math.Clamp(roundness.BottomLeft, 0f, 1f) * Math.Min(width, height) / 2f
    
    let topLine =
        (Vector2(point.X + rTopLeft, point.Y),
        Vector2(point.X + width - rTopRight, point.Y))
    let bottomLine = (
        Vector2(point.X + rBottomLeft, point.Y + height),
        Vector2(point.X + width - rBottomRight, point.Y + height))
    let leftLine = (
        Vector2(point.X, point.Y + rTopLeft),
        Vector2(point.X, point.Y + height - rBottomLeft))
    let rightLine = (
        Vector2(point.X + width, point.Y + rTopRight),
        Vector2(point.X + width, point.Y + height - rBottomRight))
    
    [| topLine; bottomLine; leftLine; rightLine |]
    |> Array.iter (fun (start, end_) ->
        DrawLineEx(start, end_, thickness, color)
    )
    
    [|
        (Vector2(point.X + rTopLeft, point.Y + rTopLeft), rTopLeft, 180.0f, 270.0f)
        (Vector2(point.X + width - rTopRight, point.Y + rTopRight), rTopRight, 270.0f, 360.0f)
        (Vector2(point.X + width - rBottomRight, point.Y + height - rBottomRight), rBottomRight, 0.0f, 90.0f)
        (Vector2(point.X + rBottomLeft, point.Y + height - rBottomLeft), rBottomLeft, 90.0f, 180.0f)
    |] 
    |> Array.iter (fun (center, radius, startAngle, endAngle) ->
        if radius > 0.0f then
            DrawCircleSectorLinesEx center radius startAngle endAngle segments thickness color
    )