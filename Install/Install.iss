; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyInstallation "01"

#define MyExeDir "..\Release\"
#define MyFilesDir "..\Debug\"
#define MyAppName "CHECK"
#define MyRegistryCheck "CHECK"
#define MyRegistrySimulator "PME\NexoSimulator"
#define MyAppName "CHECK"
#define MySoftwareVersion GetFileVersion(MyExeDir+"check.dll")
#define MyAppPublisher "Natixis Payment Solutions"
#define MyInstallDir "..\Install\"
#define MySourceDir "..\CHECK\"
#define MyAppPublisher "NPS/PME"
#define MySQLFilesDir "..\DATABASE_SHARED\"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{CF0C3878-19E7-4C83-8556-9F208E70491C}
AppName={#MyAppName}
;AppVersion={#MyBuildVersion}
;AppVerName={#MyAppName} v{#MySoftwareVersion}-build{#MyBuildVersion}
AppVerName={#MyAppName}
AppPublisher={#MyAppPublisher}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
DisableDirPage=yes
OutputDir=..\Install
OutputBaseFilename=Install{#MyAppName}-v{#MySoftwareVersion}-inst{#MyInstallation}

Compression=lzma
SolidCompression=yes

[Languages]
Name: "french"; MessagesFile: "compiler:Languages\French.isl"

[Dirs]
Name: "{userdocs}\{#MyAppName}"
Name: "{userdocs}\{#MyAppName}\TestCSharp"

[Files]
; program folder
Source: "{#MyExeDir}CHECK*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyExeDir}CHECK*.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyExeDir}PMS.CHPN*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyExeDir}PMS.CHPN*.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyExeDir}PMS.NEXO*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyExeDir}PMS.NEXO*.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyExeDir}PMS.COMMON*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyExeDir}PMS.COMMON*.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyExeDir}Newtonsoft.Json.*"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyExeDir}NexoSimulator.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyExeDir}TestCheck*.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyInstallDir}release notes.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyInstallDir}*.pdf"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyFilesDir}..\Certificat*.7z"; DestDir: "{app}"; Flags: ignoreversion
; programdata folder
Source: "{#MySQLFilesDir}*.sql"; DestDir: "{userdocs}\{#MyAppName}"; Flags: ignoreversion
Source: "{#MyFilesDir}Checks.accdb"; DestDir: "{userdocs}\{#MyAppName}"; Flags: ignoreversion
Source: "{#MyFilesDir}testcheck*.json"; DestDir: "{userdocs}\{#MyAppName}"; Flags: ignoreversion
Source: "{#MyFilesDir}guarantee.decisions.json"; DestDir: "{userdocs}\{#MyAppName}"; Flags: ignoreversion
Source: "{#MyExeDir}nexo.simulator*.json"; DestDir: "{userdocs}\{#MyAppName}"; Flags: ignoreversion
Source: "{#MySourceDir}TestCheck\*.*"; DestDir: "{userdocs}\{#MyAppName}\TestCheck"; Flags: ignoreversion

[Run]
;Filename: {dotnet40}\Regasm.exe;    Parameters: "NEXO.dll /codebase"; WorkingDir: "{app}"; Flags: runhidden runascurrentuser; StatusMsg: "Enregistrement du composant NEXO"
;Filename: {dotnet4064}\Regasm.exe;  Parameters: "NEXO.dll /codebase"; WorkingDir: "{app}"; Flags: runhidden runascurrentuser; StatusMsg: "Enregistrement du composant NEXO"; Check: IsWin64
;Filename: {dotnet40}\Regasm.exe;    Parameters: "CHPN.dll /codebase"; WorkingDir: "{app}"; Flags: runhidden runascurrentuser; StatusMsg: "Enregistrement du composant CHPN"
;Filename: {dotnet4064}\Regasm.exe;  Parameters: "CHPN.dll /codebase"; WorkingDir: "{app}"; Flags: runhidden runascurrentuser; StatusMsg: "Enregistrement du composant CHPN"; Check: IsWin64
;Filename: {dotnet40}\Regasm.exe;    Parameters: "CHECK.dll /codebase"; WorkingDir: "{app}"; Flags: runhidden runascurrentuser; StatusMsg: "Enregistrement du composant CHECK"
;Filename: {dotnet4064}\Regasm.exe;  Parameters: "CHECK.dll /codebase"; WorkingDir: "{app}"; Flags: runhidden runascurrentuser; StatusMsg: "Enregistrement du composant CHECK"; Check: IsWin64

[UninstallRun]
;Filename: {dotnet40}\Regasm.exe;    Parameters: "NEXO.dll /unregister"; WorkingDir: "{app}"; Flags: runhidden; StatusMsg: "Suppression du composant NEXO"
;Filename: {dotnet4064}\Regasm.exe;  Parameters: "NEXO.dll /unregister"; WorkingDir: "{app}"; Flags: runhidden; StatusMsg: "Suppression du composant NEXO"; Check: IsWin64
;Filename: {dotnet40}\Regasm.exe;    Parameters: "CHPN.dll /unregister"; WorkingDir: "{app}"; Flags: runhidden; StatusMsg: "Suppression du composant CHPN"
;Filename: {dotnet4064}\Regasm.exe;  Parameters: "CHPN.dll /unregister"; WorkingDir: "{app}"; Flags: runhidden; StatusMsg: "Suppression du composant CHPN"; Check: IsWin64
;Filename: {dotnet40}\Regasm.exe;    Parameters: "CHECK.dll /unregister"; WorkingDir: "{app}"; Flags: runhidden; StatusMsg: "Suppression du composant CHECK"
;Filename: {dotnet4064}\Regasm.exe;  Parameters: "CHECK.dll /unregister"; WorkingDir: "{app}"; Flags: runhidden; StatusMsg: "Suppression du composant CHECK"; Check: IsWin64

[Registry]
Root: HKCU; Subkey: "Software\{#MyRegistryCheck}\TestCSharp"; ValueName: "Settings"; ValueType: string; ValueData: "{userdocs}\{#MyAppName}\"; Flags: createvalueifdoesntexist uninsdeletekey
Root: HKCU; Subkey: "Software\{#MyRegistrySimulator}"; ValueName: "Settings"; ValueType: string; ValueData: "{userdocs}\{#MyAppName}\"; Flags: createvalueifdoesntexist uninsdeletekey
