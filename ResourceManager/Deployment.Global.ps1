# Install-Module -Name AzureRM -Repository PSGallery -Force
# Update-Module -Name AzureRM
# Get-Module AzureRM -ListAvailable | Select-Object -Property Name, Version, Path
# v5.4.1 - February 2018

# Connect-AzureRmAccount

# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Storage).ResourceTypes | Where-Object ResourceTypeName -eq storageAccounts).ApiVersions
# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.DocumentDB).ResourceTypes | Where-Object ResourceTypeName -eq databaseAccounts).ApiVersions
# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Search).ResourceTypes | Where-Object ResourceTypeName -eq searchServices).ApiVersions
# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Web).ResourceTypes | Where-Object ResourceTypeName -eq sites).ApiVersions
# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Cdn).ResourceTypes | Where-Object ResourceTypeName -eq profiles).ApiVersions
# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Network).ResourceTypes | Where-Object ResourceTypeName -eq trafficManagerProfiles).ApiVersions
# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Insights).ResourceTypes | Where-Object ResourceTypeName -eq components).ApiVersions

$appName = "Azure.Sky.Media"

$templateFile = ($PSScriptRoot + "\Template.Global.json")

$templateParameters = @{
	"directoryB2bTenantId" = "26baf8c5-e851-410f-8789-b491c6396ecf"
	"directoryB2bTenantDomain" = "skymediab2b.onmicrosoft.com"
	"directoryB2cTenantId" = "7fe6884f-bc15-434b-902d-90f9252495f8"
	"directoryB2cTenantDomain" = "skymediab2c.onmicrosoft.com"
	"directoryB2cPolicyIdSignUpIn" = "B2C_1_SignUpIn"
	"storageServiceName" = "SkyMedia"
	"databaseServiceName" = "SkyMedia"
	"databaseDataRegions" = "South Central US", "West US", "East US"
	"searchServiceName" = "SkyMedia"
	"searchServiceRegion" = "South Central US"
	"searchServiceTier" = "Free"
	"searchServiceReplicaCount" = 1
	"searchServicePartitionCount" = 1
	"functionAppName" = "SkyMedia-USCentral"
	"contentDeliveryProfileName" = "SkyMedia-USCentral"
    "contentDeliveryProfileTier" = "Standard_Akamai"
	"contentDeliveryEndpointSubdomain" = "SkyStorage"
	"contentDeliveryEndpointOriginPath" = "/cdn"
	"trafficManagerSubdomainName" = "SkyMedia"
	"trafficManagerRoutingMethod" = "Performance"
	"trafficManagerRoutingTimeToLive" = 30 # Seconds
	"trafficManagerAppResourceGroupNames" = ($appName + "-US.West"), ($appName + "-US.East")
	"trafficManagerAppEndpointNames" = "SkyMedia-USWest", "SkyMedia-USEast"
	"appInsightsName" = "Azure Sky Media"
	"appInsightsRegion" = "South Central US"
}

$regionLocation = "Central US"

$resourceGroupName = ($appName + "-US.Central")

$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
if (!$resourceGroup)
{
	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
}
New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFile -TemplateParameterObject $templateParameters