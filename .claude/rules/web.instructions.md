---
description: "Where npm runs and how to recover a failed test start. Use when running npm or npx, or working in the web front end."
paths: ["**/frontend/**"]
---

# Web

- **np\* (npm pnpm/yarn/npx) runs from `frontend` only** - it is the sole `package.json`. For web work, `cd E:\Repos\aidemo\frontend` in its own call once, and stay there: every other command runs from any folder (scripts and `aidemo.slnx` by full path, git paths with the `:/` root prefix, e.g. `git add :/.docs/design`). No `cd ...; npm ...` chains, no `--prefix`. `npm run typecheck`, `npm run lint` and `npm test` are allowlisted.
- `npm test` stops at startup with "Run the tests from an uppercase drive letter" when the working directory's drive letter is lowercase (a Vitest double-load bug, vitest-dev/vitest#10692): `cd E:\Repos\aidemo\frontend` in its own call and run it again.
