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
	"webAppSubdomainName" = "SkyMedia-USWest"
	"webAppRegionName" = "US West"
	"directoryClientId" = "5a5942f5-dfd4-42b4-a82e-37de4cf0ecee"
	"directoryClientSecret" = "n1=Q2u]ZNcC#xV=3"
	"directoryClientIdStaging" = "927f886f-127d-437c-8c09-fc4e95695d62"
	"directoryClientSecretStaging" = "E85xs3rvQ415jIx)"
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
	"webAppSubdomainName" = "SkyMedia-USEast"
	"webAppRegionName" = "US East"
	"directoryClientId" = "5a5942f5-dfd4-42b4-a82e-37de4cf0ecee"
	"directoryClientSecret" = "n1=Q2u]ZNcC#xV=3"
	"directoryClientIdStaging" = "927f886f-127d-437c-8c09-fc4e95695d62"
	"directoryClientSecretStaging" = "E85xs3rvQ415jIx)"
    "appInsightsName" = "Azure Sky Media"
}

$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
if (!$resourceGroup)
{
	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
}
New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFile -TemplateParameterObject $templateParameters