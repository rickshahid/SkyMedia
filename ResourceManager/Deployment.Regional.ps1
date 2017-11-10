# (Get-Module Azure -ListAvailable).Version (v4.4.1 - October 2017)
# Login-AzureRmAccount

# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Cache).ResourceTypes | Where-Object ResourceTypeName -eq redis).ApiVersions
# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Web).ResourceTypes | Where-Object ResourceTypeName -eq serverFarms).ApiVersions
# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Web).ResourceTypes | Where-Object ResourceTypeName -eq sites).ApiVersions

$appName = "Azure.Sky.Media"

$templateFile = ($PSScriptRoot + "\Template.Regional.json")

$resourceGroupName = ($appName + "-US.West")

$regionName = "US West"
$regionLocation = "West US"

$templateParameters = @{
	"globalServiceResourceGroup" = ($appName + "-US.Central")
	"directoryB2bClientId" = "9a773e4c-d1d6-4fdf-adcb-df195c5f3c03"
	"directoryB2bClientSecret" = "a7pRhpQf/OFkaF4Q4fSpjdclgTPUx5wPmkGk0pS8xwQ="
	"directoryB2bClientIdStaging" = "2c96d59b-510e-44b7-bed4-a3b527c66825"
	"directoryB2bClientSecretStaging" = "VBUI1dYVlYny9tZNSh3FQlux73YjpVH0+vYnAnlKtuY="
	"directoryB2cClientId" = "66caa1db-0ccb-46a6-a0fd-44ff62e344d5"
	"directoryB2cClientSecret" = "ZX{liJ5As1[EL6DY"
	"directoryB2cClientIdStaging" = "2f0b5e56-a49d-457a-b603-4e9bd694648b"
	"directoryB2cClientSecretStaging" = "4H#/M:7|uQo%A13n"
	"storageServiceName" = "SkyMedia"
	"databaseServiceName" = "SkyMedia"
	"databaseIdentifier" = "Media"
	"cacheServiceName" = "SkyMedia-USWest"
	"cacheServiceTier" = "Standard"
    "cacheServiceSize" = 0 # 250 MB
	"appServicePlanName" = "SkyMedia-USWest"
	"appServicePlanTier" = "Standard"
	"appServicePlanNodeSize" = "S1"
	"appServicePlanNodeCountMinimum" = 2
	"appServicePlanNodeCountMaximum" = 5
	"appSubdomainName" = "SkyMedia-USWest"
	"appRegionName" = "US West"
	"appSubscriptionId" = "3d07cfbc-17aa-41b4-baa1-488fef85a1d3"
    "appInsightsName" = "Azure Sky Media"
}

$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
if (!$resourceGroup)
{
	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
}
New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFile -TemplateParameterObject $templateParameters 

$resourceGroupName = ($appName + "-US.East")

$regionName = "US East"
$regionLocation = "East US"

$templateParameters = @{
	"globalServiceResourceGroup" = ($appName + "-US.Central")
	"directoryB2bClientId" = "9a773e4c-d1d6-4fdf-adcb-df195c5f3c03"
	"directoryB2bClientSecret" = "a7pRhpQf/OFkaF4Q4fSpjdclgTPUx5wPmkGk0pS8xwQ="
	"directoryB2bClientIdStaging" = "2c96d59b-510e-44b7-bed4-a3b527c66825"
	"directoryB2bClientSecretStaging" = "VBUI1dYVlYny9tZNSh3FQlux73YjpVH0+vYnAnlKtuY="
	"directoryB2cClientId" = "66caa1db-0ccb-46a6-a0fd-44ff62e344d5"
	"directoryB2cClientSecret" = "ZX{liJ5As1[EL6DY"
	"directoryB2cClientIdStaging" = "2f0b5e56-a49d-457a-b603-4e9bd694648b"
	"directoryB2cClientSecretStaging" = "4H#/M:7|uQo%A13n"
	"storageServiceName" = "SkyMedia"
	"databaseServiceName" = "SkyMedia"
	"databaseIdentifier" = "Media"
	"cacheServiceName" = "SkyMedia-USEast"
	"cacheServiceTier" = "Standard"
    "cacheServiceSize" = 0 # 250 MB
	"appServicePlanName" = "SkyMedia-USEast"
	"appServicePlanTier" = "Standard"
	"appServicePlanNodeSize" = "S1"
	"appServicePlanNodeCountMinimum" = 2
	"appServicePlanNodeCountMaximum" = 5
	"appSubdomainName" = "SkyMedia-USEast"
	"appRegionName" = "US East"
	"appSubscriptionId" = "3d07cfbc-17aa-41b4-baa1-488fef85a1d3"
    "appInsightsName" = "Azure Sky Media"
}

$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
if (!$resourceGroup)
{
	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
}
New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFile -TemplateParameterObject $templateParameters