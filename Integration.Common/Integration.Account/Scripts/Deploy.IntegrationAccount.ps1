param(
    [string] $RGName = "Integration-Common",
    [string] $IAName = "IA-Common",
    [string] $WorkingDirectory = $PSScriptRoot + "\..",
	[Parameter(mandatory=$true)]
	[string] $Environment = ""
)

# Identify common resource group based on environment
# DEV/SIT/UAT - DEV-Integration-Common
# PRD - PRD-Integration-Common

switch($Environment)
{
	"DEV"
	{ 
		$RGName = "DEV-" + $RGName 
		$IAName = "DEV-" + $IAName 
	}
	"SIT"
	{ 
		$RGName = "DEV-" + $RGName 
		$IAName = "DEV-" + $IAName
	}
	"UAT"
	{ 
		$RGName = "DEV-" + $RGName 
		$IAName = "DEV-" + $IAName
	}
	"PRD"
	{ 
		$RGName = "PRD-" + $RGName 
		$IAName = "PRD-" + $IAName
	}
	default 
	{ 
		$RGName = "Unknown" 
		$IAName = "Unknown"
	}
}

Write-Host "***************************************************************************"
Write-Host "Environment: " -NoNewline; Write-Host $Environment
Write-Host "Integration Account Resource Group: " -NoNewline; Write-Host $RGName
Write-Host "Integration Account: " -NoNewline; Write-Host $IAName
Write-Host "***************************************************************************"

