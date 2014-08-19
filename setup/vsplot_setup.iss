; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "VSPlot"
#define MyAppVersion "1.0"
#define MyAppPublisher "My Company, Inc."
#define MyAppURL "http://www.example.com/"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{65B2A03F-9D1A-4E01-8FAF-DD4B17DF300A}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
OutputBaseFilename=vsplot_v1
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "src\vsplot.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "src\README.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "src\pge.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "src\data_types.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "src\updateenv.bat"; DestDir: "{app}"; Flags: ignoreversion
Source: "src\demo\*"; DestDir: "{app}\demo";Flags:ignoreversion recursesubdirs

; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{group}\README"; Filename: "{app}\README.txt"
Name: "{group}\demo"; Filename: "{app}\demo\plot_plugin_client.sln"

[Code]
var
page:TInputOptionWizardPage;
versions:array[0..4] of Integer;
count:Integer;
i:Integer;
const
select_secondary = 'Installed Visual Studio versions:';
select_caption = 'Add plugin to the following versions';
registry_path = 'Software\MICROSOFT\VisualStudio\';
studio_version_2005 = '8.0\';
studio_version_2008 = '9.0\';
studio_version_2010 = '10.0\';
studio_version_2012 = '11.0\';
studio_version_2013 = '12.0\';

plugin_guid = '{8672e1d4-4dcd-4935-b491-478fe1a95b5c}';
tool_guid = '{becf3777-4750-4ab5-b11f-8a18fa7b27f5}';
plugin_autoload = '{f1536ef8-92ec-443c-9ed7-fdadf150da82}';
plugin_name = 'vsplot';

///////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////CHECKING VERSIONS/////////////////////////////////////////
function check_version():Boolean;
begin
versions[0]:= 0;versions[1]:= 0;versions[2]:= 0;versions[3]:= 0;count:=0; versions[4]:=0;
if RegKeyExists(HKEY_CLASSES_ROOT,'VisualStudio.DTE.8.0') then versions[count]:=1;count:=count+1;begin page.Add('2005'); end;
if RegKeyExists(HKEY_CLASSES_ROOT,'VisualStudio.DTE.9.0') then begin versions[count]:=2;count:=count+1;page.Add('2008'); end;
if RegKeyExists(HKEY_CLASSES_ROOT,'VisualStudio.DTE.10.0') then begin versions[count]:=3;count:=count+1;page.Add('2010');end;
if RegKeyExists(HKEY_CLASSES_ROOT,'VisualStudio.DTE.11.0') then begin versions[count]:=4;count:=count+1;page.Add('2012');end;
if RegKeyExists(HKEY_CLASSES_ROOT,'VisualStudio.DTE.12.0') then begin versions[count]:=5;count:=count+1;page.Add('2013');end;
Result := True;
end;


