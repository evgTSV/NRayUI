module ScannerTests

open System
open NRayUI.Positioning
open NRayUI.Scanner
open Xunit

module SidesScannerTests =
    
    [<Fact>]
    let ``scanSidesInput with empty string returns zeroSides`` () =
        let result = scanSidesInput ""
        Assert.Equal(zeroSides, result)
    
    [<Fact>]
    let ``scanSidesInput with single value creates uniform sides`` () =
        let result = scanSidesInput "10"
        let expected = { 
            Top = 10f; Right = 10f; Bottom = 10f; Left = 10f 
        }
        Assert.Equal(expected, result)
    
    [<Fact>]
    let ``scanSidesInput with two values creates vertical and horizontal sides`` () =
        let result = scanSidesInput "10 20"
        let expected = { 
            Top = 10f; Right = 20f; Bottom = 10f; Left = 20f 
        }
        Assert.Equal(expected, result)
    
    [<Fact>]
    let ``scanSidesInput with three values creates CSS-like sides`` () =
        let result = scanSidesInput "10;20;30"
        let expected = { 
            Top = 10f; Right = 30f; Bottom = 20f; Left = 30f 
        }
        Assert.Equal(expected, result)
    
    [<Fact>]
    let ``scanSidesInput with four values creates all sides`` () =
        let result = scanSidesInput "10 20 30 40"
        let expected = { 
            Top = 10f; Right = 20f; Bottom = 30f; Left = 40f 
        }
        Assert.Equal(expected, result)
    
    [<Fact>]
    let ``scanSidesInput with multiple separators works correctly`` () =
        let result = scanSidesInput "10;20 30;40"
        let expected = { 
            Top = 10f; Right = 20f; Bottom = 30f; Left = 40f 
        }
        Assert.Equal(expected, result)
    
    [<Fact>]
    let ``scanSidesInput with decimal values parses correctly`` () =
        let result = scanSidesInput "10.5;20.75;30.1"
        let expected = { 
            Top = 10.5f; Right = 30.1f; Bottom = 20.75f; Left = 30.1f 
        }
        Assert.Equal(expected, result)
    
    [<Fact>]
    let ``scanSidesInput with negative values parses correctly`` () =
        let result = scanSidesInput "-10;20;-5.5"
        let expected = { 
            Top = -10f; Right = -5.5f; Bottom = 20f; Left = -5.5f 
        }
        Assert.Equal(expected, result)
    
    [<Fact>]
    let ``scanSidesInput with extra spaces handles correctly`` () =
        let result = scanSidesInput "   10  ;  20  ;  30  ;  40   "
        let expected = { 
            Top = 10f; Right = 20f; Bottom = 30f; Left = 40f 
        }
        Assert.Equal(expected, result)
    
    [<Fact>]
    let ``scanSidesInput with more than 4 values throws exception`` () =
        Assert.Throws<Exception>(fun () -> 
            scanSidesInput "1 2 3 4 5" |> ignore)
    
    [<Fact>]
    let ``scanSidesInput with invalid float throws exception`` () =
        Assert.Throws<FormatException>(fun () -> 
            scanSidesInput "10;abc;30" |> ignore)
        
    [<Fact>]
    let ``scanSidesInput handles different decimal separators`` () =
        let originalCulture = System.Threading.Thread.CurrentThread.CurrentCulture
        try
            
            // In Russian culture uses comma as delimiter (e.g. 10,3)
            System.Threading.Thread.CurrentThread.CurrentCulture <- 
                System.Globalization.CultureInfo.GetCultureInfo("ru-RU")
            
            let result = scanSidesInput "10.5;20.75"
            Assert.Equal(10.5f, result.Top)
            Assert.Equal(20.75f, result.Right)
        finally
            System.Threading.Thread.CurrentThread.CurrentCulture <- originalCulture

