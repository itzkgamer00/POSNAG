; =====================================================================
;  Instalador de Sistema de Caja (Inno Setup 6)
;
;  Un solo instalador para los dos equipos de la agencia:
;    - Equipo principal (Administrador): tiene SQL Server Express. El
;      instalador crea/actualiza la base SistemaCaja, el usuario SQL de la
;      aplicacion y habilita el acceso por red para la caja secundaria.
;    - Caja secundaria: solo instala la aplicacion y la apunta al SQL
;      Server del equipo principal.
;
;  Compilar con instalador\compilar_instalador.ps1 (compila Release y
;  luego este script). El Setup.exe queda en instalador\Output.
; =====================================================================

#define MyAppName "Sistema de Caja"
#define MyAppExe "CapaPresentacion.exe"
#define MyAppVersion "1.0.0"
#define ReleaseDir "..\CapaPresentacion\bin\Release"

[Setup]
AppId={{8F3C2A51-6B7D-4E2A-9C1F-3D5B7A9E4C21}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
DefaultDirName={autopf}\SistemaCaja
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputDir=Output
OutputBaseFilename=Instalador_SistemaCaja_{#MyAppVersion}
SetupIconFile=..\CapaPresentacion\Resources\iconoprinc.ico
UninstallDisplayIcon={app}\{#MyAppExe}
Compression=lzma2
SolidCompression=yes
PrivilegesRequired=admin
WizardStyle=modern
ArchitecturesInstallIn64BitMode=x64compatible

[Languages]
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"

[Files]
; connections.config se genera en cada equipo (ver EscribirConnectionsConfig); nunca se copia el del desarrollador.
Source: "{#ReleaseDir}\*"; DestDir: "{app}"; Excludes: "*.pdb,*.xml,connections.config,connections.config.example"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "..\sql\script_completo_base_datos.sql"; DestDir: "{app}\sql"; Check: EsPrincipal; Flags: ignoreversion
Source: "crear_usuario_app.sql"; DestDir: "{app}\sql"; Check: EsPrincipal; Flags: ignoreversion

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExe}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExe}"; Description: "{cm:LaunchProgram,{#MyAppName}}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
; Al desinstalar NO se toca la base de datos; solo se borran los archivos generados por el instalador.
Type: files; Name: "{app}\connections.config"
Type: files; Name: "{app}\instalacion_sql.log"

[Code]
var
  PaginaTipo: TInputOptionWizardPage;
  PaginaConexion: TInputQueryWizardPage;

function EsPrincipal: Boolean;
begin
  Result := PaginaTipo.Values[0];
end;

function Servidor: String;
begin
  Result := Trim(PaginaConexion.Values[0]);
end;

function UsuarioApp: String;
begin
  Result := Trim(PaginaConexion.Values[1]);
end;

function ClaveApp: String;
begin
  Result := PaginaConexion.Values[2];
end;

{ Nombre de la instancia a partir de "EQUIPO\INSTANCIA" (sin "\" es la instancia por defecto). }
function InstanciaSql: String;
var
  P: Integer;
begin
  P := Pos('\', Servidor);
  if P > 0 then
    Result := Copy(Servidor, P + 1, Length(Servidor))
  else
    Result := 'MSSQLSERVER';
end;

function NombreServicioSql: String;
begin
  if CompareText(InstanciaSql, 'MSSQLSERVER') = 0 then
    Result := 'MSSQLSERVER'
  else
    Result := 'MSSQL$' + InstanciaSql;
end;

{ ---------- Requisito: .NET Framework 4.7.2 ---------- }

function NetFramework472Instalado: Boolean;
var
  Version: Cardinal;
begin
  Result := RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', Version)
            and (Version >= 461808);
end;

function InitializeSetup: Boolean;
begin
  Result := NetFramework472Instalado;
  if not Result then
    MsgBox('Este programa requiere .NET Framework 4.7.2 o superior.' + #13#10 +
           'Instálelo desde https://dotnet.microsoft.com/download/dotnet-framework y vuelva a ejecutar el instalador.',
           mbCriticalError, MB_OK);
end;

{ ---------- Paginas del asistente ---------- }

procedure InitializeWizard;
begin
  PaginaTipo := CreateInputOptionPage(wpSelectDir,
    'Tipo de equipo', '¿Qué función cumple este equipo en la agencia?',
    'Seleccione una opción y haga clic en Siguiente.', True, False);
  PaginaTipo.Add('Equipo principal (Administrador): aquí está SQL Server y la base de datos');
  PaginaTipo.Add('Caja secundaria: se conecta a la base de datos del equipo principal');
  PaginaTipo.Values[0] := True;

  PaginaConexion := CreateInputQueryPage(PaginaTipo.ID,
    'Conexión a la base de datos', 'Datos del servidor SQL Server', '');
  PaginaConexion.Add('Servidor SQL (equipo\instancia):', False);
  PaginaConexion.Add('Usuario SQL de la aplicación:', False);
  PaginaConexion.Add('Contraseña del usuario SQL:', True);
  PaginaConexion.Values[1] := 'app_caja';
end;

procedure CurPageChanged(CurPageID: Integer);
begin
  if CurPageID <> PaginaConexion.ID then Exit;

  if EsPrincipal then
  begin
    PaginaConexion.SubCaptionLabel.Caption :=
      'Se creará la base de datos SistemaCaja y el usuario SQL indicado. ' +
      'Anote el usuario y la contraseña: los necesitará al instalar la caja secundaria.';
    if Servidor = '' then
      PaginaConexion.Values[0] := '.\SQLEXPRESS';
  end
  else
  begin
    PaginaConexion.SubCaptionLabel.Caption :=
      'Escriba el nombre del equipo principal (por ejemplo CAJA-PRINCIPAL\SQLEXPRESS) ' +
      'y el mismo usuario y contraseña que se usaron al instalar el equipo principal.';
    if Servidor = '.\SQLEXPRESS' then
      PaginaConexion.Values[0] := '';
  end;
end;

function TieneCaracterInvalido(const Texto, Invalidos: String): Boolean;
var
  I: Integer;
begin
  Result := False;
  for I := 1 to Length(Invalidos) do
    if Pos(Invalidos[I], Texto) > 0 then
    begin
      Result := True;
      Exit;
    end;
end;

function NextButtonClick(CurPageID: Integer): Boolean;
begin
  Result := True;
  if CurPageID <> PaginaConexion.ID then Exit;

  if (Servidor = '') or (UsuarioApp = '') or (ClaveApp = '') then
  begin
    MsgBox('Complete el servidor, el usuario y la contraseña.', mbError, MB_OK);
    Result := False;
    Exit;
  end;

  if (not EsPrincipal) and ((Copy(Servidor, 1, 1) = '.') or (Pos('(local)', Lowercase(Servidor)) > 0)
                            or (Pos('localhost', Lowercase(Servidor)) > 0)) then
  begin
    MsgBox('En la caja secundaria debe indicar el NOMBRE del equipo principal, no "." ni "localhost".' + #13#10 +
           'Ejemplo: CAJA-PRINCIPAL\SQLEXPRESS', mbError, MB_OK);
    Result := False;
    Exit;
  end;

  { Estos caracteres romperian la cadena de conexion, el XML o el script SQL. }
  if TieneCaracterInvalido(Servidor + UsuarioApp, ' ;''"&<>[]') then
  begin
    MsgBox('El servidor y el usuario no pueden contener espacios ni los caracteres ; '' " & < > [ ]', mbError, MB_OK);
    Result := False;
    Exit;
  end;

  if TieneCaracterInvalido(ClaveApp, ' ;''"&<>') then
  begin
    MsgBox('La contraseña no puede contener espacios ni los caracteres ; '' " & < >', mbError, MB_OK);
    Result := False;
    Exit;
  end;

  if EsPrincipal and (Length(ClaveApp) < 8) then
  begin
    MsgBox('La contraseña debe tener al menos 8 caracteres.', mbError, MB_OK);
    Result := False;
  end;
end;

{ ---------- Instalacion ---------- }

procedure EscribirConnectionsConfig;
var
  Cadena, Xml: String;
begin
  Cadena := 'Data Source=' + Servidor + ';Initial Catalog=SistemaCaja;User ID=' + UsuarioApp +
            ';Password=' + ClaveApp +
            ';MultipleActiveResultSets=true;Encrypt=false;TrustServerCertificate=true;Connection Timeout=30;';

  Xml := '<?xml version="1.0" encoding="utf-8"?>' + #13#10 +
         '<!-- Generado por el instalador de Sistema de Caja. -->' + #13#10 +
         '<connectionStrings>' + #13#10 +
         '  <add name="cadena_conexion"' + #13#10 +
         '       connectionString="' + Cadena + '"' + #13#10 +
         '       providerName="System.Data.SqlClient" />' + #13#10 +
         '</connectionStrings>' + #13#10;

  SaveStringToFile(ExpandConstant('{app}\connections.config'), Xml, False);
end;

function Ejecutar(const Programa, Parametros: String): Integer;
var
  Codigo: Integer;
begin
  if not Exec(Programa, Parametros, '', SW_HIDE, ewWaitUntilTerminated, Codigo) then
    Codigo := -1;
  Result := Codigo;
end;

{ Deja SQL Server listo para que la caja secundaria se conecte por la red local:
  autenticacion mixta, TCP/IP, SQL Browser y reglas de firewall. }
function ConfigurarSqlServerParaRed: Boolean;
var
  IdInstancia, ClaveInstancia, BinRoot, Netsh, Net: String;
begin
  Result := False;

  if not RegQueryStringValue(HKLM64, 'SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL',
                             InstanciaSql, IdInstancia) then
  begin
    MsgBox('No se encontró la instancia de SQL Server "' + InstanciaSql + '" en este equipo.' + #13#10 +
           'Instale SQL Server Express antes de instalar el equipo principal, o revise el nombre del servidor.',
           mbError, MB_OK);
    Exit;
  end;

  ClaveInstancia := 'SOFTWARE\Microsoft\Microsoft SQL Server\' + IdInstancia;
  Net := ExpandConstant('{sys}\net.exe');
  Netsh := ExpandConstant('{sys}\netsh.exe');

  { Autenticacion mixta (Windows + SQL) y TCP/IP habilitado; requieren reiniciar el servicio. }
  RegWriteDWordValue(HKLM64, ClaveInstancia + '\MSSQLServer', 'LoginMode', 2);
  RegWriteDWordValue(HKLM64, ClaveInstancia + '\MSSQLServer\SuperSocketNetLib\Tcp', 'Enabled', 1);

  Ejecutar(Net, 'stop "' + NombreServicioSql + '" /y');
  if Ejecutar(Net, 'start "' + NombreServicioSql + '"') <> 0 then
  begin
    MsgBox('No se pudo reiniciar el servicio ' + NombreServicioSql + '.', mbError, MB_OK);
    Exit;
  end;

  { SQL Browser: permite conectarse por nombre de instancia (EQUIPO\SQLEXPRESS). }
  Ejecutar(ExpandConstant('{sys}\sc.exe'), 'config SQLBrowser start= auto');
  Ejecutar(Net, 'start SQLBrowser');

  { Firewall: solo redes privadas/dominio. Se borran primero para no duplicar al reinstalar. }
  Ejecutar(Netsh, 'advfirewall firewall delete rule name="Sistema de Caja - SQL Server"');
  Ejecutar(Netsh, 'advfirewall firewall delete rule name="Sistema de Caja - SQL Browser"');
  if RegQueryStringValue(HKLM64, ClaveInstancia + '\Setup', 'SQLBinRoot', BinRoot) then
    Ejecutar(Netsh, 'advfirewall firewall add rule name="Sistema de Caja - SQL Server" dir=in action=allow ' +
                    'program="' + AddBackslash(BinRoot) + 'sqlservr.exe" enable=yes profile=private,domain');
  Ejecutar(Netsh, 'advfirewall firewall add rule name="Sistema de Caja - SQL Browser" dir=in action=allow ' +
                  'protocol=UDP localport=1434 enable=yes profile=private,domain');

  Result := True;
end;

var
  RutaSqlcmd: String;

function RutaLogSql: String;
begin
  Result := ExpandConstant('{app}\instalacion_sql.log');
end;

{ Busca sqlcmd.exe: en el PATH del instalador, en el PATH del sistema (por si SQL Server se
  instalo en esta misma sesion y el PATH del proceso quedo desactualizado) y en rutas conocidas. }
function BuscarSqlcmd: String;
var
  PathSistema: String;
begin
  Result := FileSearch('sqlcmd.exe', GetEnv('PATH'));
  if Result <> '' then Exit;

  if RegQueryStringValue(HKLM, 'SYSTEM\CurrentControlSet\Control\Session Manager\Environment', 'Path', PathSistema) then
  begin
    Result := FileSearch('sqlcmd.exe', ExpandConstant(PathSistema));
    if Result <> '' then Exit;
  end;

  Result := FileSearch('sqlcmd.exe',
    ExpandConstant('{commonpf64}\SqlCmd;') +
    ExpandConstant('{commonpf64}\Microsoft SQL Server\Client SDK\ODBC\180\Tools\Binn;') +
    ExpandConstant('{commonpf64}\Microsoft SQL Server\Client SDK\ODBC\170\Tools\Binn;') +
    ExpandConstant('{commonpf64}\Microsoft SQL Server\Client SDK\ODBC\130\Tools\Binn;') +
    ExpandConstant('{commonpf64}\Microsoft SQL Server\110\Tools\Binn'));
end;

{ Ultimas lineas del log de sqlcmd, para mostrar el error real en pantalla. }
function UltimasLineasLog: String;
var
  Lineas: TArrayOfString;
  I, Desde: Integer;
begin
  Result := '';
  if not LoadStringsFromFile(RutaLogSql, Lineas) then Exit;
  Desde := GetArrayLength(Lineas) - 8;
  if Desde < 0 then Desde := 0;
  for I := Desde to GetArrayLength(Lineas) - 1 do
    if Trim(Lineas[I]) <> '' then
      Result := Result + Trim(Lineas[I]) + #13#10;
end;

procedure MostrarErrorSql(const Mensaje: String);
begin
  MsgBox(Mensaje + #13#10 + #13#10 +
         'Detalle:' + #13#10 + UltimasLineasLog + #13#10 +
         'Log completo: ' + RutaLogSql + #13#10 +
         'Corrija el problema y vuelva a ejecutar el instalador (no se pierden datos).',
         mbError, MB_OK);
end;

{ Ejecuta sqlcmd con autenticacion de Windows (el usuario que instala debe ser sysadmin,
  lo cual SQL Server Express configura por defecto). La salida queda en instalacion_sql.log.
  -I activa QUOTED_IDENTIFIER (sqlcmd lo deja OFF y los indices filtrados lo exigen).
  -C confia en el certificado autofirmado de SQL Server: las versiones nuevas de sqlcmd
  cifran la conexion por defecto y sin -C fallan con "certificate chain ... not trusted". }
function EjecutarSql(const Parametros: String): Boolean;
begin
  Result := Ejecutar(ExpandConstant('{cmd}'),
    '/C ""' + RutaSqlcmd + '" -S "' + Servidor + '" -E -C -I -b ' + Parametros +
    ' >> "' + RutaLogSql + '" 2>&1"') = 0;
end;

{ Tras reiniciar el servicio, SQL Server tarda unos segundos en aceptar conexiones. }
function EsperarSqlServer: Boolean;
var
  I: Integer;
begin
  Result := False;
  for I := 1 to 15 do
  begin
    if EjecutarSql('-l 5 -Q "SELECT 1"') then
    begin
      Result := True;
      Exit;
    end;
    Sleep(2000);
  end;
end;

procedure ConfigurarEquipoPrincipal;
begin
  DeleteFile(RutaLogSql);

  RutaSqlcmd := BuscarSqlcmd;
  if RutaSqlcmd = '' then
  begin
    MsgBox('No se encontró sqlcmd en este equipo; es necesario para crear la base de datos.' + #13#10 + #13#10 +
           'Instálelo abriendo una terminal como administrador y ejecutando:' + #13#10 +
           '    winget install Microsoft.Sqlcmd' + #13#10 + #13#10 +
           'Luego vuelva a ejecutar este instalador.', mbError, MB_OK);
    Exit;
  end;

  WizardForm.StatusLabel.Caption := 'Configurando SQL Server para la red local...';
  if not ConfigurarSqlServerParaRed then Exit;

  WizardForm.StatusLabel.Caption := 'Esperando a que SQL Server acepte conexiones...';
  if not EsperarSqlServer then
  begin
    MostrarErrorSql('No se pudo conectar a SQL Server (' + Servidor + ') con el usuario de Windows actual.');
    Exit;
  end;

  WizardForm.StatusLabel.Caption := 'Creando o actualizando la base de datos SistemaCaja...';
  if not EjecutarSql('-i "' + ExpandConstant('{app}\sql\script_completo_base_datos.sql') + '"') then
  begin
    MostrarErrorSql('No se pudo crear o actualizar la base de datos.');
    Exit;
  end;

  WizardForm.StatusLabel.Caption := 'Creando el usuario SQL de la aplicación...';
  if not EjecutarSql('-i "' + ExpandConstant('{app}\sql\crear_usuario_app.sql') + '" ' +
                     '-v UsuarioApp="' + UsuarioApp + '" ClaveApp="' + ClaveApp + '"') then
  begin
    MostrarErrorSql('No se pudo crear el usuario SQL de la aplicación.');
    Exit;
  end;

  MsgBox('Equipo principal configurado.' + #13#10 + #13#10 +
         'Al instalar la caja secundaria use como servidor:' + #13#10 +
         '    ' + GetComputerNameString + '\' + InstanciaSql + #13#10 +
         'y el usuario "' + UsuarioApp + '" con la misma contraseña.',
         mbInformation, MB_OK);
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep <> ssPostInstall then Exit;

  EscribirConnectionsConfig;
  if EsPrincipal then
    ConfigurarEquipoPrincipal;
end;
