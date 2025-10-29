module NRayUI.Loader

open System.Collections.Concurrent
open JetBrains.Lifetimes
open Raylib_CSharp.Audio
open Raylib_CSharp.Fonts
open Raylib_CSharp.Images
open Raylib_CSharp.Textures

type Loader<'a> =
    | Loader of load: (string -> 'a) * unload: ('a -> unit)
    
with
    member this.load (path: string) =
        let (Loader(load, _)) = this in load path
        
    member this.unload (x:  'a) =
        let (Loader(_, unload)) = this in unload x

let inline createLoader
    (loader: Loader<'a>)
    (lifetime: Lifetime)
    (path: string) =
        lifetime.Bracket(
            (fun () -> loader.load path),
            fun (x: 'a) -> loader.unload x)
        
let inline createTextureLoader lifetime=
    createLoader (Loader(Texture2D.Load, _.Unload())) lifetime
    
let inline createFontLoader lifetime=
    createLoader (Loader(Font.Load, _.Unload())) lifetime
    
let inline createImageLoader lifetime =
    createLoader (Loader(Image.Load, _.Unload())) lifetime
    
let inline createAudioLoader lifetime=
    createLoader (Loader(Sound.Load, _.Unload())) lifetime
    
type Resources(lifetime: Lifetime) =
    let textureLoaded = ConcurrentDictionary<string, Texture2D>()
    let fontLoaded = ConcurrentDictionary<string, Font>()
    let imageLoaded = ConcurrentDictionary<string, Image>()
    let audioLoaded = ConcurrentDictionary<string, Sound>()
        
    member this.LoadTexture(path: string) =
        textureLoaded.GetOrAdd(path, createTextureLoader lifetime)
        
    member this.LoadFont(path: string) =
        fontLoaded.GetOrAdd(path, createFontLoader lifetime)
        
    member this.LoadImage(path: string) =
        imageLoaded.GetOrAdd(path, createImageLoader lifetime)
    
    member this.LoadAudio(path: string) =
        audioLoaded.GetOrAdd(path, createAudioLoader lifetime)
        
    member this.LoadTextureFromImage(image: Image) =
        let texture = Texture2D.LoadFromImage image
        createLoader (Loader((fun _ -> texture), _.Unload())) lifetime ""