# Fidelity.Analyzers

**F# Analyzers for Native F# Development**

[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Fidelity.Analyzers?style=flat-square)](https://www.nuget.org/packages/Fidelity.Analyzers/absoluteLatest)

A fork of [Ionide.Analyzers](https://github.com/ionide/ionide-analyzers) enhanced for native F# development with the Fidelity framework.

## Overview

Fidelity.Analyzers provides F# analyzers tailored for native compilation scenarios. It includes all standard Ionide analyzers plus additional checks relevant to native F# development.

## Running these analyzers

To run these analyzers you need:
1. The [fsharp-analyzers tool](https://www.nuget.org/packages/fsharp-analyzers)
2. This [NuGet package](https://www.nuget.org/packages/Fidelity.Analyzers/absoluteLatest)

Learn more on getting started in the [SDK documentation](https://ionide.io/FSharp.Analyzers.SDK/content/Getting%20Started%20Using.html).

## The Fidelity Ecosystem

| Project | Role |
|---------|------|
| [Firefly](https://github.com/FidelityFramework/firefly) | AOT compiler |
| [FSNAC](https://github.com/FidelityFramework/FsNativeAutoComplete) | Language server |
| [Fidelity-VSCode](https://github.com/FidelityFramework/fidelity-vscode-fsharp) | VS Code extension |
| [Fidelity-Vim](https://github.com/FidelityFramework/fidelity-vim-fsharp) | Vim/Neovim plugin |
| **Fidelity.Analyzers** | This package |
| [Alloy](https://github.com/FidelityFramework/alloy) | Native standard library |

## Contributing

Get started contributing using our [docs](./docs/content/contributions.md)!

## Building

Run `dotnet fsi build.fsx -p Docs` to run the documentation locally.

## Acknowledgments

This project is a fork of [Ionide.Analyzers](https://github.com/ionide/ionide-analyzers). We're grateful to the Ionide maintainers for creating the foundation.

## License

MIT License - see [LICENSE.md](LICENSE.md)
