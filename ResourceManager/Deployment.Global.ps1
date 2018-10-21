# Get-Module AzureRM -ListAvailable | Select-Object -Property Name, Version, Path
# Install-Module -Name AzureRM -Repository PSGallery -Force
# Uninstall-Module -Name AzureRM
# v6.10.0 - October 2018

# Connect-AzureRmAccount

# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Storage).ResourceTypes | Where-Object ResourceTypeName -eq storageAccounts).ApiVersions
# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.DocumentDB).ResourceTypes | Where-Object ResourceTypeName -eq databaseAccounts).ApiVersions
# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Search).ResourceTypes | Where-Object ResourceTypeName -eq searchServices).ApiVersions
# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Cdn).ResourceTypes | Where-Object ResourceTypeName -eq profiles).ApiVersions
# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Insights).ResourceTypes | Where-Object ResourceTypeName -eq components).ApiVersions

$appName = "Sky.Media"

$templateFile = $PSScriptRoot + "\Template.Global.json"

$regionLocation = "Central US"

$resourceGroupName = $appName + "-US.Central"

$templateParameters = @{
	"storageServiceName" = "SkyMediaUSCentral"
	"storageServiceHttpsOnly" = $true
	"storageServiceEncryptionEnabled" = $true
	"databaseServiceName" = "SkyMedia"
	"databaseServiceRegions" = "West US;East US"
	"searchServiceName" = "SkyMedia"
	"searchServiceRegion" = "South Central US"
	"searchServiceTier" = "Free"
	"searchServiceReplicaCount" = 1
	"searchServicePartitionCount" = 1
	"contentDeliveryProfileName" = "SkyMediaUSCentral"
	"contentDeliveryProfileTier" = "Standard_Microsoft"
	"contentDeliveryEndpointSubdomain" = "SkyMedia"
	"contentDeliveryEndpointOriginPath" = "/cdn"
	"appInsightsRegion" = "South Central US"
	"appInsightsName" = "Azure Sky Media"
}

$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
if (!$resourceGroup)
{
	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
}
New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFile -TemplateParameterObject $templateParameters