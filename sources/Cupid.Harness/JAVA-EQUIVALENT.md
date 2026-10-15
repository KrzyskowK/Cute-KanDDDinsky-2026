# Cupid.Harness in Java: is it doable?

**Short answer: yes.** Every Harness rule (HA1, HA5–HA12) can be implemented in Java.
There is no single Roslyn equivalent, but a combination of tools covers it.

> Based on the slides (`slides/markdown/4c.md`). The Harness rule sources were not
> present in `sources/Cupid.Harness` (only `bin/` and `obj/`), so the rule
> descriptions come from the slides, not from the code.

## Is there a Roslyn equivalent in Java?

Roslyn gives syntax trees, a semantic model and a compiler in one API. Java splits
this across several tools:

| Need | Java tool |
|---|---|
| Syntax tree plus resolved symbols | **Spoon** (INRIA). Closest to Roslyn's syntax + semantics. Standalone library, runs in CI. |
| Parsing with optional type resolution | **JavaParser** with its symbol solver. Lighter than Spoon, less exact. |
| Compiler-integrated analysis | **javac Tree API** and **Error Prone**. Error Prone is the nearest thing to Roslyn analyzers (plugs into the build). |
| Bytecode and dependency structure | **ArchUnit** |
| Full IDE-grade model | **Eclipse JDT** |
| Style and bug patterns | **Checkstyle**, **PMD**, **SpotBugs** (custom rules possible) |
| Pattern rules in YAML | **Semgrep** |

## Rule by rule

| Rule | Description | Java approach |
|---|---|---|
| HA1 | Idiomatic C# (FxCop-like) | Checkstyle, PMD or Error Prone |
| HA5 | No god class (max 6 public properties) | ArchUnit (count public fields/getters) or Spoon |
| HA6 | Single concern (public methods span at most one bounded-context concern) | Spoon or JavaParser. Heuristic, needs custom logic in any tool. |
| HA7 | Fan-out: an entity references no other entity type (reference by id) | ArchUnit (`fields().that()...`) |
| HA8 | No persistence in the domain (no EF/ORM attributes, no sync/hydrate shadows) | ArchUnit (`noClasses().should().dependOnClassesThat().resideInAPackage("jakarta.persistence..")`) |
| HA9 | Entities must implement `IDentifiable` | ArchUnit (`classes().that()...should().implement(...)`) |
| HA10 | Entities must not take services in their constructor | Spoon or JavaParser (inspect constructor parameter types) |
| HA11 + HA12 | Rich identifiers (primitive obsession) | Spoon, or Error Prone with a custom check |

## Recommendation

- **ArchUnit** for the structural and dependency rules (HA5, HA7, HA8, HA9). They read
  well as JUnit tests and fit in CI.
- **Spoon** for the rules that need the AST and resolved types (HA6, HA10, HA11, HA12).
- **Checkstyle / PMD / Error Prone** for the idiomatic-style rule (HA1).

## Main difference from Roslyn

Java has no one compiler API as convenient as Roslyn's semantic model. In practice the
work splits in two:

1. Structural and dependency rules on **bytecode** (ArchUnit).
2. Rules needing full **symbol resolution** (Error Prone or Spoon).

## Open questions

- Are the Roslyn rules standalone, or real analyzers?
- Is the target Java code, or a port of the C# rules to Java?
