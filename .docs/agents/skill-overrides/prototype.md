# Local overrides - `prototype`

Apply these on top of the upstream `prototype` skill. Where they conflict, these win.

## Visual claims require visual evidence

A visual claim ("the button renders", "the layout is correct") is only made after the screenshot or the DOM/state read-back actually shows the claimed element. "It should be there" or "it is in the DOM" is not evidence of visibility.

Capture screenshots with [`.docs/scripts/Get-PageShot.ps1`](../../scripts/Get-PageShot.ps1) against the running `npm run dev` server, then Read the PNG. It is allowlisted, so it runs unprompted; chain several calls with `;` for several pages.