# Script per calcolare SHA256 checksum dei file pubblicati
# Utile per verificare l'integrità dei download

param(
    [string]$PublishFolder = ".\Publish"
)

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "   Foscamun - Calcolo Checksum SHA256" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""

if (-not (Test-Path $PublishFolder)) {
    Write-Host "Cartella $PublishFolder non trovata!" -ForegroundColor Red
    Write-Host "Esegui prima publish.ps1 per creare i file da pubblicare." -ForegroundColor Yellow
    exit 1
}

# Cerca tutti i file ZIP
$zipFiles = Get-ChildItem -Path $PublishFolder -Filter "*.zip" -Recurse

if ($zipFiles.Count -eq 0) {
    Write-Host "Nessun file ZIP trovato in $PublishFolder" -ForegroundColor Red
    exit 1
}

Write-Host "File trovati: $($zipFiles.Count)" -ForegroundColor Green
Write-Host ""

# Crea un file con i checksum
$checksumFile = Join-Path $PublishFolder "SHA256SUMS.txt"
$checksumContent = @()
$checksumContent += "# Foscamun - SHA256 Checksums"
$checksumContent += "# Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
$checksumContent += ""

foreach ($file in $zipFiles) {
    Write-Host "Calcolo checksum per: $($file.Name)..." -ForegroundColor Yellow
    
    # Calcola SHA256
    $hash = Get-FileHash -Path $file.FullName -Algorithm SHA256
    
    Write-Host "  SHA256: $($hash.Hash)" -ForegroundColor Green
    Write-Host ""
    
    # Aggiungi al contenuto
    $checksumContent += "$($hash.Hash)  $($file.Name)"
}

# Salva il file
$checksumContent | Out-File -FilePath $checksumFile -Encoding UTF8

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "   Checksum salvati in:" -ForegroundColor Green
Write-Host "   $checksumFile" -ForegroundColor Yellow
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""

# Mostra il contenuto
Write-Host "Contenuto del file SHA256SUMS.txt:" -ForegroundColor Cyan
Write-Host ""
Get-Content $checksumFile | ForEach-Object { Write-Host $_ -ForegroundColor White }

Write-Host ""
Write-Host "Copia questo contenuto nelle release notes di GitHub!" -ForegroundColor Yellow
