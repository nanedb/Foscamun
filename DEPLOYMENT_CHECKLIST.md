# ✅ Foscamun Release Checklist

See [DEPLOYMENT.md](DEPLOYMENT.md) for the details of each step.

## Before the tag

### Version
- [ ] `<Version>`, `<AssemblyVersion>`, `<FileVersion>` updated in `Foscamun.csproj`
- [ ] If the database schema changed: `DatabaseGuard.CurrentVersion` raised and the upgrade written in `SqliteDataAccess`

### Testing
- [ ] `dotnet build` without errors
- [ ] Committee: Setup → Roll call → Session (timer, warnings) → Voting → Result → Final result
- [ ] ICJ: the same flow
- [ ] Voting rules for rounds 1, 2 and 3 (committees and ICJ), including a tie in round 3
- [ ] Language change (EN, FR, ES): new strings present in all three dictionaries
- [ ] Start with an existing database from the previous version: data kept
- [ ] Start without a database: created from the seed
- [ ] Custom committee logos load

### Documentation
- [ ] `RELEASE_NOTES.md` written for this version (it becomes the release text)
- [ ] Version history in `README.md`
- [ ] `USER_GUIDE.md` updated if the behaviour changed
- [ ] Screenshots in `docs/screenshots/` updated if the interface changed

## Release

- [ ] Changes committed and pushed
- [ ] Tag created and pushed:
  ```bash
  git tag -a vX.Y.Z -m "Foscamun vX.Y.Z"
  git push origin vX.Y.Z
  ```
- [ ] Workflow "Build and Release" green in the Actions tab
- [ ] Release contains, for win-x64, win-x86 and win-arm64, `Foscamun-Setup-{rid}-vX.Y.Z.exe` and `Foscamun-{rid}-vX.Y.Z.zip`, plus `SHA256SUMS.txt`
- [ ] Pre-release / Latest set as wanted (`gh release edit …`)

## After the release

- [ ] Download the x64 installer as an anonymous user, install, run; uninstall from Settings → Apps leaves `%LocalAppData%\Foscamun` in place
- [ ] Download the x64 ZIP, extract, run
- [ ] Checksum matches:
  ```powershell
  Get-FileHash -Path "Foscamun-win-x64-vX.Y.Z.zip" -Algorithm SHA256
  ```
- [ ] An installed previous version and a portable one both offer the update at startup and install it

---

**Version**: ________
**Date**: ________
