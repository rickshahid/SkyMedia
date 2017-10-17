# (Get-Module Azure -ListAvailable).Version (v4.4.1 - October 2017)
# Login-AzureRmAccount

# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Insights).ResourceTypes | Where-Object ResourceTypeName -eq components).ApiVersions
# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Network).ResourceTypes | Where-Object ResourceTypeName -eq trafficManagerProfiles).ApiVersions

$appName = "Azure.Sky.Media"

$templateFile = ($PSScriptRoot + "\Template.Global.json")

$resourceGroupName = ($appName + "-US.Central")

$regionLocation = "Central US"

$templateParameters = @{
	"appInsightsName" = "Azure Sky Media"
	"appInsightsRegion" = "South Central US"
	"trafficManagerSubdomainName" = "SkyMedia"
	"trafficManagerTimeToLive" = 30
}

$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
if (!$resourceGroup)
{
	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
}
New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFile -TemplateParameterObject $templateParameters