module Ionide.FsNative.Analyzers.NativeConstraint.ObjTypeAnalyzer

/// Detects obj type usage, boxing, and unboxing.
/// FsNative does not support obj - all types must be statically known.

open System
open FSharp.Analyzers.SDK
open FSharp.Analyzers.SDK.TASTCollecting
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text

[<Literal>]
let boxMessage = "box is not allowed in FsNative. Use explicit types or generics instead."

[<Literal>]
let unboxMessage = "unbox is not allowed in FsNative. Use explicit types or generics instead."

let private analyze (typedTree: FSharpImplementationFileContents option) : Message list =
    let messages = ResizeArray<Message>()

    let walker =
        { new TypedTreeCollectorBase() with
            override x.WalkCall _ (mfv: FSharpMemberOrFunctionOrValue) _ _ (args: FSharpExpr list) (m: range) =
                // Detect box usage
                if mfv.FullName = "Microsoft.FSharp.Core.Operators.box" then
                    messages.Add
                        {
                            Type = "BoxOperation"
                            Message = boxMessage
                            Code = "FSNATIVE-002"
                            Severity = Severity.Error
                            Range = m
                            Fixes = []
                        }
                // Detect unbox usage
                elif mfv.FullName = "Microsoft.FSharp.Core.Operators.unbox" then
                    messages.Add
                        {
                            Type = "UnboxOperation"
                            Message = unboxMessage
                            Code = "FSNATIVE-003"
                            Severity = Severity.Error
                            Range = m
                            Fixes = []
                        }
        }

    match typedTree with
    | None -> []
    | Some typedTree ->
        walkTast walker typedTree
        Seq.toList messages

[<Literal>]
let name = "ObjTypeAnalyzer"

[<Literal>]
let shortDescription = "FsNative does not support obj. All types must be statically known."

[<Literal>]
let helpUri = "https://ionide.io/ionide-fsnative-analyzers/native-constraint/002.html"

[<CliAnalyzer(name, shortDescription, helpUri)>]
let objTypeCliAnalyzer: Analyzer<CliContext> =
    fun (context: CliContext) ->
        async { return analyze context.TypedTree }

[<EditorAnalyzer(name, shortDescription, helpUri)>]
let objTypeEditorAnalyzer: Analyzer<EditorContext> =
    fun (context: EditorContext) ->
        async { return analyze context.TypedTree }