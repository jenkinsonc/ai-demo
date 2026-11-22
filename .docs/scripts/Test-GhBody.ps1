# Compares an issue body, PR body or issue comment stored on GitHub with the file it was posted from.
# Prints IDENTICAL (exit 0) or the first differing character (exit 1). Runs from anywhere in the repo.
param(
    [Parameter(Mandatory)][string]$File,
    [int]$Issue,
    [int]$Pr,
    [long]$Comment
)
$stored = (& "$PSScriptRoot\Get-GhBody.ps1" -Issue $Issue -Pr $Pr -Comment $Comment) -replace "`r`n", "`n"
$source = [IO.File]::ReadAllText($File, [Text.UTF8Encoding]::new($false)) -replace "`r`n", "`n"
if ($stored -ceq $source) { "IDENTICAL ($($source.Length) chars)"; exit 0 }
$n = [Math]::Min($stored.Length, $source.Length)
$i = 0
while ($i -lt $n -and $stored[$i] -ceq $source[$i]) { $i++ }
$line = ($source.Substring(0, [Math]::Min($i, $source.Length)) -split "`n").Count
$codepoint = { param($s) if ($i -lt $s.Length) { 'U+{0:X4}' -f [int]$s[$i] } else { 'end' } }
"DIFFERENT at char $i (line $line): source $(& $codepoint $source), stored $(& $codepoint $stored); lengths source $($source.Length), stored $($stored.Length)"
exit 1
