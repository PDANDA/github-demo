param(
	[string] $RGName = "Integration-Common",
    [string] $IAName = "IA-Common",
    [string] $WorkingDirectory = $PSScriptRoot + "\..",
	[Parameter(mandatory=$true)]    
	[string] $Environment = "",
	[Parameter(mandatory=$true)]    
	[string] $CommonEnvironment = ""
)

# Construct environment specific resource names
$IAName = $CommonEnvironment + "-" + $IAName
$CommonRGName = $CommonEnvironment + "-" + $RGName
$RGName = $Environment + "-" + $RGName


Write-Host "***************************************************************************"
Write-Host "Environment: " -NoNewline; Write-Host $Environment
Write-Host "Integration Account Resource Group: " -NoNewline; Write-Host $CommonRGName
Write-Host "Integration Account: " -NoNewline; Write-Host $IAName
Write-Host "Working Directory: " -NoNewline; Write-Host $WorkingDirectory
Write-Host "***************************************************************************"


# Get/Create Integration Account
$IntegrationAccount = Get-AzureRmIntegrationAccount -ResourceGroupName $CommonRGName -Name $IAName -ErrorAction SilentlyContinue -ErrorVariable e
if ($e[0] -ne $null)
{
	Write-Host "Integration Account " -NoNewline; Write-Host $IAName -NoNewline; Write-Host " not found."
	exit
}


# Upload Schemas
$schemaPath = $WorkingDirectory + "\Schemas"
Write-Host "Looking for schemas in " -NoNewline; Write-Host $schemaPath
if(Test-Path $schemaPath)
{
    $files = Get-ChildItem $schemaPath "*.xsd"
    foreach($file in $files)
    {
        $schemaContent = [IO.File]::ReadAllText($file.FullName)
		$schemaName = $Environment + "-" + $file.BaseName
        $schema = Get-AzureRmIntegrationAccountSchema -ResourceGroupName $CommonRGName -Name $IAName -SchemaName $schemaName -ErrorAction SilentlyContinue -ErrorVariable e
        if ($e[0] -ne $null)
        {
		    Write-Host "Adding schema " -NoNewline; Write-Host $schemaName
            New-AzureRmIntegrationAccountSchema -ResourceGroupName $CommonRGName -Name $IAName -SchemaName $schemaName -SchemaDefinition $schemaContent
        }
	    else
	    {
		    Write-Host "Replacing schema " -NoNewline; Write-Host $schemaName
		    Remove-AzureRmIntegrationAccountSchema -ResourceGroupName $CommonRGName -Name $IAName -SchemaName $schemaName -Force
		    New-AzureRmIntegrationAccountSchema -ResourceGroupName $CommonRGName -Name $IAName -SchemaName $schemaName -SchemaDefinition $schemaContent
	    }
    }
}
else
{
    Write-Output "No schemas found"
}

# Upload Maps
$mapPath = $WorkingDirectory + "\Maps"
Write-Host "Looking for maps in " -NoNewline; Write-Host $mapPath
if(Test-Path $mapPath)
{
    $files = Get-ChildItem $mapPath "*.xslt"
    foreach($file in $files)
    {
        $mapContent = [IO.File]::ReadAllText($file.FullName)
		$mapName = $Environment + "-" + $file.BaseName
        $map = Get-AzureRmIntegrationAccountMap -ResourceGroupName $CommonRGName -Name $IAName -MapName $mapName -ErrorAction SilentlyContinue -ErrorVariable e
        if($e[0] -ne $null)
        {
		    Write-Host "Adding map " -NoNewline; Write-Host $mapName
            New-AzureRmIntegrationAccountMap -ResourceGroupName $CommonRGName -Name $IAName -MapName $mapName  -MapDefinition $mapContent
        }
	    else
	    {
		    Write-Host "Replacing map " -NoNewline; Write-Host $mapName
		    Remove-AzureRmIntegrationAccountMap -ResourceGroupName $CommonRGName -Name $IAName -MapName $mapName -Force
		    New-AzureRmIntegrationAccountMap -ResourceGroupName $CommonRGName -Name $IAName -MapName $mapName  -MapDefinition $mapContent
	    }
    }
}
else
{
    Write-Output "No maps found"
}


