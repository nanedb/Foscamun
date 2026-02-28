# ✅ Checklist Deployment Foscamun

## Pre-Deployment

### Codice
- [ ] Tutte le modifiche sono committate
- [ ] Build locale senza errori (`dotnet build`)
- [ ] Nessun warning critico
- [ ] Tutti i TODO risolti o documentati
- [ ] Codice rivisto e pulito

### Testing
- [ ] Test manuale completo dell'applicazione
- [ ] Test creazione commissione
- [ ] Test appello (Roll Call)
- [ ] Test sessione e lista oratori
- [ ] Test votazione singola e multipla
- [ ] Test CIJ completo
- [ ] Test cambio lingua (EN, FR, ES)
- [ ] Test database (creazione, lettura, scrittura)
- [ ] Test loghi personalizzati

### Documentazione
- [ ] README.md aggiornato
- [ ] DEPLOYMENT.md creato e aggiornato
- [ ] USER_GUIDE.md completo
- [ ] RELEASE_NOTES aggiornate con nuove funzionalità
- [ ] Commenti nel codice verificati
- [ ] Version number aggiornato in `Foscamun.csproj`

### Risorse
- [ ] Tutti i loghi sono in `Resources\CommitteeLogo\`
- [ ] Loghi SVG validi
- [ ] File audio in `Resources\Sounds\`
- [ ] Icone in `Resources\Icons\`
- [ ] Font in `Fonts\`
- [ ] Database template `Foscamun.db` funzionante

## Build Locale

### Script Automatico
- [ ] Eseguito `.\publish.ps1 -Version "1.0.0"`
- [ ] Verificato output in `.\Publish\`
- [ ] Test eseguibile Windows x64
- [ ] Test eseguibile Windows x86 (se disponibile)
- [ ] Verificati file inclusi in ogni archivio
- [ ] Checksum SHA256 calcolati

### Test Eseguibili
- [ ] `Foscamun.exe` si avvia correttamente
- [ ] Database creato al primo avvio
- [ ] Tutte le funzionalità operative
- [ ] Nessun errore in console
- [ ] Loghi caricati correttamente
- [ ] Cambio lingua funzionante

## Git e GitHub

### Repository
- [ ] `.gitignore` aggiornato
- [ ] Nessun file sensibile committato
- [ ] Tutte le modifiche pushate
```bash
git status  # Verifica che tutto sia pulito
```

### Tag Version
- [ ] Tag creato per la versione
```bash
git tag v1.0.0
git push origin v1.0.0
```

### GitHub Actions (Opzionale)
- [ ] Workflow `.github/workflows/build-release.yml` committato
- [ ] Workflow testato con push di un tag
- [ ] Build su GitHub Actions completato con successo

## Release GitHub

### Preparazione
- [ ] Vai su: https://github.com/nanedb/Foscamun2026/releases
- [ ] Clicca "Create a new release"
- [ ] Seleziona tag `v1.0.0`

### Informazioni Release
- [ ] **Tag version**: v1.0.0
- [ ] **Release title**: Foscamun v1.0.0
- [ ] **Description**: Copiato da RELEASE_NOTES_TEMPLATE.md
- [ ] Aggiornata data di rilascio
- [ ] Aggiunte note specifiche della versione

### File da Caricare
- [ ] `Foscamun-win-x64-v1.0.0.zip`
- [ ] `Foscamun-win-x86-v1.0.0.zip`
- [ ] `Foscamun-win-arm64-v1.0.0.zip`
- [ ] `SHA256SUMS.txt`

### Verifiche Finali
- [ ] Checksum SHA256 aggiunti nelle release notes
- [ ] Link di download testati
- [ ] README della release verificato
- [ ] Release pubblicata (non come draft)

## Post-Deployment

### Verifica Download
- [ ] Download da GitHub completato con successo
- [ ] Archivi ZIP si estraggono correttamente
- [ ] Checksum verificato:
```powershell
Get-FileHash -Path "Foscamun-win-x64-v1.0.0.zip" -Algorithm SHA256
```

### Test Download Pubblico
- [ ] Scarica come utente anonimo
- [ ] Estrai i file
- [ ] Esegui l'applicazione
- [ ] Verifica che tutto funzioni

### Comunicazione
- [ ] Annuncio pubblicato (se applicabile)
- [ ] Link alla release condiviso
- [ ] Documentazione di supporto disponibile

## Troubleshooting

### Build Fallisce
```powershell
# Pulisci e rebuilda
dotnet clean
dotnet restore
dotnet build -c Release
```

### File Mancanti nell'Output
Verifica in `Foscamun.csproj`:
- [ ] `<Resource>` tags per SVG, PNG, ICO
- [ ] `<None Include>` con `<CopyToOutputDirectory>` per WAV e database
- [ ] `<Content Include>` per il database

### Dimensione File Troppo Grande
Opzioni per ridurre:
- [ ] Rimuovi `-p:PublishReadyToRun=true`
- [ ] Aggiungi `-p:PublishTrimmed=true` (con attenzione!)
- [ ] Usa framework-dependent invece di self-contained

### GitHub Actions Fallisce
- [ ] Verifica che il workflow file sia valido (YAML syntax)
- [ ] Controlla i log su GitHub Actions
- [ ] Verifica che `GITHUB_TOKEN` sia disponibile
- [ ] Assicurati che il tag segua il pattern `v*.*.*`

## Note Finali

### Versioni Future
Per rilasciare v1.1.0:
1. Aggiorna `<Version>` in `Foscamun.csproj`
2. Aggiorna RELEASE_NOTES
3. Commit e push
4. Crea tag `v1.1.0`
5. Esegui `.\publish.ps1 -Version "1.1.0"`
6. Crea release su GitHub

### Backup
- [ ] Backup del repository locale
- [ ] Backup del database di produzione
- [ ] Backup delle risorse personalizzate

---

**Data Completamento**: ___________  
**Versione**: v1.0.0  
**Release Manager**: ___________
