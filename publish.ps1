# Foscamun Deployment Script
# Questo script crea versioni self-contained per Windows x64, x86 e ARM64

param(
    [string]$Version = "1.0.0",
    [switch]$SkipBuild = $false
)

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "   Foscamun Deployment Script v$Version" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""

# Pulisci le directory precedenti
Write-Host "Pulizia directory precedenti..." -ForegroundColor Yellow
if (Test-Path ".\Publish") {
    Remove-Item ".\Publish" -Recurse -Force
}

# Crea directory di output
New-Item -ItemType Directory -Force -Path ".\Publish" | Out-Null

# Configurazioni di build
$configurations = @(
    @{Name="win-x64"; Display="Windows 64-bit"},
    @{Name="win-x86"; Display="Windows 32-bit"},
    @{Name="win-arm64"; Display="Windows ARM64"}
)

foreach ($config in $configurations) {
    Write-Host ""
    Write-Host "Building $($config.Display)..." -ForegroundColor Green
    
    $outputPath = ".\Publish\Foscamun-$($config.Name)-v$Version"
    
    # Publish command
    dotnet publish `
        -c Release `
        -r $($config.Name) `
        --self-contained true `
        -p:PublishSingleFile=true `
        -p:IncludeNativeLibrariesForSelfExtract=true `
        -p:EnableCompressionInSingleFile=true `
        -p:PublishReadyToRun=true `
        -p:Version=$Version `
        -o $outputPath
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Build completata per $($config.Display)" -ForegroundColor Green
        
        # Crea file README per ogni versione
        $readmeContent = @"
Foscamun v$Version - $($config.Display)

INSTALLAZIONE:
1. Estrai tutti i file in una cartella
2. Esegui Foscamun.exe

REQUISITI:
- $($config.Display)
- Sistema operativo: Windows 10 o superiore

NOTE:
- Il database Foscamun.db verrà creato automaticamente al primo avvio
- I loghi delle commissioni sono inclusi nella cartella Resources\CommitteeLogo
- I suoni sono nella cartella Resources\Sounds

SUPPORTO:
Repository GitHub: https://github.com/nanedb/Foscamun2026
"@
        
        Set-Content -Path "$outputPath\README.txt" -Value $readmeContent
        
        # Crea archivio ZIP
        Write-Host "Creazione archivio ZIP..." -ForegroundColor Yellow
        $zipPath = ".\Publish\Foscamun-$($config.Name)-v$Version.zip"
        Compress-Archive -Path "$outputPath\*" -DestinationPath $zipPath -Force
        Write-Host "✓ Archivio creato: $zipPath" -ForegroundColor Green
    }
    else {
        Write-Host "✗ Errore durante il build di $($config.Display)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "   Deployment completato!" -ForegroundColor Green
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "I file sono disponibili in: .\Publish\" -ForegroundColor Yellow
Write-Host ""

# Calcola checksum SHA256
Write-Host "Calcolo checksum SHA256..." -ForegroundColor Yellow
& ".\calculate-checksums.ps1" -PublishFolder ".\Publish"

Write-Host ""
Write-Host "Deployment completato con successo!" -ForegroundColor Green
Write-Host ""

# Apri la cartella Publish
explorer.exe ".\Publish\"
