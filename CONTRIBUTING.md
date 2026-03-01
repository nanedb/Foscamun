# Contributing to Foscamun

First off, thank you for considering contributing to Foscamun! It's people like you that make Foscamun such a great tool for the MUN community.

## 📋 Table of Contents

- [Code of Conduct](#code-of-conduct)
- [How Can I Contribute?](#how-can-i-contribute)
- [Development Setup](#development-setup)
- [Pull Request Process](#pull-request-process)
- [Coding Standards](#coding-standards)
- [Commit Message Guidelines](#commit-message-guidelines)

---

## 📜 Code of Conduct

This project adheres to a code of conduct. By participating, you are expected to uphold this code.

- Be respectful and inclusive
- Welcome newcomers
- Focus on what is best for the community
- Show empathy towards other community members

---

## 🤔 How Can I Contribute?

### Reporting Bugs

Before creating bug reports, please check existing issues to avoid duplicates. When you create a bug report, include as many details as possible:

- **Clear title** and description
- **Steps to reproduce** the issue
- **Expected behavior** vs actual behavior
- **Screenshots** if applicable
- **Environment details** (Windows version, .NET version, etc.)

### Suggesting Enhancements

Enhancement suggestions are tracked as GitHub issues. When creating an enhancement suggestion:

- **Use a clear title** describing the enhancement
- **Provide detailed description** of the suggested enhancement
- **Explain why** this enhancement would be useful
- **Include mockups** or examples if applicable

### Pull Requests

- Fill in the required template
- Follow the coding standards
- Include appropriate tests
- Update documentation as needed
- End all files with a newline

---

## 🛠️ Development Setup

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Visual Studio 2025+](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- Git

### Clone and Build

```bash
# Clone the repository
git clone https://github.com/nanedb/Foscamun2026.git
cd Foscamun2026

# Restore dependencies
dotnet restore

# Build
dotnet build

# Run
dotnet run
```

### Project Structure

- `Data/` - Database access layer
- `Models/` - Data models
- `ViewModels/` - MVVM ViewModels
- `Views/` - WPF Pages and Windows
- `Repositories/` - Data repositories
- `Helpers/` - Utility classes
- `Resources/` - Images, icons, sounds
- `Strings/` - Language resources
- `Styles/` - WPF themes and styles

---

## 🔄 Pull Request Process

1. **Fork** the repository
2. **Create** a feature branch from `master`
3. **Make** your changes
4. **Test** thoroughly
5. **Commit** with clear messages
6. **Push** to your fork
7. **Submit** a Pull Request

### PR Checklist

- [ ] Code builds without errors
- [ ] Code follows project conventions
- [ ] Comments added for complex logic
- [ ] Documentation updated
- [ ] No unnecessary files included
- [ ] Tested manually

---

## 💻 Coding Standards

### C# Style

- Use **C# 14** features where appropriate
- Follow [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use **nullable reference types** (`#nullable enable`)
- Use **implicit usings** where available

### XAML Style

- Use clear, descriptive names for controls
- Follow existing indentation (4 spaces)
- Use `DynamicResource` for theme-based resources
- Keep XAML clean and readable

### MVVM Pattern

- ViewModels should not reference Views directly
- Use `IRelayCommand` from CommunityToolkit.Mvvm
- Use `[ObservableProperty]` attributes
- Keep business logic in ViewModels

### Database

- Use async methods for database operations
- Use parameterized queries (Dapper handles this)
- Handle errors gracefully
- Log errors using Debug.WriteLine

### Naming Conventions

- **Classes**: PascalCase (`CommitteeViewModel`)
- **Methods**: PascalCase (`LoadCountriesAsync`)
- **Properties**: PascalCase (`SelectedCommittee`)
- **Fields**: _camelCase (`_database`)
- **Constants**: PascalCase or UPPER_CASE

---

## 📝 Commit Message Guidelines

Use clear and descriptive commit messages:

```
<type>: <short summary>

<optional detailed description>
```

### Types

- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, etc.)
- `refactor`: Code refactoring
- `test`: Adding tests
- `chore`: Maintenance tasks

### Examples

```
feat: Add ICJ support for judges and advocates

Implemented dedicated ICJ management with judge assignments
and specialized voting procedures.
```

```
fix: Correct Back navigation from SessionPage to RollCallPage

Previously, clicking Back in SessionPage could navigate to
wrong page depending on navigation stack. Now it explicitly
returns to the appropriate RollCallPage.
```

---

## 🧪 Testing

Before submitting a PR:

- [ ] Build succeeds without errors
- [ ] Application runs without crashes
- [ ] Test affected features manually
- [ ] Test language switching
- [ ] Verify database operations work
- [ ] Check no regressions in existing features

---

## 🌍 Adding Translations

To add a new language:

1. Create `Strings/Strings.{langCode}.xaml` (e.g., `Strings.de.xaml` for German)
2. Copy structure from `Strings.en.xaml`
3. Translate all strings
4. Add language option to `Views/SetupPage.xaml`
5. Test language switching

---

## 📖 Documentation

When adding features, please update:

- Code comments for complex logic
- XML documentation for public APIs
- User-facing documentation in `USER_GUIDE.md`
- Release notes in `RELEASE_NOTES_TEMPLATE.md`

---

## 🎯 Areas for Contribution

Looking for ideas? Here are some areas where contributions would be especially valuable:

- 🌐 Additional language translations
- 📊 Export results to PDF/Excel
- 🎨 Additional themes and styles
- 🔧 Performance optimizations
- 📱 Potential future cross-platform support
- 🧪 Unit and integration tests
- 📖 Documentation improvements
- 🐛 Bug fixes

---

## ❓ Questions?

Feel free to open a [Discussion](https://github.com/nanedb/Foscamun2026/discussions) if you have questions about contributing!

---

## 🙏 Thank You!

Your contributions help make Foscamun better for everyone in the MUN community. Thank you for taking the time to contribute!

---

**Happy Coding!** 🚀
