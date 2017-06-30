param(
    [string] $RGName = "",
    [string] $IAName = "",
    [string] $WorkingDirectory = ".",
	[string] $Environment = ""
)

# Get Azure Resource Group Location
$RGLocation = (Get-AzureRmResourceGroup -Name $RGName).Location
 
# Get/Create Integration Account
$IntegrationAccount = Get-AzureRmIntegrationAccount -ResourceGroupName $RGName -Name $IAName -ErrorAction SilentlyContinue -ErrorVariable e
if ($e[0] -ne $null)
{
    Write-Output "$(Get-Date –f $timeStampFormat) - No Integration Account detected. Creating " $IAName
    $integrationAccount = New-AzureRmIntegrationAccount -ResourceGroupName $RGName -Name $IAName -Location $RGLocation -Sku "Free"
}

# Upload Partners
$partnerPath = $WorkingDirectory + "\Partners"
$searchPattern = $Environment + "-*.json"
Write-Host "Looking for Partners in: " -NoNewline; Write-Host $partnerPath
if(Test-Path $partnerPath)
{
    $files = Get-ChildItem $partnerPath $searchPattern
    foreach($file in $files)
    {
		$businessIdentities = Get-Content -Raw -Path $file.PSPath | ConvertFrom-Json
		$biHashtable = @{}

		for ($i=0; $i -lt $businessIdentities.length; $i++) 
		{
			$biHashtable.Add($businessIdentities[$i].qualifier, $businessIdentities[$i].value)
		}

		$partnerName = $file.BaseName
		$partner = Get-AzureRmIntegrationAccountPartner -ResourceGroupName $RGName -Name $IAName -PartnerName $partnerName -ErrorAction SilentlyContinue -ErrorVariable e

		if($e[0] -ne $null)
		{
			Write-Host "Adding Partner: " -NoNewline; Write-Host $partnerName
			New-AzureRmIntegrationAccountPartner -ResourceGroupName $RGName -Name $IAName -PartnerName $partnerName -PartnerType B2B -BusinessIdentities $biHashtable
		}
		else
		{
			Write-Host "Replacing Partner: " -NoNewline; Write-Host $partnerName
			Remove-AzureRmIntegrationAccountPartner -ResourceGroupName $RGName -Name $IAName -PartnerName $partnerName -Force
			New-AzureRmIntegrationAccountPartner -ResourceGroupName $RGName -Name $IAName -PartnerName $partnerName -PartnerType B2B -BusinessIdentities $biHashtable
		}
    }
}
else
{
    Write-Output "No Partners found"
}
