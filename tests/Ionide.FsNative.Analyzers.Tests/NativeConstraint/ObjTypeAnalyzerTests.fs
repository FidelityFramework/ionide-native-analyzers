module Ionide.FsNative.Analyzers.Tests.NativeConstraint.ObjTypeAnalyzerTests

open NUnit.Framework
open FSharp.Compiler.CodeAnalysis
open FSharp.Analyzers.SDK
open FSharp.Analyzers.SDK.Testing
open Ionide.FsNative.Analyzers.NativeConstraint.ObjTypeAnalyzer

let mutable projectOptions: FSharpProjectOptions = FSharpProjectOptions.zero

[<SetUp>]
let Setup () =
    task {
        let! opts = mkOptionsFromProject "net8.0" []
        projectOptions <- opts
    }

[<Test>]
let ``box operation triggers error`` () =
    async {
        let source =
            """module Lib

let x = box 42
    """

        let ctx = getContext projectOptions source
        let! msgs = objTypeCliAnalyzer ctx
        Assert.That(msgs, Is.Not.Empty)
        let msg = msgs[0]
        Assert.That(msg.Code, Is.EqualTo "FSNATIVE-002")
        Assert.That(msg.Severity, Is.EqualTo Severity.Error)
    }

[<Test>]
let ``unbox operation triggers error`` () =
    async {
        let source =
            """module Lib

let x: obj = box 42
let y: int = unbox x
    """

        let ctx = getContext projectOptions source
        let! msgs = objTypeCliAnalyzer ctx
        // Should have errors for both box and unbox
        Assert.That(msgs.Length, Is.GreaterThanOrEqualTo 2)
        Assert.That(msgs |> List.exists (fun m -> m.Code = "FSNATIVE-003"), Is.True)
    }

[<Test>]
let ``generic type does not trigger error`` () =
    async {
        let source =
            """module Lib

let identity<'T> (x: 'T) = x
let y = identity 42
    """

        let ctx = getContext projectOptions source
        let! msgs = objTypeCliAnalyzer ctx
        Assert.That(msgs, Is.Empty)
    }

[<Test>]
let ``int type does not trigger error`` () =
    async {
        let source =
            """module Lib

let x: int = 42
let y: float = 3.14
let z: string = "hello"
    """

        let ctx = getContext projectOptions source
        let! msgs = objTypeCliAnalyzer ctx
        Assert.That(msgs, Is.Empty)
    }