# Unity Scripts Kombinator
# Kombiniert alle C# Scripts in eine einzige Text-Datei für einfaches Teilen

# Pfad zu deinem Unity-Projekt
$projectPath = "D:\Unity\HexGrid CleanStart"

# Ausgabe-Datei
$outputFile = "D:\HexGrid-AllScripts.txt"

# Suche nach allen .cs Dateien im Assets Ordner
$assetsPath = Join-Path $projectPath "Assets"

if (-not (Test-Path $assetsPath)) {
    Write-Host "FEHLER: Assets Ordner nicht gefunden in $projectPath" -ForegroundColor Red
    Read-Host "Drücke Enter zum Beenden"
    exit
}

Write-Host "Suche nach C# Scripts in: $assetsPath" -ForegroundColor Green

# Finde alle .cs Dateien
$csFiles = Get-ChildItem -Path $assetsPath -Filter "*.cs" -Recurse

if ($csFiles.Count -eq 0) {
    Write-Host "WARNUNG: Keine .cs Dateien gefunden!" -ForegroundColor Yellow
    Read-Host "Drücke Enter zum Beenden"
    exit
}

Write-Host "Gefunden: $($csFiles.Count) C# Script(s)" -ForegroundColor Cyan

# Erstelle die kombinierte Datei
$content = @()
$content += "=" * 80
$content += "HEXGRID-CLEANSTART PROJECT - ALLE SCRIPTS"
$content += "Generiert am: $(Get-Date -Format 'dd.MM.yyyy HH:mm:ss')"
$content += "Anzahl Scripts: $($csFiles.Count)"
$content += "=" * 80
$content += ""
$content += ""

foreach ($file in $csFiles) {
    # Relativer Pfad zum Assets Ordner
    $relativePath = $file.FullName.Substring($assetsPath.Length + 1)
    
    Write-Host "  Füge hinzu: $relativePath" -ForegroundColor Gray
    
    # Trennlinie mit Dateiinfo
    $content += ""
    $content += "=" * 80
    $content += "DATEI: $relativePath"
    $content += "Pfad: $($file.FullName)"
    $content += "=" * 80
    $content += ""
    
    # Dateiinhalt
    $fileContent = Get-Content $file.FullName -Raw -Encoding UTF8
    $content += $fileContent
    
    $content += ""
    $content += ""
}

# Füge Projektstruktur am Ende hinzu
$content += ""
$content += "=" * 80
$content += "PROJEKTSTRUKTUR"
$content += "=" * 80
$content += ""

# Zeige Ordnerstruktur
$allFolders = Get-ChildItem -Path $assetsPath -Directory -Recurse | Select-Object -ExpandProperty FullName
foreach ($folder in $allFolders) {
    $relFolder = $folder.Substring($assetsPath.Length + 1)
    $content += "  📁 $relFolder"
}

# Speichere die Datei
$content | Out-File -FilePath $outputFile -Encoding UTF8

$fileSize = [math]::Round((Get-Item $outputFile).Length / 1KB, 2)

Write-Host ""
Write-Host "✅ FERTIG!" -ForegroundColor Green
Write-Host "Datei erstellt: $outputFile" -ForegroundColor Cyan
Write-Host "Dateigröße: $fileSize KB" -ForegroundColor Cyan
Write-Host ""
Write-Host "Du kannst jetzt die Datei hochladen!" -ForegroundColor Yellow
Write-Host ""

Read-Host "Drücke Enter zum Beenden"