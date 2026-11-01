#Requires -RunAsAdministrator
[CmdletBinding()]
param(
	[string]$InstallDirectory = "$env:ProgramFiles\SasaPdfRelayPrinter",
	[string]$PrinterName = "Sasa PDF Relay Printer",
	[string]$ShareName = "SasaPdfRelay",
	[string]$PortName = "SASA_PDF_RELAY",
	[int]$PortNumber = 19111
)

$ErrorActionPreference = 'Stop'
$projectDirectory = $PSScriptRoot
$publishDirectory = Join-Path $projectDirectory 'publish'
$driver = Get-PrinterDriver -Name 'Microsoft Print To PDF' -ErrorAction Stop

Write-Host 'Building Sasa PDF Relay Printer...'
dotnet publish (Join-Path $projectDirectory 'PdfRelayPrinter.csproj') -c Release -r win-x64 --self-contained true -o $publishDirectory

New-Item -ItemType Directory -Path $InstallDirectory -Force | Out-Null
Copy-Item (Join-Path $publishDirectory '*') $InstallDirectory -Recurse -Force

if (-not (Get-PrinterPort -Name $PortName -ErrorAction SilentlyContinue)) {
	Add-PrinterPort -Name $PortName -PrinterHostAddress '127.0.0.1' -PortNumber $PortNumber
}

if (-not (Get-Printer -Name $PrinterName -ErrorAction SilentlyContinue)) {
	Add-Printer -Name $PrinterName -DriverName $driver.Name -PortName $PortName
}

Set-Printer -Name $PrinterName -Shared $true -ShareName $ShareName -Published $false

$exe = Join-Path $InstallDirectory 'SasaPdfRelayPrinter.exe'
$startup = [Environment]::GetFolderPath('Startup')
$shortcutPath = Join-Path $startup 'Sasa PDF Relay Printer.lnk'
$shell = New-Object -ComObject WScript.Shell
$shortcut = $shell.CreateShortcut($shortcutPath)
$shortcut.TargetPath = $exe
$shortcut.WorkingDirectory = $InstallDirectory
$shortcut.Save()

Start-Process $exe

Write-Host ''
Write-Host 'Installed successfully.' -ForegroundColor Green
Write-Host "Virtual printer: $PrinterName"
Write-Host "Shared name: \\$env:COMPUTERNAME\$ShareName"
Write-Host "Loopback port: 127.0.0.1:$PortNumber"
