# Local overrides - `code-review`

Apply these on top of the upstream `code-review` skill. Where they conflict, these win.

## Standards sub-agent prompt - extra bullet

Add this bullet to the Standards sub-agent prompt (alongside the existing diff/standards/brief bullets):

- A **diagnostic gate** that is mechanical and not a judgement call: block approval until the repo is green as `AGENTS.md` §Done means green defines it - it applies verbatim, exactly as stated. This gate is independent of the Standards axis and applies unconditionally.
- **Dependency changes**: list every package the diff adds, removes or re-versions (`PackageReference` in a `*.csproj`, `dependencies`/`devDependencies` in `package.json`), each with the code that needs it. This is a listing for the user, not a gate.

## Both sub-agent prompts - tools

Add these bullets to both sub-agent prompts:

- Read and search files with Read, Grep and Glob; run git, gh and docker in the PowerShell tool.
- Read issues in the shape `.docs/agents/issue-tracker.md` §Read and claim an issue gives.

## Spec sub-agent prompt - modified brief

Replace the Spec sub-agent brief with:

> "Report: (a) requirements the spec asked for that are missing or partial; (b) behaviour in the diff that wasn't asked for (scope creep); (c) requirements that look implemented but where the implementation looks wrong. Quote the spec line for each finding. **A finding is a judgement call, not a to-do.** If the spec's literal reading conflicts with how a sibling feature in the same product was implemented, report it as a *conflict with prior art* - name the precedent - and stop. Under 400 words."

## Prior-art conflicts (new section)

If the **Spec** axis reports a *conflict with prior art* (a sibling feature in the same product was implemented differently than the spec's literal reading), **stop** and present the conflict to the user - do not resolve it, do not implement it, do not file an issue, do not remove anything. Group conflicts by the prior art they conflict with: conflicts with two distinct prior arts are presented as two independent choices; conflicts that share one prior art are presented together with a single choice covering them all.

For each group, present the three options:

- **(a) Accept the conflict** - implement the spec's literal reading in the current diff only. The new behavior is a one-off; the prior art stays as-is.
- **(b) Reject the conflict** - remain consistent with the prior art. Discard the spec's literal reading for this diff; the prior art is the implementation.
- **(c) Accept the conflict and propagate** - implement the spec's literal reading in the current diff **and** file a follow-up issue to apply the new implementation to every instance of the prior art. The follow-up issue is the propagation vehicle.

Guiding default: consistency with the existing codebase. When presenting a close call, recommend (b) and say so. If there is no prior art to conflict with, option (c) is not available - the choice is (a) or (b). Wait for the user's choice before any further action.

## After aggregating (new section)

The aggregated report ends the turn. Findings go to the user as they stand; the user picks which to act on.