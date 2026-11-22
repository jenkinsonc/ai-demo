# Local overrides - `implement`

Apply these on top of the upstream `implement` skill. Where they conflict, these win.

## Review gate (replaces the upstream "Commit your work" step)

After `code-review` returns, present both reports and **end the turn**. The review is the hand-off: commit only when the user's next message directs it. Leave every finding as it stands, including ones you agree with - the user picks which to act on.