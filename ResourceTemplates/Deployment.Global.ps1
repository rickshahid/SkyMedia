# (Get-Module Azure -ListAvailable).Version (v4.4.1 - October 2017)
# Login-AzureRmAccount

# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Storage).ResourceTypes | Where-Object ResourceTypeName -eq storageAccounts).ApiVersions
# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.DocumentDB).ResourceTypes | Where-Object ResourceTypeName -eq databaseAccounts).ApiVersions
# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Search).ResourceTypes | Where-Object ResourceTypeName -eq searchServices).ApiVersions
# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Web).ResourceTypes | Where-Object ResourceTypeName -eq sites).ApiVersions
# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Cdn).ResourceTypes | Where-Object ResourceTypeName -eq profiles).ApiVersions
# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Network).ResourceTypes | Where-Object ResourceTypeName -eq trafficManagerProfiles).ApiVersions
# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Insights).ResourceTypes | Where-Object ResourceTypeName -eq components).ApiVersions

$appName = "Azure.Sky.Media"

$templateFile = ($PSScriptRoot + "\Template.Global.json")

$resourceGroupName = ($appName + "-US.Central")

$regionLocation = "Central US"

$templateParameters = @{
	"storageServiceName" = "SkyMedia"
	"databaseServiceName" = "SkyMedia"
	"databaseDataRegions" = "South Central US", "West US", "East US"
	"searchServiceName" = "SkyMedia"
	"searchServiceRegion" = "South Central US"
	"searchServiceTier" = "Free"
	"searchServiceReplicaCount" = 1
	"searchServicePartitionCount" = 1
	"functionAppName" = "SkyFunction-USCentral"
	"contentDeliveryProfileName" = "SkyMedia-Akamai"
	"contentDeliverySubdomainName" = "SkyStorage"
	"trafficManagerSubdomainName" = "SkyMedia"
	"trafficManagerRoutingMethod" = "Performance"
	"trafficManagerRoutingTimeToLive" = 30 # seconds
	"appInsightsName" = "Azure Sky Media"
	"appInsightsRegion" = "South Central US"
}

$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
if (!$resourceGroup)
{
	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
}
New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFile -TemplateParameterObject $templateParameters