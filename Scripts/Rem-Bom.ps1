Write-Host "Removing BOM from files..." -ForegroundColor Yellow

# https://stackoverflow.com/questions/6119956/how-to-determine-if-git-handles-a-file-as-binary-or-as-text#comment15281840_6134127
$nullHash = '4b825dc642cb6eb9a060e54bf8d69288fbee4904'
$textFiles = git -c core.quotepath=off diff --numstat $nullHash HEAD -- @allFiles |
    Where-Object { -not $_.StartsWith('-') } |
    ForEach-Object { [Regex]::Unescape($_.Split("`t", 3)[2]) }
Write-Output "Text files in the repository: $($textFiles.Length)"

[array] $excludeExtensions = @('.dotsettings')

try
{
    foreach ($file in $textFiles)
    {
        if ($excludeExtensions -contains [IO.Path]::GetExtension($file).ToLowerInvariant())
        {
            continue
        }

        $fullPath = Resolve-Path -LiteralPath $file
        $content = [System.IO.File]::ReadAllText($fullPath, [System.Text.Encoding]::UTF8)
        [System.IO.File]::WriteAllText($fullPath, $content, [System.Text.UTF8Encoding]::new($false))
    }
} finally {
    Pop-Location
}
