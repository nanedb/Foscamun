# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Foscamun is a WPF desktop application (.NET 10, Windows-only) for managing Model United Nations (MUN) conferences. It handles committee sessions, roll calls, speaker queues, voting, and ICJ (International Court of Justice) simulations with multi-language support (EN/FR/ES).

## Build & Run Commands

```powershell
# Restore dependencies
dotnet restore

# Build (debug)
dotnet build

# Run
dotnet run

# Build release
dotnet build -c Release

# Publish self-contained single-file executable (x64)
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

# All platforms at once (uses publish.ps1)
.\publish.ps1 -Version "2.0.0"
```

There are no automated tests (`dotnet test` will find nothing).

## Architecture

**Pattern**: MVVM using `CommunityToolkit.Mvvm` (`[ObservableProperty]`, `[RelayCommand]` source generators).

**Navigation model**: `MainWindow` is a shell with a collapsible sidebar and a single `Frame` (`RightFrame`) for the right-side content. Navigation happens by calling `MainWindow.Instance.NavigateRightFrame(page)` from ViewModels. The sidebar buttons (Home, Roll Call, Configure) are wired in `MainWindow.xaml.cs` code-behind.

**Data layer**:
- `SqliteDataAccess` is the primary entry point for DB access. It owns three repository instances: `CommitteeRepository`, `CountryRepository`, and `ICJRepository`.
- The DB file lives at `%LocalAppData%\Foscamun\Foscamun.db`. On first run it is copied from the executable directory.
- `CommitteeRepository` and `CountryRepository` use raw ADO.NET; `ICJRepository` also uses raw ADO.NET with JSON serialization for the jurors list (`JurorsJson` column).
- ICJ is treated as a special committee with `CommID = -1` in the `CountryLists` table; the actual ICJ data lives in a separate `ICJ` table.

**DI**: `Microsoft.Extensions.DependencyInjection` is used. Only `SqliteDataAccess` and `CommitteeListViewModel` are registered as singletons in `App.OnStartup`. Most ViewModels are instantiated directly (not from DI).

**Language switching**: `App.ChangeLanguage(langCode)` swaps the active `ResourceDictionary` from `Strings/Strings.{lang}.xaml` at runtime without restarting. It fires `App.LanguageChanged` (a static event) so ViewModels can refresh bound collections.

**Styles & theming**: All WPF styles are split across `Styles/` (Colors, Typography, Buttons, Inputs, Shapes, Icons, Theme). SVG icons/flags are rendered via SharpVectors.

**Committee logos**: SVG files in `%LocalAppData%\Foscamun\CommitteeLogo\`. `Generic.svg` is the fallback. On first run, logos are copied from `Resources/CommitteeLogo/` in the exe directory via `LogoInstaller`.

**DB compatibility**: `Data/DatabaseGuard.cs` checks the existing DB at startup (before `SqliteDataAccess`) via `PRAGMA user_version` against `DatabaseGuard.CurrentVersion`; newer or unreadable DBs trigger a warning (rename aside / exit). After `SqliteDataAccess` runs its `Ensure…` upgrades, `StampVersion()` writes the current version. Raise `CurrentVersion` whenever the schema changes.

**Updates & releases**: `Services/UpdateService.cs` checks GitHub releases at startup and self-installs (ZIP per architecture, verified with `SHA256SUMS.txt`, replaced by a temp .cmd after the app exits). Releases are built by `.github/workflows/release.yml` on pushing a `v*` tag, with `RELEASE_NOTES.md` as the body; keep the asset names `Foscamun-{rid}-v{version}.zip`. Bump `<Version>`, `<AssemblyVersion>`, `<FileVersion>` in the csproj for each release.

**Settings persistence**: `Properties.Settings.Default` (auto-generated `Settings.Designer.cs`) stores language, year, last selected committee name, ConfirmExit and SkippedUpdate.

## Key Conventions

- ViewModels inherit `ObservableObject` and use `[ObservableProperty]` / `[RelayCommand]` source generators — avoid manual `INotifyPropertyChanged` boilerplate.
- Async DB methods follow the `*Async` naming convention and use raw `SqliteCommand`/`SqliteConnection` (not Dapper, despite Dapper being a dependency).
- Language-dependent display strings come from `DynamicResource` bindings to the string dictionaries, not hardcoded text. Add new strings to all three `.xaml` files.
- Country names are localized: `Country.Name` returns the name in the active language; use `IsoCode` as the stable identifier.
