module Ionide.FsNative.Analyzers.Tests.NativeConstraint.BclUsageAnalyzerTests

open NUnit.Framework
open FSharp.Compiler.CodeAnalysis
open FSharp.Analyzers.SDK
open FSharp.Analyzers.SDK.Testing
open Ionide.FsNative.Analyzers.NativeConstraint.BclUsageAnalyzer

let mutable projectOptions: FSharpProjectOptions = FSharpProjectOptions.zero

[<SetUp>]
let Setup () =
    task {
        let! opts = mkOptionsFromProject "net8.0" []
        projectOptions <- opts
    }

[<Test>]
let ``open System triggers error`` () =
    async {
        let source =
            """module Lib

open System

let x = 42
    """

        let ctx = getContext projectOptions source
        let! msgs = bclUsageCliAnalyzer ctx
        Assert.That(msgs, Is.Not.Empty)
        Assert.That(msgs |> List.exists (fun m -> m.Code = "FSNATIVE-005"), Is.True)
    }

[<Test>]
let ``open System.Collections triggers error`` () =
    async {
        let source =
            """module Lib

open System.Collections.Generic

let x = 42
    """

        let ctx = getContext projectOptions source
        let! msgs = bclUsageCliAnalyzer ctx
        Assert.That(msgs, Is.Not.Empty)
        Assert.That(msgs |> List.exists (fun m -> m.Code = "FSNATIVE-005"), Is.True)
    }

[<Test>]
let ``System.DateTime type triggers error`` () =
    async {
        let source =
            """module Lib

let x: System.DateTime = System.DateTime.Now
    """

        let ctx = getContext projectOptions source
        let! msgs = bclUsageCliAnalyzer ctx
        Assert.That(msgs, Is.Not.Empty)
    }

[<Test>]
let ``System.Console call triggers error`` () =
    async {
        let source =
            """module Lib

let _ = System.Console.WriteLine("test")
    """

        let ctx = getContext projectOptions source
        let! msgs = bclUsageCliAnalyzer ctx
        Assert.That(msgs, Is.Not.Empty)
    }

[<Test>]
let ``open Microsoft.FSharp.Core does not trigger error`` () =
    async {
        let source =
            """module Lib

open Microsoft.FSharp.Core

let x: int option = None
    """

        let ctx = getContext projectOptions source
        let! msgs = bclUsageCliAnalyzer ctx
        // Should not have any FSNATIVE-005 errors for F# Core
        Assert.That(msgs |> List.exists (fun m -> m.Code = "FSNATIVE-005"), Is.False)
    }

[<Test>]
let ``F# list type does not trigger error`` () =
    async {
        let source =
            """module Lib

let x: int list = [1; 2; 3]
let y = List.map ((+) 1) x
    """

        let ctx = getContext projectOptions source
        let! msgs = bclUsageCliAnalyzer ctx
        // F# Core types should be allowed
        Assert.That(msgs |> List.filter (fun m -> m.Code.StartsWith("FSNATIVE")), Is.Empty)
    }

[<Test>]
let ``F# option type does not trigger error`` () =
    async {
        let source =
            """module Lib

let x: int option = Some 42
let y = Option.map ((+) 1) x
    """

        let ctx = getContext projectOptions source
        let! msgs = bclUsageCliAnalyzer ctx
        Assert.That(msgs |> List.filter (fun m -> m.Code.StartsWith("FSNATIVE")), Is.Empty)
    }

[<Test>]
let ``basic F# types do not trigger error`` () =
    async {
        let source =
            """module Lib

let a: int = 1
let b: float = 2.0
let c: string = "hello"
let d: bool = true
let e: unit = ()
    """

        let ctx = getContext projectOptions source
        let! msgs = bclUsageCliAnalyzer ctx
        Assert.That(msgs |> List.filter (fun m -> m.Code.StartsWith("FSNATIVE")), Is.Empty)
    }
