# Get-Module AzureRM -ListAvailable | Select-Object -Property Name, Version, Path
# Install-Module -Name AzureRM -Repository PSGallery -Force
# Uninstall-Module -Name AzureRM
# v6.10.0 - October 2018

# Connect-AzureRmAccount

# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Network).ResourceTypes | Where-Object ResourceTypeName -eq trafficManagerProfiles).ApiVersions

$appName = "Sky.Media"

$templateFile = $PSScriptRoot + "\Template.Global.Traffic.json"

$regionLocation = "Central US"

$resourceGroupName = $appName + "-US.Central"

$templateParameters = @{
	"trafficManagerProfileName" = "SkyMedia"
	"trafficManagerRoutingMethod" = "Performance"
	"trafficManagerRoutingTimeToLive" = 30 # Seconds
	"trafficManagerEndpointGroupNames" = ($appName + "-US.West"), ($appName + "-US.East")
	"trafficManagerEndpointNames" = "SkyMedia-USWest-Web", "SkyMedia-USEast-Web"
}

$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
if (!$resourceGroup)
{
	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
}
New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFile -TemplateParameterObject $templateParameters