#Requires -RunAsAdministrator
[CmdletBinding()]
param([string]$InstallDirectory = "$env:ProgramFiles\SasaImagePrinter")

$ErrorActionPreference = 'Stop'
Get-Process SasaImagePrinter -ErrorAction SilentlyContinue | Stop-Process -Force
foreach ($name in @('Sasa Image Printer (JPEG)', 'Sasa Image Printer (TIFF)')) {
    if (Get-Printer -Name $name -ErrorAction SilentlyContinue) { Remove-Printer -Name $name }
}
foreach ($port in @('SASA_IMAGE_JPEG', 'SASA_IMAGE_TIFF')) {
    if (Get-PrinterPort -Name $port -ErrorAction SilentlyContinue) { Remove-PrinterPort -Name $port }
}
$shortcut = Join-Path ([Environment]::GetFolderPath('Startup')) 'Sasa Image Printer.lnk'
Remove-Item $shortcut -Force -ErrorAction SilentlyContinue
Write-Host "Printer registrations removed. Installed files remain at: $InstallDirectory"
