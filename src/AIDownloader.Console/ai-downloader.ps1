$scriptPath = Split-Path -Parent -Path $MyInvocation.MyCommand.Definition
$exePath = Join-Path -Path $scriptPath -ChildPath "AIDownloader.Console.exe"
$params = $args
& $exePath $params