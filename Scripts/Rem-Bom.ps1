Write-Host "Removing BOM from files..." -ForegroundColor Yellow

Get-ChildItem -Recurse -Include *.fs,*.fsx,.editorconfig,*.json,*.ps1,*.fsproj | ForEach-Object {
    $content = [System.IO.File]::ReadAllText($_.FullName, [System.Text.Encoding]::UTF8)
    [System.IO.File]::WriteAllText($_.FullName, $content, [System.Text.UTF8Encoding]::new($false))
}
