# Install-Module -Name AzureRM -Repository PSGallery -Force
# Uninstall-Module -Name AzureRM
# Update-Module -Name AzureRM
# Get-Module AzureRM -ListAvailable | Select-Object -Property Name, Version, Path
# v5.5.0 - March 2018

# Connect-AzureRmAccount

# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Web).ResourceTypes | Where-Object ResourceTypeName -eq serverFarms).ApiVersions
# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Web).ResourceTypes | Where-Object ResourceTypeName -eq sites).ApiVersions

$appName = "Azure.Sky.Media"

$templateFile = ($PSScriptRoot + "\Template.Regional.json")

$templateParameters = @{
	"globalServicesResourceGroup" = ($appName + "-US.Central")
	"directoryB2bClientId" = "64155701-3cc4-4ef9-9d21-dbc7920d8c8a"
	"directoryB2bClientSecret" = "GDLlpsvt7V8CWJXVZlptEuEiWTRcDSLvT65/9G/bh8Y="
	"directoryB2bClientIdStaging" = "1646960c-9354-49dd-a72d-0f921ff0d1c3"
	"directoryB2bClientSecretStaging" = "qP+jcODxU1Vd8V+KWhmmJqlMuq7vwI3NsSfT+OuNPTE="
	"directoryB2cClientId" = "66caa1db-0ccb-46a6-a0fd-44ff62e344d5"
	"directoryB2cClientSecret" = "ZX{liJ5As1[EL6DY"
	"directoryB2cClientIdStaging" = "2f0b5e56-a49d-457a-b603-4e9bd694648b"
	"directoryB2cClientSecretStaging" = "4H#/M:7|uQo%A13n"
	"appSubscriptionId" = "3d07cfbc-17aa-41b4-baa1-488fef85a1d3"
	"storageServiceName" = "SkyMedia"
	"databaseServiceName" = "SkyMedia"
    "databaseRegionsRead" = "West US;South Central US;East US"
	"databaseIdentifier" = "Media"
	"appServicePlanName" = "SkyMedia-USWest"
	"appServicePlanTier" = "Standard"
	"appServicePlanNodeSize" = "S1"
	"appServicePlanNodeCountMinimum" = 2
	"appServicePlanNodeCountMaximum" = 5
	"appSubdomainName" = "SkyMedia-USWest"
	"appRegionName" = "US West"
    "appInsightsName" = "Azure Sky Media"
}

$regionLocation = "West US"

$resourceGroupName = ($appName + "-US.West")

$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
if (!$resourceGroup)
{
	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
}
New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFile -TemplateParameterObject $templateParameters 

$templateParameters = @{
	"globalServicesResourceGroup" = ($appName + "-US.Central")
	"directoryB2bClientId" = "9a773e4c-d1d6-4fdf-adcb-df195c5f3c03"
	"directoryB2bClientSecret" = "a7pRhpQf/OFkaF4Q4fSpjdclgTPUx5wPmkGk0pS8xwQ="
	"directoryB2bClientIdStaging" = "2c96d59b-510e-44b7-bed4-a3b527c66825"
	"directoryB2bClientSecretStaging" = "VBUI1dYVlYny9tZNSh3FQlux73YjpVH0+vYnAnlKtuY="
	"directoryB2cClientId" = "66caa1db-0ccb-46a6-a0fd-44ff62e344d5"
	"directoryB2cClientSecret" = "ZX{liJ5As1[EL6DY"
	"directoryB2cClientIdStaging" = "2f0b5e56-a49d-457a-b603-4e9bd694648b"
	"directoryB2cClientSecretStaging" = "4H#/M:7|uQo%A13n"
	"appSubscriptionId" = "3d07cfbc-17aa-41b4-baa1-488fef85a1d3"
	"storageServiceName" = "SkyMedia"
	"databaseServiceName" = "SkyMedia"
    "databaseRegionsRead" = "East US;South Central US;West US"
	"databaseIdentifier" = "Media"
	"appServicePlanName" = "SkyMedia-USEast"
	"appServicePlanTier" = "Standard"
	"appServicePlanNodeSize" = "S1"
	"appServicePlanNodeCountMinimum" = 2
	"appServicePlanNodeCountMaximum" = 5
	"appSubdomainName" = "SkyMedia-USEast"
	"appRegionName" = "US East"
    "appInsightsName" = "Azure Sky Media"
}

$regionLocation = "East US"

$resourceGroupName = ($appName + "-US.East")

$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
if (!$resourceGroup)
{
	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
}
New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFile -TemplateParameterObject $templateParameters