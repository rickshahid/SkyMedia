#(Get-Module Azure -ListAvailable).Version (v4.4.1 - October 2017)
#Login-AzureRmAccount

#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Resources).ResourceTypes | Where-Object ResourceTypeName -eq deployments).ApiVersions

# Data Tier
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Storage).ResourceTypes | Where-Object ResourceTypeName -eq storageAccounts).ApiVersions
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.DocumentDB).ResourceTypes | Where-Object ResourceTypeName -eq databaseAccounts).ApiVersions

# App Tier
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Insights).ResourceTypes | Where-Object ResourceTypeName -eq components).ApiVersions
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Batch).ResourceTypes | Where-Object ResourceTypeName -eq batchAccounts).ApiVersions
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Media).ResourceTypes | Where-Object ResourceTypeName -eq mediaServices).ApiVersions
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Cache).ResourceTypes | Where-Object ResourceTypeName -eq redis).ApiVersions

# Web Tier
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Web).ResourceTypes | Where-Object ResourceTypeName -eq serverFarms).ApiVersions
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Web).ResourceTypes | Where-Object ResourceTypeName -eq sites).ApiVersions
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Cdn).ResourceTypes | Where-Object ResourceTypeName -eq profiles).ApiVersions
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Network).ResourceTypes | Where-Object ResourceTypeName -eq trafficManagerProfiles).ApiVersions
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Insights).ResourceTypes | Where-Object ResourceTypeName -eq autoscaleSettings).ApiVersions

$templateFileDataTier = ($PSScriptRoot + "\Template.DataTier.json")
$templateFileAppTier = ($PSScriptRoot + "\Template.AppTier.json")
$templateFileWebTier = ($PSScriptRoot + "\Template.WebTier.json")

$resourceGroupNameUSWest = "SkyMedia-USWest"
$resourceGroupNameUSEast = "SkyMedia-USEast"
$resourceGroupNameUSCentral = "SkyMedia-USCentral"

$regionLocation = "West US"

$templateParametersData = @{
	"databaseServiceName" = "SkyMedia"
	"databaseDataRegions" = "West US", "East US"
	"mediaStorageName" = "SkyMediaUSWest"
	"mediaStorageRegion" = "West US"
}

#$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupNameUSWest -ErrorAction Ignore
#if (!$resourceGroup)
#{
#	New-AzureRmResourceGroup -Name $resourceGroupNameUSWest -Location $regionLocation
#}
#New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupNameUSWest -TemplateFile $templateFileDataTier -TemplateParameterObject $templateParametersData 

$templateParametersApp = $templateParametersData.Clone()
$templateParametersApp.Add("databaseResourceGroup", $resourceGroupNameUSWest)
$templateParametersApp.Add("batchServiceName", "SkyMedia")
$templateParametersApp.Add("mediaServiceName", "SkyUSWest")
$templateParametersApp.Add("cacheServiceName", "SkyMedia-USWest")

$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupNameUSWest -ErrorAction Ignore
if (!$resourceGroup)
{
	New-AzureRmResourceGroup -Name $resourceGroupNameUSWest -Location $regionLocation
}
New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupNameUSWest -TemplateFile $templateFileAppTier -TemplateParameterObject $templateParametersApp

$templateParametersWeb = $templateParametersApp.Clone()
$templateParametersWeb.Add("appServicePlanName", "SkyMedia-USWest")
$templateParametersWeb.Add("appServicePlanTier", "Standard")
$templateParametersWeb.Add("appServicePlanNodeSize", "S1")
$templateParametersWeb.Add("appServicePlanNodeCountMinimum", 2)
$templateParametersWeb.Add("appServicePlanNodeCountMaximum", 5)
$templateParametersWeb.Add("webSiteName", "SkyMedia-USWest")

#$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupNameUSWest -ErrorAction Ignore
#if (!$resourceGroup)
#{
#	New-AzureRmResourceGroup -Name $resourceGroupNameUSWest -Location $regionLocation
#}
#New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupNameUSWest -TemplateFile $templateFileWebTier -TemplateParameterObject $templateParametersWeb

$regionLocation = "East US"

$templateParametersData = @{
	"mediaStorageName" = "SkyMediaUSEast"
	"mediaStorageRegion" = "East US 2"
}

#$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupNameUSEast -ErrorAction Ignore
#if (!$resourceGroup)
#{
#	New-AzureRmResourceGroup -Name $resourceGroupNameUSEast -Location $regionLocation
#}
#New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupNameUSEast -TemplateFile $templateFileDataTier -TemplateParameterObject $templateParametersData

$templateParametersApp = $templateParametersData.Clone()
$templateParametersApp.Add("databaseResourceGroup", $resourceGroupNameUSWest)
$templateParametersApp.Add("batchServiceName", "SkyMedia")
$templateParametersApp.Add("mediaServiceName", "SkyUSEast")
$templateParametersApp.Add("cacheServiceName", "SkyMedia-USEast")

$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupNameUSEast -ErrorAction Ignore
if (!$resourceGroup)
{
	New-AzureRmResourceGroup -Name $resourceGroupNameUSEast -Location $regionLocation
}
New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupNameUSEast -TemplateFile $templateFileAppTier -TemplateParameterObject $templateParametersApp

$templateParametersWeb = $templateParametersApp.Clone()
$templateParametersWeb.Add("appServicePlanName", "SkyMedia-USEast")
$templateParametersWeb.Add("appServicePlanTier", "Standard")
$templateParametersWeb.Add("appServicePlanNodeSize", "S1")
$templateParametersWeb.Add("appServicePlanNodeCountMinimum", 2)
$templateParametersWeb.Add("appServicePlanNodeCountMaximum", 5)
$templateParametersWeb.Add("webSiteName", "SkyMedia-USEast")

#$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupNameUSEast -ErrorAction Ignore
#if (!$resourceGroup)
#{
#	New-AzureRmResourceGroup -Name $resourceGroupNameUSEast -Location $regionLocation
#}
#New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupNameUSEast -TemplateFile $templateFileWebTier -TemplateParameterObject $templateParametersWeb

$regionLocation = "South Central US"

$templateParametersData = @{
	"storageServiceName" = "SkyMedia"
	"databaseServiceName" = "SkyMedia"
}

#$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupNameUSCentral -ErrorAction Ignore
#if (!$resourceGroup)
#{
#	New-AzureRmResourceGroup -Name $resourceGroupNameUSCentral -Location $regionLocation
#}
#New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupNameUSCentral -TemplateFile $templateFileDataTier -TemplateParameterObject $templateParametersData 

$templateParametersApp = $templateParametersData.Clone()
$templateParametersApp.Add("databaseResourceGroup", $resourceGroupNameUSWest)
$templateParametersApp.Add("appInsightsName", "Azure Sky Media")
$templateParametersApp.Add("appInsightsRegion", $regionLocation)
$templateParametersApp.Add("functionAppName", "SkyMedia")

$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupNameUSCentral -ErrorAction Ignore
if (!$resourceGroup)
{
	New-AzureRmResourceGroup -Name $resourceGroupNameUSCentral -Location $regionLocation
}
New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupNameUSCentral -TemplateFile $templateFileAppTier -TemplateParameterObject $templateParametersApp