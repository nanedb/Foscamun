# 📖 Foscamun - User Guide

This guide describes **version 2.0.0** (new interface). Version 1.3.0 (old interface) works on the same data.

## 🚀 First launch

1. **Extract** the ZIP to a user folder (Desktop, Documents…)
   - ⚠️ Avoid protected folders like `C:\Program Files\`: Foscamun could not update itself there
2. **Run** `Foscamun.exe` — no installation and no administrator privileges needed
3. On the first launch the database is created in `%LocalAppData%\Foscamun\`

## 🧭 Moving around

The sidebar on the left shows where you are in the flow:

- at the top: the **previous** page (arrow left), the **current** page (check mark) and the **next** page (arrow right). A dimmed next page arrives by itself (for instance Result, after the last vote)
- at the bottom: **Setup** (gear) and **Exit** (door)
- the ☰ button opens the sidebar with the page names, or closes it to icons only

The flow is: **Home → Roll call → Session → Voting → Result → Final result**.

## ⚙️ Setup

Open **Setup** with the gear at the bottom of the sidebar.

- **Select Year** — the conference year, shown on the Home page
- **Select Language** — English, Français, Español; the change is immediate
- **Ask before exiting** — whether Exit asks for confirmation
- **Select Committee** — click a committee to make it the current one; Roll call opens it

### Committees

1. **Add Committee**: enter name, topic, president, vice-president and moderator, then choose the participating countries
2. **Edit Committee**: changes the selected committee (click a country in its list to remove it)
3. **Remove**: deletes the selected committee

### International Court of Justice

1. **Add ICJ** (or **Edit ICJ** once it exists)
2. Enter the topic, the **judge** and the two **vice judges**
3. Add **plaintiff** and **defense** advocates, each with the country they represent, and the **jurors**
4. Saving needs at least one plaintiff, one defense advocate and one juror

### Committee logos

Logos are SVG files in `%LocalAppData%\Foscamun\CommitteeLogo\`, named after the committee (e.g. `ECOSOC.svg`), white on a transparent background. A committee without a logo gets a copy of the generic one, which you can replace.

## 📋 Roll call

1. Choose the **Session** number at the top right
2. Click a country in **Absent** to mark it present, or in **Present** to mark it absent; **Mark all present** does it for everyone
3. The bar shows how many are present and absent
4. **Proceed to session**

For the ICJ the lists contain advocates (with their country's flag) and jurors.

## 🎤 Session

- **Available** (left): click a country to add it to the **Speakers list**; click it in the speakers list to send it back
- The first in the speakers list is **now speaking**, shown with flag and name
- **Timer**:
  - **Start / Pause** — also with the **space bar**
  - **Speaking time** — `−` / `+` change it by 15 seconds; the presets 1:00, 1:30, 2:00, 3:00 set it at once
  - **Next** — the next speaker takes the floor
  - the last seconds tick, a bell rings at the end, and the panel turns orange in overtime
- **Warn** gives a warning to the current speaker; the **Warnings** list keeps the count. Select a delegate there and click **Remove Warning** to take one away
- **Open voting** starts the vote among the present delegates

## 🗳️ Voting

One delegate at a time, with the buttons or the keyboard:

| Key | Vote |
|---|---|
| **A** | Approve |
| **S** | Abstain (not available in the rounds without abstaining) |
| **D** | Reject |

Under the name you see how many have voted and who comes next. The previous-page link goes back to the session (votes already cast are lost, so it asks first).

### Rules

| Round | Committees | ICJ |
|---|---|---|
| 1 | Decided only by **unanimity** | **Majority of the votes cast** (abstentions do not count) |
| 2 | **Two-thirds majority** of the votes cast | **Simple majority**, no abstaining |
| 3 | **Simple majority**, no abstaining; a tie rejects | **Simple majority**, no abstaining; a tie rejects |

### Result

After a round with no decision, the **Result** page shows why the vote goes on (no unanimity, no two-thirds majority, no majority, tie), the tally bar (green in favor, orange abstained, red against) and who voted what.

- **Next round** — starts the next round
- **Back to session** — ends the vote and returns to the session

### Final result

When the vote is decided, **Final result** shows:

- the outcome (adopted / rejected) and the final tally
- a row for every round, with its bar, its tally and why it did not decide; the decisive round is highlighted
- who voted what in the decisive round

From here go **Back to session** or **Home**.

## 🔄 Updates

At every start Foscamun checks GitHub for a newer version. If there is one:

- **Yes** downloads it (a window shows the progress; **Cancel** stops it), checks it, closes Foscamun, replaces the files and starts the new version
- **No** asks again at the next start; tick **Don't remind me about this version** to skip that version

Your committees, settings and logos are kept. Without an internet connection nothing is shown. If Foscamun cannot write to its own folder, it offers to open the download page instead.

## 💾 Data and backup

### Where is the data?

- Database: `%LocalAppData%\Foscamun\Foscamun.db` (paste the path in File Explorer's address bar)
- Committee logos: `%LocalAppData%\Foscamun\CommitteeLogo\`
- Settings (language, year, selected committee): saved automatically

The `Foscamun.db` next to `Foscamun.exe` is only the starting database, copied on the first launch.

### Backup

1. Close Foscamun
2. Copy `%LocalAppData%\Foscamun\Foscamun.db` somewhere safe

### Restore

1. Close Foscamun
2. Put the backup copy in `%LocalAppData%\Foscamun\` with the name `Foscamun.db`
3. Start Foscamun

### "Incompatible database"

If the database was saved by a newer version of Foscamun, or cannot be read, Foscamun warns at startup:

- **Yes** — the current file is renamed (e.g. `Foscamun-backup-20261108-184515.db`, never deleted) and Foscamun starts with a new database
- **No** — Foscamun closes without changing anything; you can install the newer version instead

## ❓ Common issues

### The application does not start
- Windows 10 or later is required
- If Windows blocks the downloaded file: right-click the ZIP → Properties → **Unblock**, then extract again
- Make sure you downloaded the right architecture (x64, x86, ARM64)

### Updates do not install
- Foscamun must be able to write to its folder: move it out of `C:\Program Files\` to a user folder

### A logo is missing
- Check that the SVG in `%LocalAppData%\Foscamun\CommitteeLogo\` has exactly the committee's name

## 📧 Support

- **Bugs or requests**: https://github.com/nanedb/Foscamun/issues

## 📚 More

- [README.md](README.md) — overview
- [DEPLOYMENT.md](DEPLOYMENT.md) — building and releasing

---

**Version**: 2.0.0
**Last updated**: November 2026
**Repository**: https://github.com/nanedb/Foscamun
