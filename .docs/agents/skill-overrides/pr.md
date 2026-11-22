# Local overrides - `pr`

Apply these on top of the upstream `pr` skill. Where they conflict, these win.

## Hard completion criterion

The body **must** end with one of these lines for every ticket the PR fully addresses:

```
Closes #<n>
```

(or `Fixes #<n>` / `Resolves #<n>`). If the PR is partial, use `Part of #<n>` instead. A PR body without one of these lines is not done - GitHub will not auto-close the ticket on merge.

Multiple tickets: one line per ticket, e.g.

```
Closes #16
Closes #25
```