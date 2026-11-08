# Foscamun v2.0.2

**Release Date**: 2026-11-08

> **New interface.** The previous interface stays available as **v1.3.0 — old interface**. Both use the same database, so you can try them side by side.

## 🎉 What's New

### ⏱️ Session timer
- **Reset** button (round arrow next to Start/Pause): puts the clock back to the full speaking time, stopped, for the same speaker
- **The speaking time can be typed in**: click it, type and press Enter (Esc gives up)
  - `1:30` is a minute and a half
  - a number up to 10 is minutes (`2` = 2:00), a larger one is seconds (`45` = 0:45)
  - from 0:10 to 10:00; the matching preset lights up by itself
- `−` / `+` keep changing the time by 10 seconds, and the presets 1:00 / 1:30 / 2:00 / 3:00 still set it at once

Everything else is as in [v2.0.1](https://github.com/nanedb/Foscamun/releases/tag/v2.0.1) (Windows installer, automatic updates) and [v2.0.0](https://github.com/nanedb/Foscamun/releases/tag/v2.0.0) (new interface).

## 💾 Download

| Package | System |
|---|---|
| **Foscamun-Setup-win-x64-v2.0.2.exe** | Installer — Windows 10/11 64-bit (recommended) |
| **Foscamun-Setup-win-x86-v2.0.2.exe** | Installer — Windows 10/11 32-bit |
| **Foscamun-Setup-win-arm64-v2.0.2.exe** | Installer — Windows on ARM |
| **Foscamun-win-x64-v2.0.2.zip** | Portable — Windows 10/11 64-bit |
| **Foscamun-win-x86-v2.0.2.zip** | Portable — Windows 10/11 32-bit |
| **Foscamun-win-arm64-v2.0.2.zip** | Portable — Windows on ARM |

## 📥 Installation

**Installer**: run `Foscamun-Setup-…exe` and follow the steps. If Windows shows "Windows protected your PC" (the installer is not signed with a paid certificate), click **More info → Run anyway**.

**Portable**: extract the ZIP to a user folder (Desktop, Documents, USB drive…) — not `C:\Program Files\` — and run `Foscamun.exe`.

## 🔄 Upgrading

Versions 2.0.0 and 2.0.1 offer this update at startup. Your data in `%LocalAppData%\Foscamun\` is never touched by installing, updating or uninstalling.

The SHA256 checksums of all packages are in `SHA256SUMS.txt`.
