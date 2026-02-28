# 📖 Foscamun - Guida Rapida Utente

## 🚀 Primo Avvio

1. **Estrai** tutti i file dall'archivio ZIP
2. **Esegui** `Foscamun.exe`
3. Al primo avvio, il database verrà creato automaticamente

## ⚙️ Configurazione Iniziale

### 1. Seleziona la Lingua
- Scegli tra Inglese, Francese o Spagnolo
- La lingua può essere cambiata in qualsiasi momento

### 2. Imposta l'Anno
- Inserisci l'anno della conferenza MUN

### 3. Aggiungi una Commissione
1. Clicca su **"Aggiungi Commissione"**
2. Inserisci:
   - Nome commissione
   - Presidente
   - Vice-presidente
   - Moderatore
   - Tema
3. Clicca **"OK"**

### 4. Aggiungi Paesi alla Commissione
1. Seleziona la commissione
2. Clicca **"Modifica Commissione"**
3. Aggiungi i paesi partecipanti
4. Clicca **"Salva"**

## 📋 Gestire una Sessione

### Fase 1: Appello (Roll Call)

1. **Dal menu principale**, clicca su **"Appello"**
2. Seleziona:
   - Tema (già impostato nella commissione)
   - Numero di sessione
3. **Segna i presenti**:
   - Clicca su un paese nella lista "Disponibili"
   - Il paese si sposterà nella lista "Presenti"
   - Oppure usa **"Segna tutti presenti"**
4. Clicca **"Continua"** per iniziare la sessione

### Fase 2: Sessione

1. **Lista Oratori**:
   - Clicca su un paese in "Oratori Disponibili"
   - Il paese viene aggiunto alla "Lista Oratori"
   - L'ordine può essere gestito

2. **Oratore Corrente**:
   - Il primo nella lista diventa l'oratore corrente
   - Usa il **Timer** per gestire il tempo di intervento

3. **Gestione Oratore**:
   - **Rimuovi**: Toglie l'oratore dalla lista
   - **Avvisa**: Aggiunge un avviso all'oratore
   - **Rimuovi Avviso**: Toglie un avviso

4. **Quando serve votare**:
   - Clicca su **"Votazione"**

### Fase 3: Votazione

1. **Prima del voto**:
   - Tutti i paesi presenti sono disponibili per votare

2. **Durante il voto**, per ogni paese clicca:
   - **Approva** - Voto a favore
   - **Astieni** - Astensione
   - **Rifiuta** - Voto contro

3. **Risultati**:
   - Vengono mostrati i risultati del round
   - Clicca **"Nuovo Round"** per un altro giro di voti
   - Oppure **"Indietro"** per tornare alla sessione

4. **Risultati Finali**:
   - Mostra il risultato complessivo di tutti i round
   - Indica se la mozione è approvata o respinta

## 🏛️ CIJ (Corte Internazionale di Giustizia)

La CIJ funziona in modo simile ma con alcune differenze:

### Configurazione CIJ
1. Clicca **"Modifica CIJ"** dalla configurazione
2. Inserisci:
   - Giudice principale
   - Vice-giudice 1
   - Vice-giudice 2
   - Tema del caso
3. Aggiungi:
   - Avvocati
   - Giurati

### Sessione CIJ
- L'appello include avvocati e giurati
- La sessione funziona come per le commissioni normali
- Il sistema di votazione è lo stesso

## ⌨️ Scorciatoie da Tastiera

- **Home** → Torna alla pagina principale
- **Esc** → Torna indietro (dove disponibile)

## 💾 Dati e Backup

### Dove sono salvati i dati?
- Database: `Foscamun.db` (nella cartella dell'applicazione)
- Impostazioni: Salvate automaticamente

### Come fare un backup?
1. Chiudi l'applicazione
2. Copia il file `Foscamun.db`
3. Conserva la copia in un luogo sicuro

### Come ripristinare un backup?
1. Chiudi l'applicazione
2. Sostituisci `Foscamun.db` con la copia di backup
3. Riavvia l'applicazione

## 🎨 Personalizzazione

### Loghi Commissioni
I loghi si trovano in: `Resources\CommitteeLogo\`

**Per aggiungere un logo personalizzato:**
1. Crea un file SVG con il nome della commissione (es. `ECOSOC.svg`)
2. Posizionalo in `Resources\CommitteeLogo\`
3. Riavvia l'applicazione

**Logo generico:** Se non c'è un logo specifico, viene usato `Generic.svg`

### Suoni
I file audio si trovano in: `Resources\Sounds\`
- Formato supportato: WAV
- Usati per notifiche e timer

## ❓ Problemi Comuni

### L'applicazione non si avvia
- Verifica di avere Windows 10 o superiore
- Prova a eseguire come amministratore (click destro → "Esegui come amministratore")

### Database non trovato
- Verifica che `Foscamun.db` sia nella stessa cartella di `Foscamun.exe`
- Se mancante, l'applicazione lo ricreerà automaticamente (vuoto)

### I loghi non si vedono
- Verifica che la cartella `Resources\CommitteeLogo\` esista
- Verifica che i file SVG siano validi
- Usa `Generic.svg` come fallback

### Lingua non cambia
- Riavvia l'applicazione dopo aver cambiato la lingua

## 📧 Supporto

- **Bug o problemi?** Apri una issue su: https://github.com/nanedb/Foscamun2026/issues
- **Domande?** Usa le discussioni: https://github.com/nanedb/Foscamun2026/discussions

## 📚 Risorse Aggiuntive

- **Documentazione completa**: [README.md](README.md)
- **Guida deployment**: [DEPLOYMENT.md](DEPLOYMENT.md)
- **Release notes**: [RELEASE_NOTES_TEMPLATE.md](RELEASE_NOTES_TEMPLATE.md)

---

**Versione**: 1.0.0  
**Ultimo aggiornamento**: 2025  
**Repository**: https://github.com/nanedb/Foscamun2026
