# Screenshots a page of the running dev server (npm run dev) with headless Edge.
# Runs from the repo root or frontend. Chain several calls with ; to capture several pages.
#   .\.docs\scripts\Get-PageShot.ps1 -Page '/?face=A' -Out <scratchpad>\face-A.png      (repo root)
#   ..\.docs\scripts\Get-PageShot.ps1 -Page '/?face=A' -Out <scratchpad>\face-A.png     (frontend)
param(
    [Parameter(Mandatory)][string]$Page,
    [Parameter(Mandatory)][string]$Out,
    [int]$Width = 1600,
    [int]$Height = 900,
    [int]$WaitMs = 4000,
    [string]$BaseUrl = 'http://localhost:5173'
)
$edge = @(
    "${env:ProgramFiles(x86)}\Microsoft\Edge\Application\msedge.exe",
    "$env:ProgramFiles\Microsoft\Edge\Application\msedge.exe"
) | Where-Object { Test-Path $_ } | Select-Object -First 1
if (-not $edge) { throw 'Microsoft Edge not found.' }
$url = if ($Page -match '^https?://') { $Page } else { $BaseUrl.TrimEnd('/') + '/' + $Page.TrimStart('/') }
# Edge screenshots its own error page when the server is down, so check reachability first.
try { Invoke-WebRequest $url -UseBasicParsing -TimeoutSec 5 | Out-Null }
catch { throw "$url is not reachable. Is the dev server running (npm run dev)? $($_.Exception.Message)" }
# Resolve against PowerShell's location, not .NET's working directory, which does not follow cd.
$Out = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($Out)
if (Test-Path $Out) { Remove-Item $Out }
# Edge logs identity/GPU noise to stderr on every run; it is not a failure signal. The file is.
# Piping to Out-Null makes PowerShell wait for Edge to exit before the check below.
& $edge --headless=new --disable-gpu --hide-scrollbars "--window-size=$Width,$Height" "--virtual-time-budget=$WaitMs" "--screenshot=$Out" $url 2>$null | Out-Null
if (-not (Test-Path $Out)) { throw "Edge wrote no screenshot for $url." }
"SAVED $((Get-Item $Out).Length) bytes to $Out ($url)"
