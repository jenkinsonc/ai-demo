# Fetches an issue body, PR body or issue comment exactly as stored on GitHub.
# With -File, saves it there as UTF-8 without a BOM, ready to edit and re-post; without -File, returns it.
# Runs from anywhere in the repo.
param(
    [string]$File,
    [int]$Issue,
    [int]$Pr,
    [long]$Comment
)
# gh prints UTF-8; read it as UTF-8, not the console code page.
[Console]::OutputEncoding = [Text.UTF8Encoding]::new($false)
# @json keeps the body on one line, so trailing newlines survive the pipe.
if ($Issue) { $json = gh issue view $Issue --json body --jq '.body | @json' }
elseif ($Pr) { $json = gh pr view $Pr --json body --jq '.body | @json' }
elseif ($Comment) { $json = gh api "repos/{owner}/{repo}/issues/comments/$Comment" --jq '.body | @json' }
else { throw 'Pass -Issue, -Pr or -Comment.' }
if ($LASTEXITCODE) { throw "gh exited with code $LASTEXITCODE." }
$body = $json | ConvertFrom-Json
if (-not $File) { return $body }
[IO.File]::WriteAllText($File, $body, [Text.UTF8Encoding]::new($false))
"SAVED $($body.Length) chars to $File"
