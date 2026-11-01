#Requires -RunAsAdministrator
[CmdletBinding()]
param(
	[string]$InstallDirectory = "$env:ProgramFiles\SasaPdfRelayPrinter",
	[string]$PrinterName = "Sasa PDF Relay Printer",
	[string]$PortName = "SASA_PDF_RELAY"
)

$ErrorActionPreference = 'Stop'
Get-Process SasaPdfRelayPrinter -ErrorAction SilentlyContinue | Stop-Process -Force

if (Get-Printer -Name $PrinterName -ErrorAction SilentlyContinue) {
	Remove-Printer -Name $PrinterName
}

if (Get-PrinterPort -Name $PortName -ErrorAction SilentlyContinue) {
	Remove-PrinterPort -Name $PortName
}

$shortcut = Join-Path ([Environment]::GetFolderPath('Startup')) 'Sasa PDF Relay Printer.lnk'
Remove-Item $shortcut -Force -ErrorAction SilentlyContinue

Write-Host "Printer registration removed. Installed files remain at: $InstallDirectory"
