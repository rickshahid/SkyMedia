# Get-Module AzureRM -ListAvailable | Select-Object -Property Name, Version, Path
# Install-Module -Name AzureRM -Repository PSGallery -Force
# Uninstall-Module -Name AzureRM
# v6.10.0 - October 2018

# Connect-AzureRmAccount

# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Storage).ResourceTypes | Where-Object ResourceTypeName -eq storageAccounts).ApiVersions
# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Network).ResourceTypes | Where-Object ResourceTypeName -eq virtualNetworks).ApiVersions
# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Compute).ResourceTypes | Where-Object ResourceTypeName -eq virtualMachines).ApiVersions

$appName = "Sky.Media"

$templateFile = $PSScriptRoot + "\Template.Media.Render.json"

$regionLocation = "West US"

$resourceGroupName = $appName + "-US.West-Render"

$templateParameters = @{
	"storageServiceName" = "SkyMediaUSWestRender"
	"storageServiceHttpsOnly" = $true
	"storageServiceEncryptionEnabled" = $true
	"virtualNetworkName" = "SkyMediaUSWestRender"
	"virtualNetworkAddresses" = "10.0.0.0/24"
	"virtualNetworkPublicAddressName" = "vFXT-IP-USWest"
	"virtualNetworkSecurityGroupName" = "vFXT-NSG-USWest"
	"virtualNetworkInterfaceName" = "vFXT-NIC-USWest"
	"virtualMachineName" = "vFXT-USWest"
	"virtualMachineSize" = "Standard_D16s_v3"
	"virtualMachineAdminName" = "sysadmin"
	"virtualMachineAdminPassword" = "Passw0rd"
}

$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
if (!$resourceGroup)
{
	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
}
New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFile -TemplateParameterObject $templateParameters

$regionLocation = "East US"

$resourceGroupName = $appName + "-US.East-Render"

$templateParameters = @{
	"storageServiceName" = "SkyMediaUSEastRender"
	"storageServiceHttpsOnly" = $true
	"storageServiceEncryptionEnabled" = $true
	"virtualNetworkName" = "SkyMediaUSEastRender"
	"virtualNetworkAddresses" = "10.0.0.0/24"
	"virtualNetworkPublicAddressName" = "vFXT-IP-USEast"
	"virtualNetworkSecurityGroupName" = "vFXT-NSG-USEast"
	"virtualNetworkInterfaceName" = "vFXT-NIC-USEast"
	"virtualMachineName" = "vFXT-USEast"
	"virtualMachineSize" = "Standard_D16s_v3"
	"virtualMachineAdminName" = "sysadmin"
	"virtualMachineAdminPassword" = "Passw0rd"
}

$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
if (!$resourceGroup)
{
	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
}
New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFile -TemplateParameterObject $templateParameters