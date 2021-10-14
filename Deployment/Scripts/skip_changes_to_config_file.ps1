<#
This script tells git to ignore changes to config files but not to ignore the file itself.  This is great
for config files that need to be part of source control and act as a template that defines the settings 
of the application with empty values or appropriate defaults. 
#>

Param(
    [switch] $undo # Turns tracking back on so you can edit config files and the check them back in and then turn tracking back off
)

$solutionRoot = Get-Location
$ACSAgentHubPath = $(Join-Path $PSScriptRoot ..\.. "ACSAgentHub" -Resolve)

Set-Location $ACSAgentHubPath

if ($undo) {
  .\Deployment\Scripts\skip_changes_to_config_file.ps1 -undo $undo
} else {
  .\Deployment\Scripts\skip_changes_to_config_file.ps1
}

Set-Location $solutionRoot

$ComposerAssistantSettings = $(Join-Path $PSScriptRoot ..\.. "ComposerAssistant\ComposerAssistant\settings\appsettings.json" -Resolve)

Write-Host "ComposerAssistantSettings:" $ComposerAssistantSettings

if ($undo) {
	$x = git update-index --no-skip-worktree $ComposerAssistantSettings

	#Write-Host "Git has resumed tracking changes to the following configuration files:" -ForegroundColor Green
}
else {
Write-Host "git update-index --skip-worktree $ComposerAssistantSettings"

	#Write-Host "Git is ignoring changes to the following configuration files:" -ForegroundColor Green
}

# The skip_changes_to_config_file.ps1 in ACSAgentHub will have already wrote out the "git has resumed/ignored" heading so just need to add to that here
write-Host $ComposerAssistantSettings
