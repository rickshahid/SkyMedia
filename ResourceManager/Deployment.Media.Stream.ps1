# Get-Module AzureRM -ListAvailable | Select-Object -Property Name, Version, Path
# Install-Module -Name AzureRM -Repository PSGallery -Force
# Uninstall-Module -Name AzureRM
# v6.10.0 - October 2018

# Connect-AzureRmAccount

# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Storage).ResourceTypes | Where-Object ResourceTypeName -eq storageAccounts).ApiVersions
# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Media).ResourceTypes | Where-Object ResourceTypeName -eq mediaServices).ApiVersions

$appName = "Sky.Media"

$templateFile = $PSScriptRoot + "\Template.Media.Stream.json"

$regionLocation = "West US"

$resourceGroupName = $appName + "-US.West-Stream"

$templateParameters = @{
	"storageServiceName" = "SkyMediaUSWestStream"
	"storageServiceHttpsOnly" = $true
	"storageServiceEncryptionEnabled" = $true
	"mediaServiceName" = "SkyCast1"
}

$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
if (!$resourceGroup)
{
	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
}
New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFile -TemplateParameterObject $templateParameters

$regionLocation = "East US"

$resourceGroupName = $appName + "-US.East-Stream"

$templateParameters = @{
	"storageServiceName" = "SkyMediaUSEastStream"
	"storageServiceHttpsOnly" = $true
	"storageServiceEncryptionEnabled" = $true
	"mediaServiceName" = "SkyCast2"
}

$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
if (!$resourceGroup)
{
	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
}
New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFile -TemplateParameterObject $templateParameters