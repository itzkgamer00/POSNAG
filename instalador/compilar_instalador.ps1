# Compila la solucion en Release y genera el instalador con Inno Setup 6.
# Uso (desde la raiz del repo):  .\instalador\compilar_instalador.ps1
# Resultado: instalador\Output\Instalador_SistemaCaja_<version>.exe

$ErrorActionPreference = 'Stop'
$raiz = Split-Path $PSScriptRoot -Parent

$vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
$msbuild = & $vswhere -latest -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
if (-not $msbuild) { throw 'No se encontro MSBuild (Visual Studio).' }

& $msbuild "$raiz\PosNg3.sln" /t:Rebuild /p:Configuration=Release /v:minimal /nologo
if ($LASTEXITCODE -ne 0) { throw 'Fallo la compilacion en Release.' }

$iscc = @(
    "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe",
    "$env:ProgramFiles\Inno Setup 6\ISCC.exe",
    "$env:LOCALAPPDATA\Programs\Inno Setup 6\ISCC.exe"
) | Where-Object { Test-Path $_ } | Select-Object -First 1
if (-not $iscc) { throw 'No se encontro Inno Setup 6. Instalelo con: winget install JRSoftware.InnoSetup' }

& $iscc "$PSScriptRoot\SistemaCaja.iss"
if ($LASTEXITCODE -ne 0) { throw 'Fallo la compilacion del instalador.' }
