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
	"directoryB2bTenantId" = "26baf8c5-e851-410f-8789-b491c6396ecf"
	"directoryB2bTenantDomain" = "skymediab2b.onmicrosoft.com"
	"directoryB2bClientId" = "9a773e4c-d1d6-4fdf-adcb-df195c5f3c03"
	"directoryB2cTenantId" = "7fe6884f-bc15-434b-902d-90f9252495f8"
	"directoryB2cTenantDomain" = "skymediab2c.onmicrosoft.com"
	"directoryB2cClientId" = "66caa1db-0ccb-46a6-a0fd-44ff62e344d5"
	"directoryB2cPolicyIdSignUpIn" = "B2C_1_SignUpIn"
	"storageServiceName" = "SkyMedia"
	"databaseServiceName" = "SkyMedia"
	"databaseDataRegions" = "South Central US", "West US", "East US"
	"searchServiceName" = "SkyMedia"
	"searchServiceRegion" = "South Central US"
	"searchServiceTier" = "Free"
	"searchServiceReplicaCount" = 1
	"searchServicePartitionCount" = 1
	"functionAppName" = "SkyFunction-USCentral"
	"contentDeliveryProfileName" = "SkyMedia-USCentral"
    "contentDeliveryProfileTier" = "Standard_Akamai"
	"contentDeliveryEndpointSubdomain" = "SkyStorage"
	"contentDeliveryEndpointOriginPath" = "/cdn"
	"trafficManagerSubdomainName" = "SkyMedia"
	"trafficManagerRoutingMethod" = "Performance"
	"trafficManagerRoutingTimeToLive" = 30 # Seconds
	"appInsightsName" = "Azure Sky Media"
	"appInsightsRegion" = "South Central US"
}

$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
if (!$resourceGroup)
{
	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
}
New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFile -TemplateParameterObject $templateParameters