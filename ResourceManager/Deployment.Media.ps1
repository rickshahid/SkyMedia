# Install-Module -Name AzureRM -Repository PSGallery -Force
# Update-Module -Name AzureRM
# Get-Module AzureRM -ListAvailable | Select-Object -Property Name, Version, Path
# v5.4.1 - February 2018

# Connect-AzureRmAccount

# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Storage).ResourceTypes | Where-Object ResourceTypeName -eq storageAccounts).ApiVersions
# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Batch).ResourceTypes | Where-Object ResourceTypeName -eq batchAccounts).ApiVersions
# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Media).ResourceTypes | Where-Object ResourceTypeName -eq mediaServices).ApiVersions
# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Cdn).ResourceTypes | Where-Object ResourceTypeName -eq profiles).ApiVersions

$appName = "Azure.Sky.Media"

$templateFile = ($PSScriptRoot + "\Template.Media.json")

$templateParameters = @{
	"storageServiceName" = "SkyMediaUSWest"
	"batchServiceName" = "SkyMedia"
	"mediaServiceName" = "USWest"
    "mediaRegionName" = "West US"
	"contentDeliveryProfileName" = "SkyMedia-USWest"
	"contentDeliveryProfileTier" = "Standard_Akamai"
}

$regionLocation = "West US"

$resourceGroupName = ($appName + "-US.West")

$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
if (!$resourceGroup)
{
	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
}
New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFile -TemplateParameterObject $templateParameters

$regionLocation = "East US"

$resourceGroupName = ($appName + "-US.East")

$templateParameters = @{
	"storageServiceName" = "SkyMediaUSEast"
	"batchServiceName" = "SkyMedia"
	"mediaServiceName" = "USEast"
    "mediaRegionName" = "East US 2"
	"contentDeliveryProfileName" = "SkyMedia-USEast"
	"contentDeliveryProfileTier" = "Standard_Akamai"
}

$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
if (!$resourceGroup)
{
	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
}
New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFile -TemplateParameterObject $templateParameters