# PowerShell Script to publish a .NET Core Console App

# Navigate to the project directory
$projectDir = ".\AIDownloader.Console"
$outputDir = "$env:USERPROFILE\Desktop\ai-downloader"
$zipFile = "$env:USERPROFILE\Desktop\AIDownloader-CLI.zip"

# Check if the output directory exists, if so, delete it
if (Test-Path $outputDir) {
    Remove-Item $outputDir -Recurse -Force
}

# Check if the zip file exists, if so, delete it
if (Test-Path $zipFile) {
    Remove-Item $zipFile -Force
}

# Use dotnet publish to publish the app
dotnet build -c Release --force $projectDir
dotnet publish --output $outputDir -p:PublishProfile=FolderProfile $projectDir

if (-not (Test-Path -Path $outputDir\lib)) {
    New-Item -ItemType Directory -Path $outputDir\lib
}

if (Test-Path "$outputDir\config.json") {
    Remove-Item "$outputDir\config.json" -Force
}

# Copy all files from 'lib' directory to the output directory
Copy-Item -Path ".\lib\*" -Destination "$outputDir\lib" -Recurse -Force

# Compress the output directory into a zip file
Compress-Archive -Path "$outputDir\*" -DestinationPath $zipFile

# Delete the output directory
Remove-Item $outputDir -Recurse -Force