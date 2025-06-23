# PowerShell Build Script for ModelContextProtocol Server
param(
    [Parameter(Position=0)]
    [string]$Target = "build",
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64"
)

# Project configuration
$ProjectDir = "Server"
$ProjectFile = "$ProjectDir/modelcontextprotocol.csproj"
$OutputDir = "$ProjectDir/bin"

function Show-Help {
    Write-Host "Available targets:" -ForegroundColor Green
    Write-Host "  build       - Build the project in Release configuration"
    Write-Host "  build-debug - Build the project in Debug configuration"
    Write-Host "  restore     - Restore NuGet packages"
    Write-Host "  clean       - Clean build artifacts"
    Write-Host "  publish     - Publish the application as self-contained executable"
    Write-Host "  run         - Run the application"
    Write-Host "  run-debug   - Run the application in debug mode"
    Write-Host "  test        - Run tests"
    Write-Host "  rebuild     - Clean, restore, and build"
    Write-Host "  help        - Show this help message"
    Write-Host ""
    Write-Host "Usage examples:" -ForegroundColor Yellow
    Write-Host "  .\build.ps1"
    Write-Host "  .\build.ps1 build"
    Write-Host "  .\build.ps1 run"
    Write-Host "  .\build.ps1 publish"
}

function Test-DotNet {
    try {
        $version = dotnet --version
        Write-Host "Using .NET SDK version: $version" -ForegroundColor Green
        return $true
    }
    catch {
        Write-Error "Error: .NET SDK is not installed or not in PATH"
        return $false
    }
}

function Invoke-Build {
    Write-Host "Building ModelContextProtocol Server..." -ForegroundColor Cyan
    dotnet build $ProjectFile --configuration $Configuration
}

function Invoke-BuildDebug {
    Write-Host "Building ModelContextProtocol Server (Debug)..." -ForegroundColor Cyan
    dotnet build $ProjectFile --configuration Debug
}

function Invoke-Restore {
    Write-Host "Restoring NuGet packages..." -ForegroundColor Cyan
    dotnet restore $ProjectFile
}

function Invoke-Clean {
    Write-Host "Cleaning build artifacts..." -ForegroundColor Cyan
    dotnet clean $ProjectFile
    
    if (Test-Path $OutputDir) {
        Remove-Item $OutputDir -Recurse -Force
        Write-Host "Removed $OutputDir" -ForegroundColor Yellow
    }
    
    $objDir = "$ProjectDir/obj"
    if (Test-Path $objDir) {
        Remove-Item $objDir -Recurse -Force
        Write-Host "Removed $objDir" -ForegroundColor Yellow
    }
}

function Invoke-Publish {
    Write-Host "Publishing ModelContextProtocol Server..." -ForegroundColor Cyan
    dotnet publish $ProjectFile --configuration $Configuration --runtime $Runtime --self-contained true --single-file
}

function Invoke-Run {
    Write-Host "Running ModelContextProtocol Server..." -ForegroundColor Cyan
    dotnet run --project $ProjectFile
}

function Invoke-RunDebug {
    Write-Host "Running ModelContextProtocol Server (Debug)..." -ForegroundColor Cyan
    dotnet run --project $ProjectFile --configuration Debug
}

function Invoke-Test {
    Write-Host "Running tests..." -ForegroundColor Cyan
    dotnet test $ProjectFile
}

function Invoke-Rebuild {
    Write-Host "Performing full rebuild..." -ForegroundColor Cyan
    Invoke-Clean
    Invoke-Restore
    Invoke-Build
}

# Main execution
if (-not (Test-DotNet)) {
    exit 1
}

switch ($Target.ToLower()) {
    "build" { Invoke-Build }
    "build-debug" { Invoke-BuildDebug }
    "restore" { Invoke-Restore }
    "clean" { Invoke-Clean }
    "publish" { Invoke-Publish }
    "run" { Invoke-Run }
    "run-debug" { Invoke-RunDebug }
    "test" { Invoke-Test }
    "rebuild" { Invoke-Rebuild }
    "help" { Show-Help }
    default { 
        Write-Host "Unknown target: $Target" -ForegroundColor Red
        Show-Help
        exit 1
    }
} 