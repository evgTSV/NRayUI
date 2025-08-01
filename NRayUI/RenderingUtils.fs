﻿module NRayUI.RenderingUtils

open System
open System.Numerics
open NRayUI.RlGlUtils
open NRayUI.Field
open NRayUI.Positioning
open NRayUI.Utils
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
    
let DrawCircleSectorLinesEx
    (center: Vector2)
    (radius: float32)
    (startAngle: float32)
    (endAngle: float32)
    (segments: int)
    (thickness: float32)
    (color: Color)
    : unit =
    let segments =
        if segments < 0 then 4
        elif segments < 4 then
            let th = MathF.Acos(2f * (MathF.Pow(1f - Constants.SmoothCircleErrorRate / radius, 2f) - 1f))
            MathF.Ceiling((endAngle - startAngle) / th) |> int
        else segments
        
    let innerRadius = radius
    let outerRadius = radius + thickness
    
    let step = (endAngle - startAngle) / float32 segments
    
    let drawTriangles =
        (fun () ->
            for i in 0..(segments - 1) do
                let angle1 = startAngle + float32 i * step
                let angle2 = startAngle + float32 (i + 1) * step

                RlGl.Vertex2F(center.X + cos(deg2rad angle1) * outerRadius, center.Y + sin(deg2rad angle1) * outerRadius)
                RlGl.Vertex2F(center.X + cos(deg2rad angle1) * innerRadius, center.Y + sin(deg2rad angle1) * innerRadius)
                RlGl.Vertex2F(center.X + cos(deg2rad angle2) * outerRadius, center.Y + sin(deg2rad angle2) * outerRadius)

                RlGl.Vertex2F(center.X + cos(deg2rad angle1) * innerRadius, center.Y + sin(deg2rad angle1) * innerRadius)
                RlGl.Vertex2F(center.X + cos(deg2rad angle2) * innerRadius, center.Y + sin(deg2rad angle2) * innerRadius)
                RlGl.Vertex2F(center.X + cos(deg2rad angle2) * outerRadius, center.Y + sin(deg2rad angle2) * outerRadius)
        )
        |> withRlGlColor4ub color
        |> withRlGlMode DrawMode.Triangles
        
    let drawLines =
        (fun () ->
        for i in 0..(segments - 1) do
            let angleOld, angleNew =
                startAngle + float32 i * step,
                startAngle + float32 (i + 1) * step
                
            RlGl.Vertex2F(center.X + cos(deg2rad angleOld) * outerRadius, center.Y + sin(deg2rad angleOld) * outerRadius)
            RlGl.Vertex2F(center.X + cos(deg2rad angleNew) * outerRadius, center.Y + sin(deg2rad angleNew) * outerRadius)
        )
        |> withRlGlColor4ub color
        |> withRlGlMode DrawMode.Lines
        
    if thickness > 1f then
        drawTriangles()
    else
        drawLines()

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
    
    let getRadius (corner: float32) =
        Math.Clamp(corner, 0f, 1f) * MathF.Min(width, height) / 2f
    
    let radii = 
        { TopLeft = roundness.TopLeft |> getRadius
          TopRight = roundness.TopRight |> getRadius
          BottomRight = roundness.BottomRight |> getRadius
          BottomLeft = roundness.BottomLeft |> getRadius }
    
    let getOffset (radius: float32) =
        if radius > 0f then -thickness / 2f else thickness / 2f
    
    let offsets =
        { TopLeft = radii.TopLeft |> getOffset
          TopRight = radii.TopRight |> getOffset
          BottomRight = radii.BottomRight |> getOffset
          BottomLeft = radii.BottomLeft |> getOffset }
    
    let topLine =
        (Vector2(point.X + radii.TopLeft - offsets.TopLeft, point.Y),
         Vector2(point.X + width - radii.TopRight + offsets.TopRight, point.Y))
    
    let bottomLine =
        (Vector2(point.X + radii.BottomLeft - offsets.BottomLeft, point.Y + height),
        Vector2(point.X + width - radii.BottomRight + offsets.BottomRight, point.Y + height))
    
    let leftLine =
        (Vector2(point.X, point.Y + radii.TopLeft - offsets.TopLeft),
         Vector2(point.X, point.Y + height - radii.BottomLeft + offsets.BottomLeft))
    
    let rightLine =
        (Vector2(point.X + width, point.Y + radii.TopRight - offsets.TopRight),
         Vector2(point.X + width, point.Y + height - radii.BottomRight + offsets.BottomRight))
    
    [| topLine; bottomLine; leftLine; rightLine |]
    |> Array.iter (fun (start, end_) ->
        DrawLineEx(start, end_, thickness, color)
    )
    
    let d = thickness / 2f
    [|
        (Vector2(point.X + radii.TopLeft + d, point.Y + radii.TopLeft + d), radii.TopLeft, 180.0f, 270.0f)
        (Vector2(point.X + width - radii.TopRight - d, point.Y + radii.TopRight + d), radii.TopRight, 270.0f, 360.0f)
        (Vector2(point.X + width - radii.BottomRight - d, point.Y + height - radii.BottomRight - d), radii.BottomRight, 0.0f, 90.0f)
        (Vector2(point.X + radii.BottomLeft + d, point.Y + height - radii.BottomLeft - d), radii.BottomLeft, 90.0f, 180.0f)
    |] 
    |> Array.iter (fun (center, radius, startAngle, endAngle) ->
        if radius > 0.0f then
            DrawCircleSectorLinesEx center radius startAngle endAngle segments thickness color
    )
    
