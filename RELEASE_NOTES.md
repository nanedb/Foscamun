# Foscamun v2.0.0

**Release Date**: 2026-11-08

> **New interface.** This release and **v1.3.0 — old interface** are both available, so you can try them side by side and choose. They share the same database: committees, countries and ICJ data work in both.

## 🎉 What's New

### 🎨 New interface
- New sidebar: previous page, current page and next page of the flow, with Setup and Exit at the bottom; it can be collapsed to icons only
- Redesigned pages: Home, Setup, Roll call, Session, Voting, Result and Final result, for committees and ICJ
- **Session** with the timer built into the page: speaking time ±15 s, presets 1:00 / 1:30 / 2:00 / 3:00, Start/Pause (also with the space bar), Next, warnings, sounds
- **Voting** one delegate at a time, with the keys A (approve), S (abstain), D (reject)
- **Result** and **Final result** with a bar per round, who voted what, and the history of every round of the vote
- Animated page changes
- App-styled dialogs instead of the Windows message boxes

### 🔄 Automatic updates
- At startup Foscamun checks GitHub for a newer version and offers to download and install it; the download is verified against the published SHA256 checksum
- "Don't remind me about this version" to skip a release

### 💾 Database
- The existing database is always kept. If it was saved by a newer version or cannot be read, a warning offers to set it aside (renamed, never deleted) and start with a new one, or to exit without changing anything

## 💾 Download

| Package | System |
|---|---|
| **Foscamun-win-x64-v2.0.0.zip** | Windows 10/11 64-bit (recommended) |
| **Foscamun-win-x86-v2.0.0.zip** | Windows 10/11 32-bit (older PCs) |
| **Foscamun-win-arm64-v2.0.0.zip** | Windows ARM devices (e.g., Surface Pro X) |

## 📥 Installation

1. Download the appropriate ZIP file
2. Extract contents to a user folder (Desktop, Documents, etc.) — avoid protected folders like `C:\Program Files\`, otherwise automatic updates cannot replace the files
3. Run `Foscamun.exe` — no installation or administrator privileges required

To compare with the previous interface, extract **v1.3.0** into a different folder.

## 🔄 Upgrading from Previous Versions

Extract and run — the database `Foscamun.db` and committee logos live in `%LocalAppData%\Foscamun\` and are not affected by replacing the executable. Settings are kept when the new version is extracted into the same folder as the old one.
