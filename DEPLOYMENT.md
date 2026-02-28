# 📦 Foscamun Deployment Guide

## Quick Start

### Option 1: Automated Script (Recommended)

**PowerShell** (all platforms):
```powershell
.\publish.ps1 -Version "1.0.0"
```

**Batch** (Windows x64 only):
```cmd
publish-quick.bat
```

### Option 2: Manual Command

```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true
```

## 🎯 Output

Published files will be in:
- `.\Publish\Foscamun-win-x64-v{version}\` - Windows 64-bit
- `.\Publish\Foscamun-win-x86-v{version}\` - Windows 32-bit  
- `.\Publish\Foscamun-win-arm64-v{version}\` - Windows ARM64

ZIP archives will be created automatically.

## 📋 Pre-Deployment Checklist

- [ ] Version updated in `Foscamun.csproj` (`<Version>` property)
- [ ] Local build working without errors
- [ ] Complete application testing
- [ ] Verify all logos are in `Resources\CommitteeLogo\`
- [ ] Verify sounds are in `Resources\Sounds\`
- [ ] Database `Foscamun.db` present and working
- [ ] README files created for each version

## 🚀 Deploying to GitHub

### Creating a Release

1. Commit and push changes:
```bash
git add .
git commit -m "Release v1.0.0"
git push origin master
```

2. Create a tag:
```bash
git tag v1.0.0
git push origin v1.0.0
```

3. Go to GitHub → Releases → "Create a new release"
4. Select tag `v1.0.0`
5. Title: "Foscamun v1.0.0"
6. Upload ZIP files from `.\Publish\`:
   - Foscamun-win-x64-v1.0.0.zip
   - Foscamun-win-x86-v1.0.0.zip
   - Foscamun-win-arm64-v1.0.0.zip

## 📄 Important Notes

### Self-Contained vs Framework-Dependent

**Self-Contained** (Recommended - used by scripts):
- ✅ Includes .NET 10 runtime
- ✅ Works on any Windows PC (no need to install .NET)
- ✅ Size: ~150-200 MB
- ✅ Single file: one executable only

**Framework-Dependent** (Optional):
- ⚠️ Requires .NET 10 Desktop Runtime installed
- ✅ Smaller size: ~5-10 MB
- ⚠️ User must download .NET 10 from: https://dotnet.microsoft.com/download/dotnet/10.0

### Files Included in Deployment

Verify these are present:
- `Foscamun.exe` - Main executable
- `Foscamun.db` - SQLite database
- `Resources\CommitteeLogo\*.svg` - Committee logos
- `Resources\Sounds\*.wav` - Audio files

### End User Requirements

**Self-Contained:**
- Windows 10 or higher
- 64-bit, 32-bit, or ARM64 (depending on version)

**Framework-Dependent:**
- Windows 10 or higher
- .NET 10 Desktop Runtime

## 🔧 Troubleshooting

### "File cannot be executed"
- Verify the architecture is correct (x64, x86, ARM64)
- Right-click → Properties → "Unblock" if downloaded from internet

### "Error starting the application"
- Verify `Foscamun.db` is present
- Verify read/write permissions in folder

### Single File doesn't work
Remove `-p:PublishSingleFile=true` from publish command

## 📊 Expected Sizes

- **win-x64**: ~150 MB (single file) / ~180 MB (extracted)
- **win-x86**: ~130 MB (single file) / ~160 MB (extracted)
- **win-arm64**: ~140 MB (single file) / ~170 MB (extracted)

## 🆕 Future Updates

To release a new version:
1. Update `<Version>` in `Foscamun.csproj`
2. Run `.\publish.ps1 -Version "1.1.0"`
3. Create a new release on GitHub with the new tag

---

**Repository**: https://github.com/nanedb/Foscamun2026  
**License**: (Add your license)
