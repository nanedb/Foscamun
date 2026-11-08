; Foscamun installer (Inno Setup 6)
;
; Per-user installation, no administrator rights: the app goes to
; %LocalAppData%\Programs\Foscamun, so the app can update itself there.
; The user's data (%LocalAppData%\Foscamun: database, logos) is never touched,
; neither by installing nor by uninstalling.
;
; Build (from the repository root, after "dotnet publish" for one architecture):
;   ISCC.exe /DAppVersion=2.0.1 /DArch=win-x64 /DSourceDir=<publish folder> /DOutputDir=<folder> installer\Foscamun.iss
; Output: Foscamun-Setup-<Arch>-v<AppVersion>.exe
;
; The app's updater runs the installer with /VERYSILENT /SUPPRESSMSGBOXES /NORESTART /RELAUNCH
; once the app has closed; /RELAUNCH starts the new version at the end.

#ifndef AppVersion
  #error Pass the version: /DAppVersion=x.y.z
#endif
#ifndef Arch
  #define Arch "win-x64"
#endif
#ifndef SourceDir
  #error Pass the folder of the published app: /DSourceDir=...
#endif
#ifndef OutputDir
  #define OutputDir "."
#endif

[Setup]
; Never change the AppId: Windows uses it to recognise upgrades and the uninstaller
AppId={{6F1B3E2A-6C8D-4F4B-9E7A-2D5C1A8B9F30}
AppName=Foscamun
AppVersion={#AppVersion}
AppVerName=Foscamun {#AppVersion}
AppPublisher=Foscamun
AppPublisherURL=https://github.com/nanedb/Foscamun
AppSupportURL=https://github.com/nanedb/Foscamun/issues
AppUpdatesURL=https://github.com/nanedb/Foscamun/releases
VersionInfoVersion={#AppVersion}.0
VersionInfoProductName=Foscamun

PrivilegesRequired=lowest
DefaultDirName={autopf}\Foscamun
DefaultGroupName=Foscamun
DisableProgramGroupPage=yes
UsePreviousAppDir=yes

#if Arch == "win-x64"
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
#elif Arch == "win-arm64"
ArchitecturesAllowed=arm64
ArchitecturesInstallIn64BitMode=arm64
#endif

OutputDir={#OutputDir}
OutputBaseFilename=Foscamun-Setup-{#Arch}-v{#AppVersion}
SetupIconFile=..\Resources\Foscamun.ico
UninstallDisplayIcon={app}\Foscamun.exe
UninstallDisplayName=Foscamun
WizardStyle=modern
Compression=lzma2/max
SolidCompression=yes
CloseApplications=yes
RestartApplications=no

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"
Name: "it"; MessagesFile: "compiler:Languages\Italian.isl"
Name: "fr"; MessagesFile: "compiler:Languages\French.isl"
Name: "es"; MessagesFile: "compiler:Languages\Spanish.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "{#SourceDir}\Foscamun.exe"; DestDir: "{app}"; Flags: ignoreversion
; Starting database: the app copies it to %LocalAppData%\Foscamun only if there is none
Source: "{#SourceDir}\Foscamun.db"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#SourceDir}\Resources\*"; DestDir: "{app}\Resources"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\Foscamun"; Filename: "{app}\Foscamun.exe"
Name: "{autodesktop}\Foscamun"; Filename: "{app}\Foscamun.exe"; Tasks: desktopicon

[Run]
; Interactive installation: offer to start the app at the end
Filename: "{app}\Foscamun.exe"; Description: "{cm:LaunchProgram,Foscamun}"; Flags: nowait postinstall skipifsilent
; Update from the app (silent): start the new version
Filename: "{app}\Foscamun.exe"; Flags: nowait; Check: IsRelaunch

[Code]
function IsRelaunch: Boolean;
var
  I: Integer;
begin
  Result := False;
  for I := 1 to ParamCount do
    if CompareText(ParamStr(I), '/RELAUNCH') = 0 then
      Result := True;
end;