module PositioningCornersTests =
    
    [<Fact>]
    let ``scanCornersInput with empty string returns zeroCorners`` () =
        let result = scanCornersInput ""
        Assert.Equal(zeroCorners, result)
    
    [<Fact>]
    let ``scanCornersInput with single value creates uniform corners`` () =
        let result = scanCornersInput "5.5"
        let expected = { 
            TopLeft = 5.5f; TopRight = 5.5f; BottomRight = 5.5f; BottomLeft = 5.5f 
        }
        Assert.Equal(expected, result)
    
    [<Fact>]
    let ``scanCornersInput with two values creates diagonal corners`` () =
        let result = scanCornersInput "10 20"
        let expected = { 
            TopLeft = 10f; TopRight = 20f; BottomRight = 10f; BottomLeft = 20f 
        }
        Assert.Equal(expected, result)
    
    [<Fact>]
    let ``scanCornersInput with three values creates mixed corners`` () =
        let result = scanCornersInput "10;20;30"
        let expected = { 
            TopLeft = 10f; TopRight = 30f; BottomRight = 20f; BottomLeft = 30f 
        }
        Assert.Equal(expected, result)
    
    [<Fact>]
    let ``scanCornersInput with four values creates all corners`` () =
        let result = scanCornersInput "1 2 3 4"
        let expected = { 
            TopLeft = 1f; TopRight = 2f; BottomRight = 3f; BottomLeft = 4f 
        }
        Assert.Equal(expected, result)
    
    [<Fact>]
    let ``scanCornersInput handles scientific notation`` () =
        let result = scanCornersInput "1e1;2.5e0;3.2e-1;4e2"
        let expected = { 
            TopLeft = 10f; TopRight = 2.5f; BottomRight = 0.32f; BottomLeft = 400f 
        }
        Assert.Equal(expected, result)
    
    [<Fact>]
    let ``scanCornersInput with mixed separators works correctly`` () =
        let result = scanCornersInput "1;2 3;4"
        let expected = { 
            TopLeft = 1f; TopRight = 2f; BottomRight = 3f; BottomLeft = 4f 
        }
        Assert.Equal(expected, result)
      
module EdgeCaseTests =
    
    [<Fact>]
    let ``scanSidesInput with maximum float32 values`` () =
        let result = scanSidesInput "3.4028235E+38"
        Assert.Equal(3.4028235E+38f, result.Top)
    
    [<Fact>]
    let ``scanSidesInput with minimum float32 values`` () =
        let result = scanSidesInput "-3.4028235E+38"
        Assert.Equal(-3.4028235E+38f, result.Top)
    
    [<Fact>]
    let ``scanSidesInput with very small values`` () =
        let result = scanSidesInput "0.0000001;0.0000002"
        Assert.True(abs(0.0000001f - result.Top) < 0.000001f)
    
    [<Fact>]
    let ``scanSidesInput handles culture invariant parsing`` () =
        let result = scanSidesInput "10.5;20.75"
        Assert.Equal(10.5f, result.Top)
        Assert.Equal(20.75f, result.Right)
        
module ParametrizedTests =
    
    [<Theory>]
    [<InlineData("", 0f, 0f, 0f, 0f)>]
    [<InlineData("5", 5f, 5f, 5f, 5f)>]
    [<InlineData("1 2", 1f, 2f, 1f, 2f)>]
    [<InlineData("1;2;3", 1f, 3f, 2f, 3f)>]
    [<InlineData("1 2 3 4", 1f, 2f, 3f, 4f)>]
    let ``scanSidesInput parametrized tests`` input top right bottom left =
        let result = scanSidesInput input
        Assert.Equal(top, result.Top)
        Assert.Equal(right, result.Right)
        Assert.Equal(bottom, result.Bottom)
        Assert.Equal(left, result.Left)
    
    [<Theory>]
    [<InlineData("", 0f, 0f, 0f, 0f)>]
    [<InlineData("2.5", 2.5f, 2.5f, 2.5f, 2.5f)>]
    [<InlineData("1 2", 1f, 2f, 1f, 2f)>]
    [<InlineData("1;2;3", 1f, 3f, 2f, 3f)>]
    [<InlineData("1 2 3 4", 1f, 2f, 3f, 4f)>]
    let ``scanCornersInput parametrized tests`` input topLeft topRight bottomRight bottomLeft =
        let result = scanCornersInput input
        Assert.Equal(topLeft, result.TopLeft)
        Assert.Equal(topRight, result.TopRight)
        Assert.Equal(bottomRight, result.BottomRight)
        Assert.Equal(bottomLeft, result.BottomLeft)