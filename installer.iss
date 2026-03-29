[Setup]
AppName=DesktopKit Diffchecker
AppVersion=1.0
AppPublisher=DesktopKit Project
DefaultDirName={autopf}\DesktopKit\Diffchecker
DefaultGroupName=DesktopKit Diffchecker
UninstallDisplayIcon={app}\DesktopKit.Diffchecker.exe
OutputDir=.
OutputBaseFilename=DiffcheckerSetup
SetupIconFile=_works\Diffchecker.ico
Compression=lzma2
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64compatible

[Files]
Source: "publish\DesktopKit.Diffchecker.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\D3DCompiler_47_cor3.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\PenImc_cor3.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\PresentationNative_cor3.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\vcruntime140_cor3.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "publish\wpfgfx_cor3.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "ReadMe.txt"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\DesktopKit Diffchecker"; Filename: "{app}\DesktopKit.Diffchecker.exe"; IconFilename: "{app}\DesktopKit.Diffchecker.exe"
Name: "{group}\Uninstall DesktopKit Diffchecker"; Filename: "{uninstallexe}"

[Run]
Filename: "{app}\DesktopKit.Diffchecker.exe"; Description: "DesktopKit Diffchecker を起動"; Flags: nowait postinstall skipifsilent
