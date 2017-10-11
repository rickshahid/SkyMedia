#(Get-Module Azure -ListAvailable).Version (v4.4.0 - September 2017)
#Login-AzureRmAccount

#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Resources).ResourceTypes | Where-Object ResourceTypeName -eq deployments).ApiVersions

# Data Tier
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Storage).ResourceTypes | Where-Object ResourceTypeName -eq storageAccounts).ApiVersions
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.DocumentDB).ResourceTypes | Where-Object ResourceTypeName -eq databaseAccounts).ApiVersions

# App Tier
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Batch).ResourceTypes | Where-Object ResourceTypeName -eq batchAccounts).ApiVersions
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Media).ResourceTypes | Where-Object ResourceTypeName -eq mediaServices).ApiVersions
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Cache).ResourceTypes | Where-Object ResourceTypeName -eq redis).ApiVersions
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Logic).ResourceTypes | Where-Object ResourceTypeName -eq workflows).ApiVersions
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Insights).ResourceTypes | Where-Object ResourceTypeName -eq components).ApiVersions
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Insights).ResourceTypes | Where-Object ResourceTypeName -eq autoscaleSettings).ApiVersions

# Web Tier
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Web).ResourceTypes | Where-Object ResourceTypeName -eq serverFarms).ApiVersions
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Web).ResourceTypes | Where-Object ResourceTypeName -eq sites).ApiVersions
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Cdn).ResourceTypes | Where-Object ResourceTypeName -eq profiles).ApiVersions
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Network).ResourceTypes | Where-Object ResourceTypeName -eq trafficManagerProfiles).ApiVersions

$appName = "SkyMedia";

$templateFileDataTier = ($PSScriptRoot + "\Template.DataTier.json")
$templateFileAppTier = ($PSScriptRoot + "\Template.AppTier.json")
$templateFileWebTier = ($PSScriptRoot + "\Template.WebTier.json")

$regionLocation = "West US"

$resourceGroupName = ($appName + "-USWest")

$templateParametersData = @{
	"storageAccountName" = "SkyMediaUSWest";
	"databaseAccountName" = "SkyMedia";
}

#$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
#if (!$resourceGroup)
#{
#	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
#}
#New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFileDataTier -TemplateParameterObject $templateParametersData

$templateParametersApp = $templateParametersData.Clone()
$templateParametersApp.Add("batchAccountName", "SkyMedia")
$templateParametersApp.Add("mediaAccountName", "SkyUSWest")
$templateParametersApp.Add("cacheAccountName", "SkyMedia-USWest")
$templateParametersApp.Add("cacheServiceTier", "Standard")
$templateParametersApp.Add("cacheServiceSize", "C0")
$templateParametersApp.Add("functionAppName", "SkyMedia-Functions-USWest")
$templateParametersApp.Add("logicAppName", "SkyMedia")

#$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
#if (!$resourceGroup)
#{
#	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
#}
#New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFileAppTier -TemplateParameterObject $templateParametersApp

$templateParametersWeb = $templateParametersApp.Clone()
$templateParametersWeb.Add("appHostingPlanName", "SkyMedia-USWest")
$templateParametersWeb.Add("appServicePlanTier", "Standard")
$templateParametersWeb.Add("appServicePlanNodeSize", "S1")
$templateParametersWeb.Add("appServicePlanNodeCountMinimum", 2)
$templateParametersWeb.Add("appServicePlanNodeCountMaximum", 5)
$templateParametersWeb.Add("webSiteName", "SkyMedia-USWest")

$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
if (!$resourceGroup)
{
	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
}
New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFileWebTier -TemplateParameterObject $templateParametersWeb

$regionLocation = "East US 2"

$resourceGroupName = ($appName + "-USEast")

$templateParametersData = @{
	"storageAccountName" = "SkyMediaUSEast";
	"databaseAccountName" = "---";
}

#$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
#if (!$resourceGroup)
#{
#	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
#}
#New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFileDataTier -TemplateParameterObject $templateParametersData

$templateParametersApp = $templateParametersData.Clone()
$templateParametersApp.Add("batchAccountName", "SkyMedia")
$templateParametersApp.Add("mediaAccountName", "SkyUSEast")
$templateParametersApp.Add("cacheAccountName", "SkyMedia-USEast")
$templateParametersApp.Add("cacheServiceTier", "Standard")
$templateParametersApp.Add("cacheServiceSize", "C0")
$templateParametersApp.Add("functionAppName", "---")
$templateParametersApp.Add("logicAppName", "---")

#$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
#if (!$resourceGroup)
#{
#	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
#}
#New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFileAppTier -TemplateParameterObject $templateParametersApp

$templateParametersWeb = $templateParametersApp.Clone()
$templateParametersWeb.Add("appHostingPlanName", "SkyMedia-USEast")
$templateParametersWeb.Add("appServicePlanTier", "Standard")
$templateParametersWeb.Add("appServicePlanNodeSize", "S1")
$templateParametersWeb.Add("appServicePlanNodeCountMinimum", 2)
$templateParametersWeb.Add("appServicePlanNodeCountMaximum", 5)
$templateParametersWeb.Add("webSiteName", "SkyMedia-USEast")

$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
if (!$resourceGroup)
{
	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
}
New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFileWebTier -TemplateParameterObject $templateParametersWeb

$regionLocation = "South Central US"

$resourceGroupName = ($appName + "-USCentral")

$templateParametersApp = $templateParametersData.Clone()

#$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
#if (!$resourceGroup)
#{
#	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
#}
#New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFileAppTier -TemplateParameterObject $templateParametersApp