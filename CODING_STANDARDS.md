# Coding Standards
Coding standards for this codebase - judgement calls the reviewer enforces on top of what the compiler, the linters and the architecture tests (`aidemo.Tests/ArchitectureTests/`) already check. A rule that can be checked mechanically lives in one of those instead, with the rule in its failure message; this file only holds what no guardrail can substitute.

## Comments
- Comments describe the **current** architecture. A comment that references a pattern that no longer exists (e.g. "previously available via the Controller") is stale and must be deleted.

## Migrations
- **One migration per schema change**, named after the change (e.g. `AddWidgetInstances`).
- **Mutable until released, then locked.** A migration may be removed or regenerated freely until it ships in a published release (it is in a published `vX.Y.Z` tag). After that it is never changed; fixes go in a new migration.
- **Exception: a migration that never applied.** A released migration that never applied successfully to the deploy database (its release failed at `efbundle` and was rolled back) may be fixed or regenerated in the next release.
- **Never hand-edit generated migration files**, with two exceptions:
	- adding data-preserving SQL where EF would lose data (e.g. a rename EF scaffolds as drop + add);
	- creating a table EF doesn't own (e.g. NLog's `[NLog]`) with `migrationBuilder.Sql(...)` in `Up` and a matching drop in `Down`. Such a table has no entity class.

## Settings
- Settings use the Options pattern: a settings class bound with `AddOptions<T>().BindConfiguration(...)`, validated with `.ValidateDataAnnotations()` and `.ValidateOnStart()`, and consumed as `IOptions<T>`. Required values carry `[Required]` (or another data-annotation constraint), so a missing or invalid setting stops the app at startup rather than failing on first use.

## Nullability & string defaults
- Nullable reference types are off in every project (`Directory.Build.props`), so `null` is the "no value" state for reference types. Do **not** use `string.Empty` (or any sentinel) to mean "not set" - an empty string is a value, not the absence of one.
- Match property nullability to the column: a nullable property must map to a nullable DB column.

## Warning suppressions
- Suppressing a compiler, analyzer or linter diagnostic is a hard gate: each instance needs the user's explicit approval and its own `suppressions` issue. Before writing one, follow `.docs/agents/suppressions.md`; `SuppressionArchitectureTests` fails on any suppression without its issue reference.