let DrawCircleSectorEx
    (center: Vector2)
    (radius: float32)
    (startAngle: float32)
    (endAngle: float32)
    (segments: int)
    (color: Color)
    : unit =
    let segments =
        if segments < 0 then 4
        elif segments < 4 then
            let th = MathF.Acos(2f * (MathF.Pow(1f - Constants.SmoothCircleErrorRate / radius, 2f) - 1f))
            MathF.Ceiling((endAngle - startAngle) / th) |> int
        else segments
    
    let step = (endAngle - startAngle) / float32 segments
    
    let drawTriangles =
        (fun () ->
            for i in 0..(segments - 1) do
                let angle1 = startAngle + float32 i * step
                let angle2 = startAngle + float32 (i + 1) * step

                RlGl.Vertex2F(center.X, center.Y)
                RlGl.Vertex2F(center.X + cos(deg2rad angle2) * radius, center.Y + sin(deg2rad angle2) * radius)     
                RlGl.Vertex2F(center.X + cos(deg2rad angle1) * radius, center.Y + sin(deg2rad angle1) * radius)
        )
        |> withRlGlColor4ub color
        |> withRlGlMode DrawMode.Triangles
    
    drawTriangles()

let DrawRectangleCustomRounded
    (rec_: Transformations.Rectangle) 
    (roundness: Corners) 
    (segments: int)
    (color: Color) 
    : unit =
        
    let point = Vector2(rec_.X, rec_.Y)
    let width = rec_.Width
    let height = rec_.Height
        
    let getRadius (corner: float32) =
        Math.Clamp(corner, 0f, 1f) * MathF.Min(width, height) / 2f
    
    let radii = 
        { TopLeft = roundness.TopLeft |> getRadius
          TopRight = roundness.TopRight |> getRadius
          BottomRight = roundness.BottomRight |> getRadius
          BottomLeft = roundness.BottomLeft |> getRadius }
    (*
    Quick sketch to make sense all of this,
    there are 9 parts to draw, also mark the 12 points we'll use

          P0____________________P1
          /|                    |\
         /1|          2         |3\
     P7 /__|____________________|__\ P2
       |   |P8                P9|   |
       | 8 |          9         | 4 |
       | __|____________________|__ |
     P6 \  |P11              P10|  / P3
         \7|          6         |5/
          \|____________________|/
          P5                    P4
    *)
        
    let points =
        [| Vector2(point.X + radii.TopLeft, point.Y) // P0 - top-left corner
           Vector2(point.X + width - radii.TopRight, point.Y) // P1 - top-right corner
           Vector2(point.X + width, point.Y + radii.TopRight) // P2 - right-top corner
           Vector2(point.X + width, point.Y + height - radii.BottomRight) // P3 - right-bottom corner
           Vector2(point.X + width - radii.BottomRight, point.Y + height) // P4 - bottom-right corner
           Vector2(point.X + radii.BottomLeft, point.Y + height) // P5 - bottom-left corner
           Vector2(point.X, point.Y + height - radii.BottomLeft) // P6 - left-bottom corner
           Vector2(point.X, point.Y + radii.TopLeft) // P7 - left-top corner
           Vector2(point.X + radii.TopLeft, point.Y + radii.TopLeft) // P8 - inner top-left (rectangle start)
           Vector2(point.X + width - radii.TopRight, point.Y + radii.TopRight) // P9 - inner top-right (fixed y-coordinate)
           Vector2(point.X + width - radii.BottomRight, point.Y + height - radii.BottomRight) // P10 - inner bottom-right
           Vector2(point.X + radii.BottomLeft, point.Y + height - radii.BottomLeft) |] // P11 - inner bottom-left
        
    let drawRect =
        (fun () ->
            // [2] Top rectangle
            RlGl.Vertex2F(points[0].X, points[0].Y)
            RlGl.Vertex2F(points[8].X, points[8].Y)
            RlGl.Vertex2F(points[9].X, points[9].Y)
            RlGl.Vertex2F(points[1].X, points[1].Y)
            RlGl.Vertex2F(points[0].X, points[0].Y)
            RlGl.Vertex2F(points[9].X, points[9].Y)
            
            // [4] Right rectangle
            RlGl.Vertex2F(points[9].X, points[9].Y)
            RlGl.Vertex2F(points[10].X, points[10].Y)
            RlGl.Vertex2F(points[3].X, points[3].Y)
            RlGl.Vertex2F(points[2].X, points[2].Y)
            RlGl.Vertex2F(points[9].X, points[9].Y)
            RlGl.Vertex2F(points[3].X, points[3].Y)
            
            // [6] Bottom rectangle
            RlGl.Vertex2F(points[11].X, points[11].Y)
            RlGl.Vertex2F(points[5].X, points[5].Y)
            RlGl.Vertex2F(points[4].X, points[4].Y)
            RlGl.Vertex2F(points[10].X, points[10].Y)
            RlGl.Vertex2F(points[11].X, points[11].Y)
            RlGl.Vertex2F(points[4].X, points[4].Y)
            
            // [8] Left rectangle
            RlGl.Vertex2F(points[7].X, points[7].Y)
            RlGl.Vertex2F(points[6].X, points[6].Y)
            RlGl.Vertex2F(points[11].X, points[11].Y)
            RlGl.Vertex2F(points[8].X, points[8].Y)
            RlGl.Vertex2F(points[7].X, points[7].Y)
            RlGl.Vertex2F(points[11].X, points[11].Y)
            
            // [9] Middle rectangle
            RlGl.Vertex2F(points[8].X, points[8].Y)
            RlGl.Vertex2F(points[11].X, points[11].Y)
            RlGl.Vertex2F(points[10].X, points[10].Y)
            RlGl.Vertex2F(points[9].X, points[9].Y)
            RlGl.Vertex2F(points[8].X, points[8].Y)
            RlGl.Vertex2F(points[10].X, points[10].Y)
        )
        |> withRlGlColor4ub color
        |> withRlGlMode DrawMode.Triangles
        
    drawRect()
    
    [|
        (points[8], radii.TopLeft, 180.0f, 270.0f)
        (points[9], radii.TopRight, 270.0f, 360.0f)
        (points[10], radii.BottomRight, 0.0f, 90.0f)
        (points[11], radii.BottomLeft, 90.0f, 180.0f)
    |] 
    |> Array.iter (fun (center, radius, startAngle, endAngle) ->
        if radius > 0.0f then
            DrawCircleSectorEx center radius startAngle endAngle segments color
    )
    