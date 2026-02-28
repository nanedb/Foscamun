# Foscamun v1.0.0

**Data di rilascio**: [Data]

## 🎉 Novità

- Sistema di gestione commissioni MUN completo
- Supporto per CIJ (Corte Internazionale di Giustizia)
- Sistema di appello (Roll Call) per delegati
- Gestione sessioni con lista oratori
- Sistema di votazione con round multipli
- Timer integrato per gli interventi
- Supporto multilingua (Inglese, Francese, Spagnolo)
- Database SQLite per persistenza dati
- Loghi personalizzabili per commissioni

## 📋 Funzionalità

### Gestione Commissioni
- Creazione e modifica commissioni
- Assegnazione presidente, vice-presidente e moderatore
- Gestione paesi partecipanti
- Temi e sessioni

### Sistema di Sessione
- Lista oratori dinamica
- Timer per interventi
- Sistema di avvisi
- Rimozione oratori

### Sistema di Voto
- Votazione per approvazione/astensione/reiezione
- Round multipli di votazione
- Risultati finali con statistiche

### CIJ Speciale
- Gestione giudici (principale e vice)
- Gestione avvocati e giurati
- Sistema di votazione personalizzato

## 💾 Download

Scegli la versione appropriata per il tuo sistema:

### Windows 64-bit (Consigliato)
**Foscamun-win-x64-v1.0.0.zip** (~150 MB)
- Per la maggior parte dei PC moderni
- Windows 10/11 64-bit

### Windows 32-bit
**Foscamun-win-x86-v1.0.0.zip** (~130 MB)
- Per PC più vecchi
- Windows 10/11 32-bit

### Windows ARM64
**Foscamun-win-arm64-v1.0.0.zip** (~140 MB)
- Per dispositivi Windows ARM (es. Surface Pro X)

## 📥 Installazione

1. Scarica il file ZIP appropriato
2. Estrai il contenuto in una cartella
3. Esegui `Foscamun.exe`
4. Nessuna installazione richiesta - l'applicazione è self-contained

## ⚙️ Requisiti di Sistema

- **Sistema Operativo**: Windows 10 o superiore
- **Memoria RAM**: 4 GB minimo (8 GB consigliato)
- **Spazio su disco**: 500 MB liberi
- **Risoluzione**: 1024x768 minimo (1920x1080 consigliato)

## 🐛 Problemi Risolti

- Risolti errori di codifica XML con caratteri speciali
- Corretta navigazione Back da SessionPage a RollCallPage
- Normalizzazione codici lingua (en-US → en)
- Ottimizzazione gestione risorse

## 📚 Documentazione

Per maggiori informazioni consulta:
- [DEPLOYMENT.md](DEPLOYMENT.md) - Guida al deployment
- [README.md](README.md) - Documentazione principale

## 🔄 Aggiornamento da Versioni Precedenti

Prima installazione - nessun aggiornamento necessario.

## 🆘 Supporto

- **Issues**: https://github.com/nanedb/Foscamun2026/issues
- **Discussioni**: https://github.com/nanedb/Foscamun2026/discussions

## 📝 Note

- Il database `Foscamun.db` viene creato automaticamente al primo avvio
- Le impostazioni sono salvate automaticamente
- I loghi delle commissioni possono essere personalizzati in `Resources\CommitteeLogo\`

---

**Checksum SHA256** (per verifica integrità):
```
Foscamun-win-x64-v1.0.0.zip:   [Da calcolare]
Foscamun-win-x86-v1.0.0.zip:   [Da calcolare]
Foscamun-win-arm64-v1.0.0.zip: [Da calcolare]
```

**Repository**: https://github.com/nanedb/Foscamun2026  
**Licenza**: [La tua licenza]
