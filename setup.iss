[Setup]
AppName=PoliceReport
AppVersion=1.0
DefaultDirName={pf}\PoliceReport
DefaultGroupName=PoliceReport
OutputDir=output
OutputBaseFilename=setup
Compression=lzma
SolidCompression=yes
SetupIconFile="output\icon.ico"

[Files]
; Inclure tous les fichiers et sous-répertoires du répertoire 'output'
Source: "output\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; Inclure l'icône dans l'application
Source: "output\icon.ico"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\PoliceReport"; Filename: "{app}\PoliceReport.exe"; IconFilename: "{app}\icon.ico"
Name: "{group}\Uninstall PoliceReport"; Filename: "{uninstallexe}"

[Run]
; Lancer l'application après l'installation (optionnel)
Filename: "{app}\PoliceReport.exe"; Description: "Lancer PoliceReport"; Flags: nowait postinstall skipifsilent
