module Ionide.FsNative.Analyzers.NativeConstraint.BclUsageAnalyzer

/// Detects BCL usage (open System, System.* types).
/// FsNative does not support BCL - use native types from Alloy instead.

open System
open FSharp.Compiler.Text
open FSharp.Compiler.Syntax
open FSharp.Analyzers.SDK
open FSharp.Analyzers.SDK.ASTCollecting
open FSharp.Analyzers.SDK.TASTCollecting
open FSharp.Compiler.Symbols

[<Literal>]
let openSystemMessage = "open System is not allowed in FsNative. Use native types from Alloy."

[<Literal>]
let bclTypeMessage = "BCL types (System.*) are not allowed in FsNative. Use native types from Alloy."

/// Namespaces that are allowed (F# Core that FsNative supports)
let allowedNamespaces = 
    set [
        "Microsoft.FSharp.Core"
        "Microsoft.FSharp.Collections"
        "Microsoft.FSharp.Control"
    ]

/// Check if a namespace is System.* but not an allowed exception
let private isBclNamespace (ns: string) =
    (ns.StartsWith("System", StringComparison.Ordinal) 
     || ns.StartsWith("Microsoft.Win32", StringComparison.Ordinal)
     || ns.StartsWith("Microsoft.VisualBasic", StringComparison.Ordinal))
    && not (allowedNamespaces.Contains ns)

/// Get full namespace from a LongIdent
let private longIdentToString (lid: LongIdent) =
    lid |> List.map (fun i -> i.idText) |> String.concat "."

let private analyzeUntyped (parsedInput: ParsedInput) : Message list =
    let messages = ResizeArray<Message>()

    let collector =
        { new SyntaxCollectorBase() with
            override x.WalkSynModuleDecl(_path, decl) =
                match decl with
                | SynModuleDecl.Open(target, m) ->
                    match target with
                    | SynOpenDeclTarget.ModuleOrNamespace(longId, _) ->
                        let ns = longIdentToString longId.LongIdent
                        if isBclNamespace ns then
                            messages.Add
                                {
                                    Type = "OpenSystem"
                                    Message = openSystemMessage
                                    Code = "FSNATIVE-005"
                                    Severity = Severity.Error
                                    Range = m
                                    Fixes = []
                                }
                    | SynOpenDeclTarget.Type(synType, _) ->
                        match synType with
                        | SynType.LongIdent(SynLongIdent(lid, _, _)) ->
                            let ns = longIdentToString lid
                            if isBclNamespace ns then
                                messages.Add
                                    {
                                        Type = "OpenSystem"
                                        Message = openSystemMessage
                                        Code = "FSNATIVE-005"
                                        Severity = Severity.Error
                                        Range = m
                                        Fixes = []
                                    }
                        | _ -> ()
                | _ -> ()

            override x.WalkType(_path, synType) =
                match synType with
                | SynType.LongIdent(SynLongIdent(lid, _, _)) ->
                    let typeName = longIdentToString lid
                    if isBclNamespace typeName then
                        messages.Add
                            {
                                Type = "BclType"
                                Message = bclTypeMessage
                                Code = "FSNATIVE-006"
                                Severity = Severity.Error
                                Range = synType.Range
                                Fixes = []
                            }
                | _ -> ()
        }

    walkAst collector parsedInput
    Seq.toList messages

let private analyzeTyped (typedTree: FSharpImplementationFileContents option) : Message list =
    let messages = ResizeArray<Message>()

    let walker =
        { new TypedTreeCollectorBase() with
            override x.WalkCall _ (mfv: FSharpMemberOrFunctionOrValue) _ _ _ (m: range) =
                // Check if calling a BCL method
                match mfv.DeclaringEntity with
                | Some entity ->
                    match entity.TryGetFullName() with
                    | Some fullName when isBclNamespace fullName ->
                        messages.Add
                            {
                                Type = "BclCall"
                                Message = $"BCL member '{mfv.DisplayName}' from '{fullName}' is not allowed in FsNative."
                                Code = "FSNATIVE-007"
                                Severity = Severity.Error
                                Range = m
                                Fixes = []
                            }
                    | _ -> ()
                | None -> ()
        }

    match typedTree with
    | None -> []
    | Some typedTree ->
        walkTast walker typedTree
        Seq.toList messages

[<Literal>]
let name = "BclUsageAnalyzer"

[<Literal>]
let shortDescription = "FsNative does not support BCL. Use native types from Alloy."

[<Literal>]
let helpUri = "https://ionide.io/ionide-fsnative-analyzers/native-constraint/005.html"

[<CliAnalyzer(name, shortDescription, helpUri)>]
let bclUsageCliAnalyzer: Analyzer<CliContext> =
    fun (context: CliContext) ->
        async { 
            let untypedMessages = analyzeUntyped context.ParseFileResults.ParseTree
            let typedMessages = analyzeTyped context.TypedTree
            return untypedMessages @ typedMessages
        }

[<EditorAnalyzer(name, shortDescription, helpUri)>]
let bclUsageEditorAnalyzer: Analyzer<EditorContext> =
    fun (context: EditorContext) ->
        async { 
            let untypedMessages = analyzeUntyped context.ParseFileResults.ParseTree
            let typedMessages = analyzeTyped context.TypedTree
            return untypedMessages @ typedMessages
        }