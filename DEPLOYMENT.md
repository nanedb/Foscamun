# 📦 Foscamun Deployment Guide

## Quick Start

### Opzione 1: Script Automatico (Consigliato)

**PowerShell** (tutte le piattaforme):
```powershell
.\publish.ps1 -Version "1.0.0"
```

**Batch** (solo Windows x64):
```cmd
publish-quick.bat
```

### Opzione 2: Comando Manuale

```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true
```

## 🎯 Output

I file pubblicati saranno in:
- `.\Publish\Foscamun-win-x64-v{version}\` - Windows 64-bit
- `.\Publish\Foscamun-win-x86-v{version}\` - Windows 32-bit  
- `.\Publish\Foscamun-win-arm64-v{version}\` - Windows ARM64

Gli archivi ZIP saranno creati automaticamente.

## 📋 Checklist Pre-Deployment

- [ ] Versione aggiornata in `Foscamun.csproj` (proprietà `<Version>`)
- [ ] Build locale funzionante senza errori
- [ ] Test completo dell'applicazione
- [ ] Verificare che tutti i loghi siano in `Resources\CommitteeLogo\`
- [ ] Verificare che i suoni siano in `Resources\Sounds\`
- [ ] Database `Foscamun.db` presente e funzionante
- [ ] File README creati per ogni versione

## 🚀 Deployment su GitHub

### Creare una Release

1. Commit e push delle modifiche:
```bash
git add .
git commit -m "Release v1.0.0"
git push origin master
```

2. Creare un tag:
```bash
git tag v1.0.0
git push origin v1.0.0
```

3. Vai su GitHub → Releases → "Create a new release"
4. Seleziona il tag `v1.0.0`
5. Titolo: "Foscamun v1.0.0"
6. Carica i file ZIP da `.\Publish\`:
   - Foscamun-win-x64-v1.0.0.zip
   - Foscamun-win-x86-v1.0.0.zip
   - Foscamun-win-arm64-v1.0.0.zip

## 📄 Note Importanti

### Self-Contained vs Framework-Dependent

**Self-Contained** (Consigliato - usato dagli script):
- ✅ Include il runtime .NET 10
- ✅ Funziona su qualsiasi PC Windows (non serve installare .NET)
- ✅ Dimensione: ~150-200 MB
- ✅ Single file: un solo eseguibile

**Framework-Dependent** (Opzionale):
- ⚠️ Richiede .NET 10 Desktop Runtime installato
- ✅ Dimensione più piccola: ~5-10 MB
- ⚠️ L'utente deve scaricare .NET 10 da: https://dotnet.microsoft.com/download/dotnet/10.0

### File Inclusi nel Deployment

Verifica che siano presenti:
- `Foscamun.exe` - Eseguibile principale
- `Foscamun.db` - Database SQLite
- `Resources\CommitteeLogo\*.svg` - Loghi commissioni
- `Resources\Sounds\*.wav` - File audio

### Requisiti Utente Finale

**Self-Contained:**
- Windows 10 o superiore
- 64-bit, 32-bit o ARM64 (a seconda della versione)

**Framework-Dependent:**
- Windows 10 o superiore
- .NET 10 Desktop Runtime

## 🔧 Troubleshooting

### "Il file non può essere eseguito"
- Verifica che l'architettura sia corretta (x64, x86, ARM64)
- Fai click destro → Proprietà → "Sblocca" se scaricato da internet

### "Errore durante l'avvio dell'applicazione"
- Verifica che `Foscamun.db` sia presente
- Verifica i permessi di lettura/scrittura nella cartella

### Single File non funziona
Rimuovi `-p:PublishSingleFile=true` dal comando publish

## 📊 Dimensioni Previste

- **win-x64**: ~150 MB (single file) / ~180 MB (estratto)
- **win-x86**: ~130 MB (single file) / ~160 MB (estratto)
- **win-arm64**: ~140 MB (single file) / ~170 MB (estratto)

## 🆕 Aggiornamenti Futuri

Per rilasciare una nuova versione:
1. Aggiorna `<Version>` in `Foscamun.csproj`
2. Esegui `.\publish.ps1 -Version "1.1.0"`
3. Crea una nuova release su GitHub con il nuovo tag

---

**Repository**: https://github.com/nanedb/Foscamun2026  
**License**: (Aggiungi la tua licenza)
