module Ionide.FsNative.Analyzers.Tests.NativeConstraint.NullLiteralAnalyzerTests

open NUnit.Framework
open FSharp.Compiler.CodeAnalysis
open FSharp.Analyzers.SDK
open FSharp.Analyzers.SDK.Testing
open Ionide.FsNative.Analyzers.NativeConstraint.NullLiteralAnalyzer

let mutable projectOptions: FSharpProjectOptions = FSharpProjectOptions.zero

[<SetUp>]
let Setup () =
    task {
        let! opts = mkOptionsFromProject "net8.0" []
        projectOptions <- opts
    }

[<Test>]
let ``null literal in let binding triggers error`` () =
    async {
        let source =
            """module Lib

let x = null
    """

        let ctx = getContext projectOptions source
        let! msgs = nullLiteralCliAnalyzer ctx
        Assert.That(msgs, Is.Not.Empty)
        let msg = msgs[0]
        Assert.That(msg.Code, Is.EqualTo "FSNATIVE-001")
        Assert.That(msg.Severity, Is.EqualTo Severity.Error)
    }

[<Test>]
let ``null in comparison triggers error`` () =
    async {
        let source =
            """module Lib

let f (x: string) = x = null
    """

        let ctx = getContext projectOptions source
        let! msgs = nullLiteralCliAnalyzer ctx
        Assert.That(msgs, Is.Not.Empty)
        Assert.That(msgs[0].Code, Is.EqualTo "FSNATIVE-001")
    }

[<Test>]
let ``null in pattern match triggers error`` () =
    async {
        let source =
            """module Lib

let f x =
    match x with
    | null -> "null"
    | _ -> "not null"
    """

        let ctx = getContext projectOptions source
        let! msgs = nullLiteralCliAnalyzer ctx
        Assert.That(msgs, Is.Not.Empty)
        Assert.That(msgs[0].Code, Is.EqualTo "FSNATIVE-001")
    }

[<Test>]
let ``null as function argument triggers error`` () =
    async {
        let source =
            """module Lib

let f (x: string) = x
let y: string = f null
    """

        let ctx = getContext projectOptions source
        let! msgs = nullLiteralCliAnalyzer ctx
        Assert.That(msgs, Is.Not.Empty)
        Assert.That(msgs[0].Code, Is.EqualTo "FSNATIVE-001")
    }

[<Test>]
let ``None does not trigger error`` () =
    async {
        let source =
            """module Lib

let x: int option = None
let y = Option.defaultValue 0 x
    """

        let ctx = getContext projectOptions source
        let! msgs = nullLiteralCliAnalyzer ctx
        Assert.That(msgs, Is.Empty)
    }

[<Test>]
let ``ValueNone does not trigger error`` () =
    async {
        let source =
            """module Lib

let x: int voption = ValueNone
    """

        let ctx = getContext projectOptions source
        let! msgs = nullLiteralCliAnalyzer ctx
        Assert.That(msgs, Is.Empty)
    }
