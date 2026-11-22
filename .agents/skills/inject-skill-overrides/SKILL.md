---
name: inject-skill-overrides
description: "Re-link local overrides into third-party skills after `npx skills update`."
disable-model-invocation: true
---

`npx skills update` overwrites the SKILL.md of every third-party skill, wiping any locally added text. The override **content** is safe: it lives in `.docs/agents/skill-overrides/<name>.md`, which the update never touches. The **pointer** - the one block at the top of each SKILL.md that makes the skill read its overrides - is what the update destroys, and what this skill restores.

## Process

### 1. Resolve the override set

List `.docs/agents/skill-overrides/`. Each file `<name>.md` names a skill whose SKILL.md must carry a pointer. If the folder is missing or empty, report that and stop.

### 2. Inject the pointer, per skill

For each `<name>`:

- **Target**: `.claude/skills/<name>/SKILL.md`. If the file is absent, skip the skill and note it.
- **Check first**: If the pointer block from the next step is already present, skip the skill.
- **Insert** the block as the first line of the body - immediately after the closing `---` of the frontmatter - followed by a blank line. Leave the frontmatter and everything after the block untouched.

The pointer block is fixed text; copy it verbatim, substituting `<name>`:

```markdown
> **Local overrides.** Before anything else, read `.docs/agents/skill-overrides/<name>.md` and apply it to everything in this skill. Where they conflict, the overrides win.
```

### 3. Verify and report

Re-check every skill in the override set: the pointer present, or a skip reason (target absent / already present). Report the outcome per skill in one line each.

## Completion criterion

Every skill in the override set has a verbatim pointer as the first line of its body, or a recorded skip reason. No SKILL.md is left silently without its pointer.