////////////////////////////////////////////////////////////////////////////////////////
procedure Register(version:String;studio_dir_name:String);
var 
ResultCode: Integer;
begin
////////////////////////////////////////////////////Installed products/////////////////////////////////////
if(RegWriteStringValue(HKEY_LOCAL_MACHINE,registry_path+version+'InstalledProducts\'+plugin_name,'','#110') = False) then begin
MsgBox('Failed to add value InprocServer32',mbInformation,MB_OK);end; 

if(RegWriteStringValue(HKEY_LOCAL_MACHINE,registry_path+version+'InstalledProducts\'+plugin_name,'Package',plugin_guid) = False) then begin
MsgBox('Failed to add value InprocServer32',mbInformation,MB_OK);end;  

if(RegWriteStringValue(HKEY_LOCAL_MACHINE,registry_path+version+'InstalledProducts\'+plugin_name,'ProductDetails','#112') = False) then begin
MsgBox('Failed to add value Class',mbInformation,MB_OK);end;

if(RegWriteStringValue(HKEY_LOCAL_MACHINE,registry_path+version+'InstalledProducts\'+plugin_name,'PID','1.0') = False) then begin
MsgBox('Failed to add value Class',mbInformation,MB_OK);end;

if(RegWriteStringValue(HKEY_LOCAL_MACHINE,registry_path+version+'InstalledProducts\'+plugin_name,'LogoID','#400') = False) then begin
MsgBox('Failed to add value CodeBase',mbInformation,MB_OK);end;

//////////////////////////////////////////////////Packages/////////////////////////////////////////////
if(RegWriteStringValue(HKEY_LOCAL_MACHINE,registry_path+version+'Packages\'+plugin_guid,'','bukachacha.vsplot.vsplot, vsplot, Version=1.0.5003.23643, Culture=neutral, PublicKeyToken=e4b31acc0cb316c9') = False) then begin
MsgBox('Failed to add value InprocServer32',mbInformation,MB_OK);end; 

if(RegWriteStringValue(HKEY_LOCAL_MACHINE,registry_path+version+'Packages\'+plugin_guid,'InprocServer32','C:\\WINDOWS\\system32\\mscoree.dll') = False) then begin
MsgBox('Failed to add value InprocServer32',mbInformation,MB_OK);end;  

if(RegWriteStringValue(HKEY_LOCAL_MACHINE,registry_path+version+'Packages\'+plugin_guid,'Class','bukachacha.vsplot.vsplot') = False) then begin
MsgBox('Failed to add value Class',mbInformation,MB_OK);end;

if(RegWriteStringValue(HKEY_LOCAL_MACHINE,registry_path+version+'Packages\'+plugin_guid,'CodeBase','C:\\Program Files\\VSPlot\\vsplot.dll') = False) then begin
MsgBox('Failed to add value Class',mbInformation,MB_OK);end;

if(RegWriteDWordValue(HKEY_LOCAL_MACHINE,registry_path+version+'Packages\'+plugin_guid,'ID',104) = False) then begin
MsgBox('Failed to add value ID',mbInformation,MB_OK);end;

if(RegWriteStringValue(HKEY_LOCAL_MACHINE,registry_path+version+'Packages\'+plugin_guid,'MinEdition','Standard') = False) then begin
MsgBox('Failed to add value CodeBase',mbInformation,MB_OK);end;

if(RegWriteStringValue(HKEY_LOCAL_MACHINE,registry_path+version+'Packages\'+plugin_guid,'ProductVersion','1.0') = False) then begin
MsgBox('Failed to add value CodeBase',mbInformation,MB_OK);end;

if(RegWriteStringValue(HKEY_LOCAL_MACHINE,registry_path+version+'Packages\'+plugin_guid,'ProductName',plugin_name) = False) then begin
MsgBox('Failed to add value CodeBase',mbInformation,MB_OK);end;

if(RegWriteStringValue(HKEY_LOCAL_MACHINE,registry_path+version+'Packages\'+plugin_guid,'CompanyName','private') = False) then begin
MsgBox('Failed to add value CodeBase',mbInformation,MB_OK);end;

////////////////////////////////////////////////////Tool Window//////////////////////////////////////
if(RegWriteStringValue(HKEY_LOCAL_MACHINE,registry_path+version+'ToolWindows\'+tool_guid,'',plugin_guid) = False) then begin
MsgBox('Failed to add value CodeBase',mbInformation,MB_OK);end;

if(RegWriteStringValue(HKEY_LOCAL_MACHINE,registry_path+version+'ToolWindows\'+tool_guid,'Name','bukachacha.vsplot.MyToolWindow') = False) then begin
MsgBox('Failed to add value CodeBase',mbInformation,MB_OK);end;

/////////////////////////////////////////////Menus///////////////////////////////////////
if(RegWriteStringValue(HKEY_LOCAL_MACHINE,registry_path+version+'Menus',plugin_guid,', 1000, 1') = False) then begin
MsgBox('Failed to add value CodeBase',mbInformation,MB_OK);end;


//running devenv /setup to update environment

Exec('C:\Program Files\VSplot\updateenv.bat',studio_dir_name,'',SW_SHOW,ewWaitUntilTerminated,ResultCode);

end;


procedure Unregister(version:String);
begin
if(RegDeleteKeyIncludingSubkeys(HKEY_LOCAL_MACHINE,registry_path+version+'InstalledProducts\'+plugin_name) = False) then begin
MsgBox('failed to remove key:' +registry_path+version+'InstalledProducts\'+plugin_name,mbInformation,MB_OK);end;

if(RegDeleteKeyIncludingSubkeys(HKEY_LOCAL_MACHINE,registry_path+version+'Packages\'+plugin_guid) = False) then begin
MsgBox('failed to remove key:' +registry_path+version+'Packages\'+plugin_guid,mbInformation,MB_OK);end;

if(RegDeleteKeyIncludingSubkeys(HKEY_LOCAL_MACHINE,registry_path+version+'ToolWindows\'+tool_guid) = False) then begin
MsgBox('failed to remove key:' +registry_path+version+'ToolWindows\'+tool_guid,mbInformation,MB_OK);end;

if(RegDeleteValue(HKEY_LOCAL_MACHINE,registry_path+version+'Menus',plugin_guid) = False) then begin
MsgBox('failed to remove key:' +registry_path+version+'ToolWindows\'+tool_guid,mbInformation,MB_OK);end;
end;

///////////////////////////////////////////////////////////////////////////////////////
function RegisterPlugin:Boolean;
begin
for i:=0 to (count-1) do
begin
if(page.Values[i] = True) then 
begin 
  if(versions[i] = 1)then begin MsgBox('2005',mbInformation,MB_OK);Register(studio_version_2005,'"Microsoft Visual Studio 8"');end;
  if(versions[i] = 2)then begin MsgBox('2008',mbInformation,MB_OK);Register(studio_version_2008,'"Microsoft Visual Studio 9.0"');end;
  if(versions[i] = 3)then begin MsgBox('2010',mbInformation,MB_OK);Register(studio_version_2010,'"Microsoft Visual Studio 10.0"');end;
  if(versions[i] = 4)then begin MsgBox('2012',mbInformation,MB_OK);Register(studio_version_2012,'"Microsoft Visual Studio 11"');end;
  if(versions[i] = 5)then begin MsgBox('2013',mbInformation,MB_OK);Register(studio_version_2013,'"Microsoft Visual Studio 12"');end;
end;
end;
end;
///////////////////////////////////////////////////////////////////////////////////////////
procedure InitializeWizard;
begin
  page:=CreateInputOptionPage(wpWelcome,select_caption,'',select_secondary,False,False);
  check_version();
end;
//////////////////////////////////////////////////////////////////////////////////////////
procedure CurStepChanged(CurStep:TSetupStep);
begin
if (CurStep = ssPostInstall) then begin
RegisterPlugin();
end; 
end;


procedure CurUninstallStepChanged(CurStep:TUninstallStep);
begin
if(CurStep = usPostUninstall) then begin
Unregister(studio_version_2005);
Unregister(studio_version_2008);
Unregister(studio_version_2010);
Unregister(studio_version_2012);
Unregister(studio_version_2013);
end;
end;