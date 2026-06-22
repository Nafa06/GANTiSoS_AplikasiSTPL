#define MyAppName "STPLapp"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "GANTiSoS"
#define MyAppExeName "STPLapp.exe"
[Setup]
AppId={{D1C3E4F5-6A7B-8C9D-0E1F-2A3B4C5D6E7F}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DisableProgramGroupPage=yes
OutputDir=.\Output
OutputBaseFilename=STPLapp_Setup
Compression=lzma
SolidCompression=yes
WizardStyle=modern
[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
[Files]
Source: ".\STPLapp\STPLapp\bin\Release\STPLapp.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Naufal\Kuliah\Semester 4\PABD\Pertemuan 7 UCP\UCP\GANTiSoS_AplikasiSTPL\STPLapp\STPLapp\bin\Release*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Excludes: "STPLapp.exe, *.pdb, *.xml"
[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent