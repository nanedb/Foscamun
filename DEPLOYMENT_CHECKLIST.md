# ✅ Foscamun Deployment Checklist

## Pre-Deployment

### Code
- [ ] All changes are committed
- [ ] Local build without errors (`dotnet build`)
- [ ] No critical warnings
- [ ] All TODOs resolved or documented
- [ ] Code reviewed and clean

### Testing
- [ ] Complete manual application testing
- [ ] Test committee creation
- [ ] Test roll call
- [ ] Test session and speakers list
- [ ] Test single and multiple voting
- [ ] Test complete ICJ
- [ ] Test language change (EN, FR, ES)
- [ ] Test database (create, read, write)
- [ ] Test custom logos

### Documentation
- [ ] README.md updated
- [ ] DEPLOYMENT.md created and updated
- [ ] USER_GUIDE.md complete
- [ ] RELEASE_NOTES updated with new features
- [ ] Code comments verified
- [ ] Version number updated in `Foscamun.csproj`

### Resources
- [ ] All logos are in `Resources\CommitteeLogo\`
- [ ] Valid SVG logos
- [ ] Audio files in `Resources\Sounds\`
- [ ] Icons in `Resources\Icons\`
- [ ] Fonts in `Fonts\`
- [ ] Database template `Foscamun.db` working

## Local Build

### Automated Script
- [ ] Executed `.\publish.ps1 -Version "1.0.0"`
- [ ] Verified output in `.\Publish\`
- [ ] Test Windows x64 executable
- [ ] Test Windows x86 executable (if available)
- [ ] Verified files included in each archive
- [ ] SHA256 checksums calculated

### Executable Tests
- [ ] `Foscamun.exe` starts correctly
- [ ] Database created on first launch
- [ ] All features operational
- [ ] No errors in console
- [ ] Logos load correctly
- [ ] Language change working

## Git and GitHub

### Repository
- [ ] `.gitignore` updated
- [ ] No sensitive files committed
- [ ] All changes pushed
```bash
git status  # Verify everything is clean
```

### Version Tag
- [ ] Tag created for version
```bash
git tag v1.0.0
git push origin v1.0.0
```

### GitHub Actions (Optional)
- [ ] Workflow `.github/workflows/build-release.yml` committed
- [ ] Workflow tested with tag push
- [ ] Build on GitHub Actions completed successfully

## GitHub Release

### Preparation
- [ ] Go to: https://github.com/nanedb/Foscamun2026/releases
- [ ] Click "Create a new release"
- [ ] Select tag `v1.0.0`

### Release Information
- [ ] **Tag version**: v1.0.0
- [ ] **Release title**: Foscamun v1.0.0
- [ ] **Description**: Copied from RELEASE_NOTES_TEMPLATE.md
- [ ] Updated release date
- [ ] Added version-specific notes

### Files to Upload
- [ ] `Foscamun-win-x64-v1.0.0.zip`
- [ ] `Foscamun-win-x86-v1.0.0.zip`
- [ ] `Foscamun-win-arm64-v1.0.0.zip`
- [ ] `SHA256SUMS.txt`

### Final Checks
- [ ] SHA256 checksums added to release notes
- [ ] Download links tested
- [ ] Release README verified
- [ ] Release published (not as draft)

## Post-Deployment

### Download Verification
- [ ] Download from GitHub completed successfully
- [ ] ZIP archives extract correctly
- [ ] Checksum verified:
```powershell
Get-FileHash -Path "Foscamun-win-x64-v1.0.0.zip" -Algorithm SHA256
```

### Public Download Test
- [ ] Download as anonymous user
- [ ] Extract files
- [ ] Run application
- [ ] Verify everything works

### Communication
- [ ] Announcement published (if applicable)
- [ ] Release link shared
- [ ] Support documentation available

## Troubleshooting

### Build Fails
```powershell
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build -c Release
```

### Missing Files in Output
Verify in `Foscamun.csproj`:
- [ ] `<Resource>` tags for SVG, PNG, ICO
- [ ] `<None Include>` with `<CopyToOutputDirectory>` for WAV and database
- [ ] `<Content Include>` for database

### File Size Too Large
Options to reduce:
- [ ] Remove `-p:PublishReadyToRun=true`
- [ ] Add `-p:PublishTrimmed=true` (with caution!)
- [ ] Use framework-dependent instead of self-contained

### GitHub Actions Fails
- [ ] Verify workflow file is valid (YAML syntax)
- [ ] Check logs on GitHub Actions
- [ ] Verify `GITHUB_TOKEN` is available
- [ ] Make sure tag follows pattern `v*.*.*`

## Final Notes

### Future Versions
To release v1.1.0:
1. Update `<Version>` in `Foscamun.csproj`
2. Update RELEASE_NOTES
3. Commit and push
4. Create tag `v1.1.0`
5. Run `.\publish.ps1 -Version "1.1.0"`
6. Create release on GitHub

### Backup
- [ ] Backup of local repository
- [ ] Backup of production database
- [ ] Backup of custom resources

---

**Completion Date**: ___________  
**Version**: v1.0.0  
**Release Manager**: ___________
