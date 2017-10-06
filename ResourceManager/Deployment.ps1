#(Get-Module Azure -ListAvailable).Version (v4.4.0 - September 2017)
#Login-AzureRmAccount

# Data Tier
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Storage).ResourceTypes | Where-Object ResourceTypeName -eq storageAccounts).ApiVersions
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.DocumentDB).ResourceTypes | Where-Object ResourceTypeName -eq databaseAccounts).ApiVersions

# App Tier
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Batch).ResourceTypes | Where-Object ResourceTypeName -eq batchAccounts).ApiVersions
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Media).ResourceTypes | Where-Object ResourceTypeName -eq mediaServices).ApiVersions
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Cache).ResourceTypes | Where-Object ResourceTypeName -eq redis).ApiVersions
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Web).ResourceTypes | Where-Object ResourceTypeName -eq serverFarms).ApiVersions
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Logic).ResourceTypes | Where-Object ResourceTypeName -eq workflows).ApiVersions
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Insights).ResourceTypes | Where-Object ResourceTypeName -eq components).ApiVersions
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Insights).ResourceTypes | Where-Object ResourceTypeName -eq autoscaleSettings).ApiVersions

# Web Tier
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Web).ResourceTypes | Where-Object ResourceTypeName -eq sites).ApiVersions
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Cdn).ResourceTypes | Where-Object ResourceTypeName -eq profiles).ApiVersions
#((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Network).ResourceTypes | Where-Object ResourceTypeName -eq trafficManagerProfiles).ApiVersions

$appName = "SkyMedia"
$appDomain = "skymedia.io"

$regionName = "USWest"
$regionLocation = "West US"

$templateFileDataTier = ($PSScriptRoot + "\Template.DataTier.json")
$templateFileAppTier = ($PSScriptRoot + "\Template.AppTier.json")
$templateFileWebTier = ($PSScriptRoot + "\Template.WebTier.json")

$templateParameters =  @{ "appName" = $appName; "appDomain" = $appDomain; "regionName" = $regionName; }

#$resourceGroupName = ($appName + "-Data-" + $regionName)
#$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
#if (!$resourceGroup)
#{
#	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
#}
#New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFileDataTier -TemplateParameterObject $templateParameters

#$resourceGroupName = ($appName + "-App-" + $regionName)
#$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
#if (!$resourceGroup)
#{
#	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
#}
#$templateParametersApp = $templateParameters.Clone()
#$templateParametersApp.Add("regionCentral", $false)
#$templateParametersApp.Add("cacheServiceTier", "Standard")
#$templateParametersApp.Add("cacheServiceSize", "C0")
#$templateParametersApp.Add("appServicePlanTier", "Standard")
#$templateParametersApp.Add("appServicePlanNodeSize", "S1")
#$templateParametersApp.Add("appServicePlanNodeCountMinimum", 2)
#$templateParametersApp.Add("appServicePlanNodeCountMaximum", 5)
#New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFileAppTier -TemplateParameterObject $templateParametersApp

#$resourceGroupName = ($appName + "-Web-" + $regionName)
#$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
#if (!$resourceGroup)
#{
#	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
#}
#New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFileWebTier -TemplateParameterObject $templateParameters

$regionName = "USEast"
$regionLocation = "East US 2"

$templateParameters =  @{ "appName" = $appName; "appDomain" = $appDomain; "regionName" = $regionName; }

#$resourceGroupName = ($appName + "-Data-" + $regionName)
#$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
#if (!$resourceGroup)
#{
#	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
#}
#New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFileDataTier -TemplateParameterObject $templateParameters

#$resourceGroupName = ($appName + "-App-" + $regionName)
#$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
#if (!$resourceGroup)
#{
#	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
#}
#$templateParametersApp = $templateParameters.Clone()
#$templateParametersApp.Add("regionCentral", $false)
#$templateParametersApp.Add("cacheServiceTier", "Standard")
#$templateParametersApp.Add("cacheServiceSize", "C0")
#$templateParametersApp.Add("appServicePlanTier", "Standard")
#$templateParametersApp.Add("appServicePlanNodeSize", "S1")
#$templateParametersApp.Add("appServicePlanNodeCountMinimum", 2)
#$templateParametersApp.Add("appServicePlanNodeCountMaximum", 5)
#New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFileAppTier -TemplateParameterObject $templateParametersApp

#$resourceGroupName = ($appName + "-Web-" + $regionName)
#$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
#if (!$resourceGroup)
#{
#	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
#}
#New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFileWebTier -TemplateParameterObject $templateParameters

$regionName = "USCentral"
$regionLocation = "South Central US"

$templateParameters =  @{ "appName" = $appName; "appDomain" = $appDomain; "regionName" = $regionName; }

#$resourceGroupName = ($appName + "-App-" + $regionName)
#$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
#if (!$resourceGroup)
#{
#	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
#}
#$templateParametersApp = $templateParameters.Clone()
#$templateParametersApp.Add("regionCentral", $true)
#$templateParametersApp.Add("cacheServiceTier", "Standard")
#$templateParametersApp.Add("cacheServiceSize", "C0")
#$templateParametersApp.Add("appServicePlanTier", "Standard")
#$templateParametersApp.Add("appServicePlanNodeSize", "S1")
#$templateParametersApp.Add("appServicePlanNodeCountMinimum", 2)
#$templateParametersApp.Add("appServicePlanNodeCountMaximum", 5)
#New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFileAppTier -TemplateParameterObject $templateParametersApp