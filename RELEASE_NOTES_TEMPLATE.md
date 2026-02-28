# Foscamun v1.0.0

**Release Date**: [Date]

## 🎉 What's New

- Complete MUN committee management system
- ICJ (International Court of Justice) support
- Roll call system for delegates
- Session management with speakers list
- Multi-round voting system
- Integrated timer for speeches
- Multi-language support (English, French, Spanish)
- SQLite database for data persistence
- Customizable committee logos

## 📋 Features

### Committee Management
- Create and edit committees
- Assign president, vice-president, and moderator
- Manage participating countries
- Topics and sessions

### Session System
- Dynamic speakers list
- Timer for speeches
- Warning system
- Remove speakers

### Voting System
- Voting for approval/abstention/rejection
- Multiple voting rounds
- Final results with statistics

### Special ICJ
- Manage judges (main and vice)
- Manage advocates and jurors
- Custom voting system

## 💾 Download

Choose the appropriate version for your system:

### Windows 64-bit (Recommended)
**Foscamun-win-x64-v1.0.0.zip** (~68 MB)
- For most modern PCs
- Windows 10/11 64-bit

### Windows 32-bit
**Foscamun-win-x86-v1.0.0.zip** (~63 MB)
- For older PCs
- Windows 10/11 32-bit

### Windows ARM64
**Foscamun-win-arm64-v1.0.0.zip** (~64 MB)
- For Windows ARM devices (e.g., Surface Pro X)

## 📥 Installation

1. Download the appropriate ZIP file
2. Extract contents to a folder
3. Run `Foscamun.exe`
4. No installation required - application is self-contained

## ⚙️ System Requirements

- **Operating System**: Windows 10 or higher
- **RAM**: 4 GB minimum (8 GB recommended)
- **Disk Space**: 500 MB free
- **Resolution**: 1024x768 minimum (1920x1080 recommended)

## 🐛 Bug Fixes

- Fixed XML encoding errors with special characters
- Fixed Back navigation from SessionPage to RollCallPage
- Normalized language codes (en-US → en)
- Optimized resource management

## 📚 Documentation

For more information see:
- [DEPLOYMENT.md](DEPLOYMENT.md) - Deployment guide
- [README.md](README.md) - Main documentation

## 🔄 Upgrading from Previous Versions

First installation - no upgrade needed.

## 🆘 Support

- **Issues**: https://github.com/nanedb/Foscamun2026/issues
- **Discussions**: https://github.com/nanedb/Foscamun2026/discussions

## 📝 Notes

- Database `Foscamun.db` is created automatically on first launch
- Settings are saved automatically
- Committee logos can be customized in `Resources\CommitteeLogo\`

---

**SHA256 Checksums** (for integrity verification):
```
Foscamun-win-x64-v1.0.0.zip:   [To be calculated]
Foscamun-win-x86-v1.0.0.zip:   [To be calculated]
Foscamun-win-arm64-v1.0.0.zip: [To be calculated]
```

**Repository**: https://github.com/nanedb/Foscamun2026  
**License**: [Your license]
