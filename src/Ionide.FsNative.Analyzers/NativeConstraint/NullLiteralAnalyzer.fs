module Ionide.FsNative.Analyzers.NativeConstraint.NullLiteralAnalyzer

/// Detects ALL null literal usage.
/// FsNative does not support null - use Option/ValueOption instead.

open FSharp.Compiler.Text
open FSharp.Compiler.Syntax
open FSharp.Analyzers.SDK
open FSharp.Analyzers.SDK.ASTCollecting

[<Literal>]
let message = "null is not allowed in FsNative. Use Option or ValueOption for optional values."

let private analyze (parsedInput: ParsedInput) : Message list =
    let messages = ResizeArray<Message>()

    let collector =
        { new SyntaxCollectorBase() with
            override x.WalkExpr(_path, synExpr) =
                match synExpr with
                | SynExpr.Null m ->
                    messages.Add
                        {
                            Type = "NullLiteral"
                            Message = message
                            Code = "FSNATIVE-001"
                            Severity = Severity.Error
                            Range = m
                            Fixes = []
                        }
                | _ -> ()

            override x.WalkPat(_path, synPat) =
                match synPat with
                | SynPat.Null m ->
                    messages.Add
                        {
                            Type = "NullLiteral"
                            Message = message
                            Code = "FSNATIVE-001"
                            Severity = Severity.Error
                            Range = m
                            Fixes = []
                        }
                | _ -> ()
        }

    walkAst collector parsedInput
    Seq.toList messages

[<Literal>]
let name = "NullLiteralAnalyzer"

[<Literal>]
let shortDescription = "FsNative does not support null. Use Option or ValueOption instead."

[<Literal>]
let helpUri = "https://ionide.io/ionide-fsnative-analyzers/native-constraint/001.html"

[<CliAnalyzer(name, shortDescription, helpUri)>]
let nullLiteralCliAnalyzer: Analyzer<CliContext> =
    fun (context: CliContext) ->
        async { return analyze context.ParseFileResults.ParseTree }

[<EditorAnalyzer(name, shortDescription, helpUri)>]
let nullLiteralEditorAnalyzer: Analyzer<EditorContext> =
    fun (context: EditorContext) ->
        async { return analyze context.ParseFileResults.ParseTree }
