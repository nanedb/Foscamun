# 📦 Foscamun Build and Release Guide

## How a release is made

Releases are built by GitHub Actions (`.github/workflows/release.yml`) when a **version tag** (`v*`) is pushed:

1. it publishes self-contained single-file builds for **win-x64**, **win-x86** and **win-arm64**
2. it zips them as `Foscamun-{rid}-v{version}.zip` and writes `SHA256SUMS.txt`
3. it creates the GitHub release with those files and the text of `RELEASE_NOTES.md`

Tags with a hyphen (e.g. `v2.1.0-beta.1`) become **pre-releases**.

⚠️ **Keep the asset names as they are.** The app's updater (`Services/UpdateService.cs`) looks for `Foscamun-{rid}-{tag}.zip` and checks it against `SHA256SUMS.txt`. It offers releases whose tag is a plain version (`v2.0.1`), pre-releases included, and ignores drafts and tags with a hyphen.

## Releasing a new version

1. Update the version in `Foscamun.csproj` — all three:
   ```xml
   <Version>2.0.1</Version>
   <AssemblyVersion>2.0.1.0</AssemblyVersion>
   <FileVersion>2.0.1.0</FileVersion>
   ```
2. If the database schema changed: raise `DatabaseGuard.CurrentVersion` (`Data/DatabaseGuard.cs`) and teach `SqliteDataAccess` to bring older databases up to date (see [Database compatibility](#-database-compatibility))
3. Write the notes of the new version in `RELEASE_NOTES.md` (they become the release text)
4. Update the version history in `README.md`
5. Commit and push, then tag and push the tag:
   ```bash
   git tag -a v2.0.1 -m "Foscamun v2.0.1"
   git push origin v2.0.1
   ```
6. Follow the run in the repository's **Actions** tab; when it is green the release is online
7. Optional: mark it as pre-release or change which release is "Latest":
   ```bash
   gh release edit v2.0.1 --prerelease
   gh release edit v1.3.0 --latest
   ```

Running versions see the new release at their next start and offer to install it.

## Building locally

### All architectures (same packages as the workflow)

```powershell
.\publish.ps1 -Version "2.0.1"
```

The ZIPs and the checksums end up in `.\Publish\`. They can be uploaded by hand with `gh release upload v2.0.1 .\Publish\*.zip`.

### Windows x64 only

```cmd
publish-quick.bat
```

### Manual command

```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true
```

## 💾 Database compatibility

The user's database lives in `%LocalAppData%\Foscamun\Foscamun.db` and is never replaced by an installation; the `Foscamun.db` shipped next to the exe is only copied there on the first launch.

The schema version is stored in SQLite's `PRAGMA user_version`:

- at startup `DatabaseGuard.EnsureCompatible()` reads it before anything opens the database
- a database **older** than `DatabaseGuard.CurrentVersion` (0 = written before v2.0.0) is brought up to date by `SqliteDataAccess` (the `Ensure…` methods), then `DatabaseGuard.StampVersion()` writes the current version
- a database **newer** than the app, or one without the core tables (`Committees`, `Countries`, `CountryLists`, `ICJ`), shows a warning: the user can set it aside (renamed `Foscamun-backup-yyyyMMdd-HHmmss.db`, never deleted) and start with a new one, or exit without changes

Version history of the schema:

| Version | Since | Content |
|---|---|---|
| 0 | v1.0.0 – v1.3.0 | no version written |
| 1 | v2.0.0 | committees, countries, ICJ with any number of advocates, ICJ member warnings |

v1.3.0 and v2.0.0 use the same schema, so they can share the database.

## 🔄 How the update works

1. At startup the app reads `https://api.github.com/repos/nanedb/Foscamun/releases` (10-second timeout; no connection = no message)
2. It takes the highest version above its own that has a ZIP for its architecture
3. If the user accepts, the ZIP is downloaded to `%TEMP%\Foscamun-update\`, checked against `SHA256SUMS.txt` and extracted
4. A small script (`%TEMP%\Foscamun-update.cmd`) waits for the app to close, copies the new files over the app folder with `robocopy`, starts the new version and cleans up

If the app folder is not writable (e.g. under `C:\Program Files\`), the app offers to open the release page instead.

## 📄 Notes

### Files in each package
- `Foscamun.exe` — single-file executable with the .NET runtime
- `Foscamun.db` — starting database
- `Resources\CommitteeLogo\*.svg` — committee logos (copied to `%LocalAppData%`)
- `Resources\Sounds\*.wav` — timer sounds
- `README.txt`

### End-user requirements
- Windows 10 or later, x64, x86 or ARM64
- No administrator privileges, no .NET installation
- Extract to a user folder, so that updates can replace the files

## 🔧 Troubleshooting

### "Create Release" fails with 403
The workflow needs `permissions: contents: write` (already in `release.yml`). Also check **Settings → Actions → General → Workflow permissions** in the repository.

### Build fails
```powershell
dotnet clean
dotnet restore
dotnet build -c Release
```

### "Access denied" when running or updating
Move the folder out of `C:\Program Files\` or `C:\Windows\` to a user folder.

---

**Repository**: https://github.com/nanedb/Foscamun
**License**: MIT
