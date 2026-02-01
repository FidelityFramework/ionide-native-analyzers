# Lattice.Analyzers

[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Lattice.Analyzers?style=flat-square)](https://www.nuget.org/packages/Lattice.Analyzers/absoluteLatest)

F# analyzers for native and polyglot systems programming with [FSharp.Analyzers.SDK](https://ionide.io/FSharp.Analyzers.SDK/).

## Heritage

This project is a hard fork of [Ionide Analyzers](https://github.com/ionide/ionide-analyzers), part of the excellent [Ionide](https://ionide.io/) F# IDE tooling ecosystem created by Krzysztof Cieślak.

Lattice extends Ionide's foundation to support polyglot systems programming with F# Native, MLIR, LLVM, F*, Lua, and C. The name "Lattice" represents the chemical progression from individual ions to organized crystal lattices—honoring the foundation while extending to polyglot systems.

See [IONIDE_HERITAGE.md](IONIDE_HERITAGE.md) for complete attribution details. 

## Running these analyzers

These analyzers are designed for F# Native projects (.fidproj) and will be integrated into the Firefly compiler toolchain and Lattice IDE extensions.

For standalone usage (when available), you will need:
- The `fsnative-analyzers` CLI tool (planned)
- This [NuGet package](https://www.nuget.org/packages/Lattice.Analyzers/absoluteLatest)

Integration is based on the [FSharp.Analyzers.SDK](https://ionide.io/FSharp.Analyzers.SDK/) framework.

## Getting started contributing

See our documentation for contribution guidelines.

## Running the documentation

Run `dotnet fsi build.fsx -p Docs` to run the documentation locally.

## Acknowledgments

We are deeply grateful to:
- **Krzysztof Cieślak** for creating Ionide and the original Ionide Analyzers
- The entire Ionide community for demonstrating what great F# tooling can be
- The FSharp.Analyzers.SDK team for the excellent analyzer framework