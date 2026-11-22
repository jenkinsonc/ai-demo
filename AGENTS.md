# Agent operating rules

Operating rules for every session. Coding rules the reviewer checks live in `CODING_STANDARDS.md`; per-skill local overrides live in `.docs/agents/skill-overrides`. Path-scoped working rules live in `.claude/rules/`: read `web.instructions.md` before working in `frontend`, and `dotnet.instructions.md` before writing C#.

## Done means green
Work is done when the repo is **green**: fresh, unfiltered, zero warnings and zero errors. A filtered or incremental run does not establish green: it says nothing about files outside the filter, and incremental reuse silently re-suppresses warnings on files that were not touched.

- **.NET**: `dotnet build E:\Repos\aidemo\aidemo.slnx --no-incremental /p:NuGetAudit=false` and an unfiltered `dotnet test E:\Repos\aidemo\aidemo.slnx --no-build`, both clean. Read the whole output of each.
- **Web**: `npm run lint`, `npm run typecheck` and `npm test`, each run from `frontend`.
- **IDE**: no IDE diagnostic left on any file the change touches (`getDiagnostics` in Claude Code, `get_errors` in Copilot). The build does not surface every IDE diagnostic.

## GitHub issues and pull requests
Before any `gh` read or write of an issue or PR - reading or claiming a ticket, writing a body or a comment - read `.docs/agents/issue-tracker.md`. Use the five default triage labels (`.docs/agents/triage-labels.md`).

## Working from a ticket
- **Read the full spec before writing code.** The issue body and its comments are the spec, and the comments are where decisions get recorded (which files, where they live, which approach was chosen).
- **Claim before you work.** Reading is free; the **claim** is the gate between reading and working. If the issue is already assigned, stop and ask: another session holds it. Otherwise claim it before the first change to the repo: a branch or a file edit.
- **Map every deliverable to a target file before writing anything.** The ticket names what to create and where it goes; that is the plan. Improvising a different file layout - or inventing files the ticket does not ask for - is the failure mode: the spec's file placement *is* the decision.
- If the ticket and a sensible default disagree, the ticket wins. Raise the disagreement with the user rather than silently following the default.

## Shell
- **Prefer the simplest working command.** If a flag does not work, remove it and re-evaluate the command rather than laying on more flags.
- **Chaining**: chain allowlisted commands with plain `;`. Give a command that prompts (`git commit`, `git push`, `gh pr create`, a scratchpad script) its own call, so the allowlisted work around it runs unprompted. Anything wrapped around or in front of an allowlisted command prompts: a `cd` (give it its own call), `if ($?) { … }`, `try { … }`, `$var = …`, `$env:X = …`. Write paths and IDs out in full (`repos/jenkinsonc/aidemo/...`, not `$s` or `{owner}/{repo}`).
- **Bash**: shell work goes in PowerShell. Wait on a background task's output with Monitor. Dev ports are in each project's `Properties/launchSettings.json`.
- **Trimming native output**: use `| Select-Object -Last N`, or a native flag (`git remote get-url origin`, `git log -n`). `-First` closes the pipe early, so the exe exits non-zero. Elsewhere, filter with the tool's own flag where one exists: `gh … --jq '… | select(test(\"x\"))'` (escape inner quotes for PowerShell 5.1), `git ls-files ':!dir'`.
- **Commit messages**: write the message to a scratchpad file with Write, then `git commit -F <file>` - the same shape as `gh pr create --body-file`.

## Responding to questions
When the user asks *why*, *what is this for*, or questions a change that has been made, respond with the **reasoning** and a **recommendation**. Do not edit code, remove code, file issues, or commit - until the user's next message directs an action. A question is not an instruction; a question the user intended as an instruction will say so.

## Citing locations
Every mention of a location - a file, heading, step, item, table row, line or symbol - is a relative-path markdown link to it, with a line anchor (`.docs/operations.md#L104`) or a heading anchor (`.docs/operations.md#sql-server-upgrades`). This holds in every message, plan, review finding and relayed sub-agent report; condensing a report keeps its links. Text not yet in a file has no location: label each block of it in chat with a number or letter, and refer to it by that label. Cite only a location a search hit confirms: a claim like "X is also in file Y" ships with the link that proves it.

Every proposed edit carries its **anchor**: a link to the target line, plus the current text -> proposed text. Linking the evidence for an edit does not anchor the edit itself.

## Skill overrides
**Skill bodies are frozen.** Never hand-edit any file under `.claude/skills/` or `.agents/skills/` - the one exception is pointer insertion, which goes through `/inject-skill-overrides`, never by hand. Every change to a skill goes in `.docs/agents/skill-overrides/<name>.md`, which each SKILL.md already points at. The bodies are third-party: `npx skills update` keeps them pristine and wipes any pointer you inject, so after any `npx skills update` or `npx skills add` run `/inject-skill-overrides` before doing anything else.

## Domain docs
Use the single-context layout. See `.docs/agents/domain.md`.
