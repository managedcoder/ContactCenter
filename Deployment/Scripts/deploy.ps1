<# 
Run this script from the ContactCenter solution folder (i.e., the one that has ContactCenter.sln)
#>

#Requires PowerShell 7

# Usage:
# The following command deploys and configures a Contgact Center Assistant (CCA) named AcmeCCA to run locally
# .\Deployment\Scripts\deploy.ps1 -name AcmeCCA -resourceGroup ContactCenterRG -location eastus -NuGetFullPath c:\nuget\nuget.exe
# 
# The following command just configures the agent hub named TestHubRHW21 to run locally but does not deploy any services. Use this command
# re-setup you local environment when you are resuming work on your project.  It restarts all the necessary services and to configure the 
# Event Grid Message subscription with the ngrok endpoint to the Function App that was started by this script and is running locally
# .\Deployment\Scripts\deploy.ps1 -name AcmeCCA -resourceGroup ContactCenterRG -configurationOnly true

Param(
    [string] $name,
    [string] $resourceGroup,
    [string] $configurationOnly = "false", # Just configure things to run locally, but don't deploy (remaining params are not needed with -restart command)
    [string] $location,
    [string] $NuGetFullPath,
    [string] $connectorPackageVersion = "1.0.0",
    [string] $showCommands = "false",
    [string] $projDir = $(Get-Location),
    [string] $logFile = $(Join-Path $PSScriptRoot .. "deploy_log.txt")
)

# Get timestamp
$startTime = Get-Date

# Reset log file
if (Test-Path $logFile) {
    Clear-Content $logFile -Force | Out-Null
}
else {
    New-Item -Path $logFile | Out-Null
}

# Get mandatory parameters
if (-not $name) {
    $name = Read-Host "? Bot Name (used as default name for resource group and deployed resources)"
}

if (-not $resourceGroup) {
    $resourceGroup = $name
}

if (-not $location -and $configurationOnly.ToLower() -eq "false") {
    $location = Read-Host "? Azure resource group region"
}

# Get timestamp
$timestamp = Get-Date -Format MMddyyyyHHmmss
$startTime = Get-Date

$solutionRoot = Get-Location
$currentDir = $solutionRoot
$agentPortalPath = ".\agent-portal"
$ACSAgentHubPath = ".\ACSAgentHub"
$errCnt = 0



$endTime = Get-Date
$duration = New-TimeSpan $startTime $endTime
Write-Host "deploy_and_configure.ps1 took to $($duration.minutes) minutes finish"

Write-Host "Deployment and configuration completed with $errCnt errors"
