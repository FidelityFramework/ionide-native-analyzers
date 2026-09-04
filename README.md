# lattice-analyzers

A hard fork of [ionide-analyzers](https://github.com/ionide/ionide-analyzers), relabelled. The twelve
analyzers run over the F# Compiler Service typed tree and emit `IONIDE-0nn` messages linking to ionide.io.

**Disposition: retired.** Under the consumer contract (`~/repos/clef/docs/fidelity/phg/Lattice_Consumer_Contract.md`),
an analyzer is not a category: a rule over program semantics is either a display of facts the Program
Semantic Graph settles (a query the Lattice server answers) or new semantics (a Baker recipe or obligation in
the Clef Compiler Service). Of the twelve rules here, four are void under the native type universe (null
comparison, `System.String`, the CLR list, `--langversion`), two suggest residence the graph settles as
literal layout, and the rest are graph queries. The analyzer SDK contract, a typed tree handed to third-party
code, has no place under the transport rule.

Upstream license and attribution: see `LICENSE.md`. ionide-analyzers is the work of the Ionide community.
