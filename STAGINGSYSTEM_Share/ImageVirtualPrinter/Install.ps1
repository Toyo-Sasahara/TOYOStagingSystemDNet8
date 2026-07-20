#Requires -RunAsAdministrator
[CmdletBinding()]
param(
    [string]$InstallDirectory = "$env:ProgramFiles\SasaImagePrinter",
    [switch]$RegisterForms = $true
)

$ErrorActionPreference = 'Stop'
$projectDirectory = $PSScriptRoot
$publishDirectory = Join-Path $projectDirectory 'publish'

Import-Module PrintManagement -ErrorAction Stop

function Test-RequiredCommand([string]$CommandName) {
    if (-not (Get-Command -Name $CommandName -ErrorAction SilentlyContinue)) {
        throw "Required PowerShell command not found: $CommandName. Check Windows PrintManagement features."
    }
}

function Add-IsoPrinterForms {
    $addPrinterFormCommand = Get-Command -Name 'Add-PrinterForm' -ErrorAction SilentlyContinue
    if (-not $addPrinterFormCommand) {
        Write-Host '  Warning: Add-PrinterForm is not available. Skipping PowerShell form registration and relying on application startup registration.'
        return
    }

    try {
        Add-PrinterForm -Name 'ISO A0' -Width 33.1 -Height 46.8 -ErrorAction Stop
        Write-Host '  OK: ISO A0 form registered'
    }
    catch {
        Write-Host ('  Warning: ISO A0 form registration failed: ' + $_.Exception.Message)
    }

    try {
        Add-PrinterForm -Name 'ISO A1' -Width 23.4 -Height 33.1 -ErrorAction Stop
        Write-Host '  OK: ISO A1 form registered'
    }
    catch {
        Write-Host ('  Warning: ISO A1 form registration failed: ' + $_.Exception.Message)
    }
}

function Add-ImagePrinter([string]$Name, [string]$PortName, [int]$PortNumber, [string]$DriverName) {
    if (-not (Get-PrinterPort -Name $PortName -ErrorAction SilentlyContinue)) {
        Add-PrinterPort -Name $PortName -PrinterHostAddress '127.0.0.1' -PortNumber $PortNumber
    }

    if (-not (Get-Printer -Name $Name -ErrorAction SilentlyContinue)) {
        Add-Printer -Name $Name -DriverName $DriverName -PortName $PortName
    }
}

foreach ($commandName in 'Get-PrinterDriver', 'Get-Printer', 'Add-Printer', 'Get-PrinterPort', 'Add-PrinterPort') {
    Test-RequiredCommand $commandName
}

if (-not (Get-Command -Name 'dotnet' -ErrorAction SilentlyContinue)) {
    throw 'dotnet SDK was not found. It is required to build ImageVirtualPrinter.'
}

try {
    $driver = Get-PrinterDriver -Name 'Microsoft Print To PDF' -ErrorAction Stop
}
catch {
    throw ('Microsoft Print to PDF driver was not found. Enable the Windows optional feature ''Microsoft Print to PDF''. Details: ' + $_.Exception.Message)
}

Write-Host 'Building Sasa Image Printer...'
dotnet publish (Join-Path $projectDirectory 'ImageVirtualPrinter.csproj') -c Release -r win-x64 --self-contained true -o $publishDirectory
if ($LASTEXITCODE -ne 0) {
    throw "dotnet publish failed with exit code: $LASTEXITCODE"
}

$exe = Join-Path $publishDirectory 'SasaImagePrinter.exe'
if (-not (Test-Path -Path $exe)) {
    throw "Published executable was not found: $exe"
}

New-Item -ItemType Directory -Path $InstallDirectory -Force | Out-Null

$runningProcess = Get-Process -Name 'SasaImagePrinter' -ErrorAction SilentlyContinue
if ($runningProcess) {
    Write-Host 'Stopping running Sasa Image Printer process...'
    $runningProcess | Stop-Process -Force
}

Copy-Item (Join-Path $publishDirectory '*') $InstallDirectory -Recurse -Force

if ($RegisterForms) {
    Write-Host 'Registering ISO A0/A1 printer forms...'
    Add-IsoPrinterForms
}

Add-ImagePrinter -Name 'Sasa Image Printer (JPEG)' -PortName 'SASA_IMAGE_JPEG' -PortNumber 19101 -DriverName $driver.Name
Add-ImagePrinter -Name 'Sasa Image Printer (TIFF)' -PortName 'SASA_IMAGE_TIFF' -PortNumber 19102 -DriverName $driver.Name

$installedExe = Join-Path $InstallDirectory 'SasaImagePrinter.exe'
$startup = [Environment]::GetFolderPath('Startup')
$shortcutPath = Join-Path $startup 'Sasa Image Printer.lnk'
$shell = New-Object -ComObject WScript.Shell
$shortcut = $shell.CreateShortcut($shortcutPath)
$shortcut.TargetPath = $installedExe
$shortcut.WorkingDirectory = $InstallDirectory
$shortcut.Save()

Start-Process -FilePath $installedExe

Write-Host ''
Write-Host 'Installed successfully.' -ForegroundColor Green
Write-Host 'Printers: Sasa Image Printer (JPEG), Sasa Image Printer (TIFF)'
Write-Host 'Forms: ISO A0, ISO A1 (registered by application startup)'
Write-Host 'Default output: Pictures\Image Printer Output'
