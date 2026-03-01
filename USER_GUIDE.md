# 📖 Foscamun - Quick User Guide

## 🚀 First Launch

1. **Extract** all files from the ZIP archive to a user folder (e.g., Desktop, Documents)
   - ⚠️ Avoid protected folders like `C:\Program Files\`
2. **Run** `Foscamun.exe`
   - ✅ No administrator privileges required
   - ✅ No installation needed
3. On first launch, the database will be created automatically

## ⚙️ Initial Setup

### 1. Select Language
- Choose between English, French, or Spanish
- Language can be changed at any time

### 2. Set the Year
- Enter the year of the MUN conference

### 3. Add a Committee
1. Click **"Add Committee"**
2. Enter:
   - Committee name
   - President
   - Vice-president
   - Moderator
   - Topic
3. Click **"OK"**

### 4. Add Countries to Committee
1. Select the committee
2. Click **"Edit Committee"**
3. Add participating countries
4. Click **"Save"**

## 📋 Managing a Session

### Phase 1: Roll Call

1. **From the main menu**, click **"Roll Call"**
2. Select:
   - Topic (already set in the committee)
   - Session number
3. **Mark present delegates**:
   - Click on a country in the "Available" list
   - The country will move to the "Present" list
   - Or use **"Mark all present"**
4. Click **"Proceed"** to start the session

### Phase 2: Session

1. **Speakers List**:
   - Click on a country in "Available Speakers"
   - The country is added to the "Speakers List"
   - Order can be managed

2. **Current Speaker**:
   - The first in the list becomes the current speaker
   - Use the **Timer** to manage speaking time

3. **Speaker Management**:
   - **Remove**: Removes the speaker from the list
   - **Warn**: Adds a warning to the speaker
   - **Remove Warning**: Removes a warning

4. **When voting is needed**:
   - Click **"Voting"**

### Phase 3: Voting

1. **Before voting**:
   - All present countries are available to vote

2. **During voting**, for each country click:
   - **Approve** - Vote in favor
   - **Abstain** - Abstention
   - **Reject** - Vote against

3. **Results**:
   - Round results are shown
   - Click **"New Round"** for another voting round
   - Or **"Back"** to return to session

4. **Final Results**:
   - Shows the overall result of all rounds
   - Indicates if the motion is approved or rejected

## 🏛️ ICJ (International Court of Justice)

ICJ works similarly but with some differences:

### ICJ Configuration
1. Click **"Edit ICJ"** from configuration
2. Enter:
   - Main judge
   - Vice-judge 1
   - Vice-judge 2
   - Case topic
3. Add:
   - Advocates
   - Jurors

### ICJ Session
- Roll call includes advocates and jurors
- Session works like regular committees
- Voting system is the same

## ⌨️ Keyboard Shortcuts

- **Home** → Return to main page
- **Esc** → Go back (where available)

## 💾 Data and Backup

### Where is data saved?
- Database: `Foscamun.db` (in application folder)
- Settings: Saved automatically

### How to backup?
1. Close the application
2. Copy the file `Foscamun.db`
3. Store the copy in a safe place

### How to restore a backup?
1. Close the application
2. Replace `Foscamun.db` with the backup copy
3. Restart the application

## 🎨 Customization

### Committee Logos
Logos are located in: `Resources\CommitteeLogo\`

**To add a custom logo:**
1. Create an SVG file with the committee name (e.g., `ECOSOC.svg`)
2. Place it in `Resources\CommitteeLogo\`
3. Restart the application

**Generic logo:** If there's no specific logo, `Generic.svg` is used

### Sounds
Audio files are in: `Resources\Sounds\`
- Supported format: WAV
- Used for notifications and timer

## ❓ Common Issues

### Application won't start
- Verify you have Windows 10 or higher
- Try running as administrator (right-click → "Run as administrator")

### Database not found
- Verify `Foscamun.db` is in the same folder as `Foscamun.exe`
- If missing, the application will recreate it automatically (empty)

### Logos not showing
- Verify the folder `Resources\CommitteeLogo\` exists
- Verify SVG files are valid
- Use `Generic.svg` as fallback

### Language doesn't change
- Restart the application after changing language

## 📧 Support

- **Bugs or issues?** Open an issue at: https://github.com/nanedb/Foscamun2026/issues
- **Questions?** Use discussions: https://github.com/nanedb/Foscamun2026/discussions

## 📚 Additional Resources

- **Complete documentation**: [README.md](README.md)
- **Deployment guide**: [DEPLOYMENT.md](DEPLOYMENT.md)
- **Release notes**: [RELEASE_NOTES_TEMPLATE.md](RELEASE_NOTES_TEMPLATE.md)

---

**Version**: 1.0.0  
**Last updated**: 2025  
**Repository**: https://github.com/nanedb/Foscamun2026
