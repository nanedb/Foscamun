# Foscamun v2.0.1

**Release Date**: 2026-11-08

> **New interface.** The previous interface stays available as **v1.3.0 — old interface**. Both use the same database, so you can try them side by side.

## 🎉 What's New

### 📦 Windows installer
- New **`Foscamun-Setup-…exe`**: installs Foscamun for your user in `%LocalAppData%\Programs\Foscamun`, with a Start menu shortcut and an optional desktop one; no administrator rights needed
- Uninstall from **Settings → Apps** like any other program; your committees, database and logos in `%LocalAppData%\Foscamun` are kept
- The portable ZIP is still available

### 🔄 Updates
- An installed Foscamun updates itself with the new installer, silently, and starts again; a portable copy keeps updating from the ZIP
- Settings (language, year, selected committee) are carried over from the copy you used last, also when switching from the ZIP to the installer

Everything else is as in [v2.0.0](https://github.com/nanedb/Foscamun/releases/tag/v2.0.0): new interface, timer in the session page, voting with A / S / D, rounds history in Final result, database compatibility check.

## 💾 Download

| Package | System |
|---|---|
| **Foscamun-Setup-win-x64-v2.0.1.exe** | Installer — Windows 10/11 64-bit (recommended) |
| **Foscamun-Setup-win-x86-v2.0.1.exe** | Installer — Windows 10/11 32-bit |
| **Foscamun-Setup-win-arm64-v2.0.1.exe** | Installer — Windows on ARM |
| **Foscamun-win-x64-v2.0.1.zip** | Portable — Windows 10/11 64-bit |
| **Foscamun-win-x86-v2.0.1.zip** | Portable — Windows 10/11 32-bit |
| **Foscamun-win-arm64-v2.0.1.zip** | Portable — Windows on ARM |

## 📥 Installation

**Installer**: run `Foscamun-Setup-…exe` and follow the steps. If Windows shows "Windows protected your PC" (the installer is not signed with a paid certificate), click **More info → Run anyway**.

**Portable**: extract the ZIP to a user folder (Desktop, Documents, USB drive…) — not `C:\Program Files\` — and run `Foscamun.exe`.

## 🔄 Upgrading

Version 2.0.0 offers this update at startup. Your data in `%LocalAppData%\Foscamun\` is never touched by installing, updating or uninstalling.

The SHA256 checksums of all packages are in `SHA256SUMS.txt`.
