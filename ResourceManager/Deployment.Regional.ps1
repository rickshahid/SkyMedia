# Get-Module AzureRM -ListAvailable | Select-Object -Property Name, Version, Path
# Install-Module -Name AzureRM -Repository PSGallery -Force
# Uninstall-Module -Name AzureRM
# v6.10.0 - October 2018

# Connect-AzureRmAccount

# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Storage).ResourceTypes | Where-Object ResourceTypeName -eq storageAccounts).ApiVersions
# ((Get-AzureRmResourceProvider -ProviderNamespace Microsoft.Web).ResourceTypes | Where-Object ResourceTypeName -eq serverFarms).ApiVersions

$appName = "Sky.Media"

$templateFile = $PSScriptRoot + "\Template.Regional.json"

$regionLocation = "West US"

$resourceGroupName = $appName + "-US.West"

$pfxCertificateFile = "C:\Solutions\AzureSkyMedia\DigiCert\wildcard_skymedia_tv.pfx"
$pfxCertificateBytes = [System.IO.File]::ReadAllBytes($pfxCertificateFile)
$pfxCertificateBase64 = [System.Convert]::ToBase64String($pfxCertificateBytes)
$pfxCertificatePassword = "amstsp.Net"

$templateParameters = @{
	"storageServiceName" = "SkyMediaUSWest"
	"storageServiceHttpsOnly" = $true
	"storageServiceEncryptionEnabled" = $true
	"appServicePlanName" = "SkyMediaUSWest"
	"appServicePlanTier" = "Standard"
	"appServicePlanNodeSize" = "S1"
	"appServicePlanNodeCountMinimum" = 2
	"appServicePlanNodeCountMaximum" = 5
	"webAppName" = "SkyMedia-USWest-Web"
	"webAppHostNames" = "www.skymedia.tv", "live.skymedia.tv", "render.skymedia.tv"
	"webAppPfxCertificateBase64" = $pfxCertificateBase64
	"webAppPfxCertificatePassword" = $pfxCertificatePassword
	"functionAppName" = "SkyMedia-USWest-Function"
	"functionIngestSchedule" = "0 0 0 * * *"
	"functionPublishUrl" = "https://skymedia-uswest-function.azurewebsites.net/api/MediaPublishHttpPost?code=HKJHr2TJhGAzBXivyvyyJCAY8juwWNRJeydovdXadKagVegCOECzPw=="
	"globalServicesResourceGroup" = $appName + "-US.Central"
	"directoryTenantId" = "7fe6884f-bc15-434b-902d-90f9252495f8"
	"directoryIssuerUrl" = "https://login.microsoftonline.com/{0}/v2.0/"
	"directoryDiscoveryPath" = ".well-known/openid-configuration"
	"directoryPolicyIdSignUpIn" = "B2C_1_SignUpIn"
	"directoryClientId" = "66caa1db-0ccb-46a6-a0fd-44ff62e344d5"
	"directoryClientSecret" = "ZX{liJ5As1[EL6DY"
	"directoryClientIdStaging" = "2f0b5e56-a49d-457a-b603-4e9bd694648b"
	"directoryClientSecretStaging" = "4H#/M:7|uQo%A13n"
	"databaseServiceName" = "SkyMedia"
	"databaseServiceRegions" = "West US;East US"
	"databaseIdentifier" = "Media"
	"searchServiceName" = "SkyMedia"
	"searchAdminKey" = "D4DF45727ED82D97B54A27E72D1032C3"
	"searchQueryKey" = "2DA38B638A6107194C59A9814F0C0371"
	"appRegionName" = "US West"
	"appInsightsName" = "Azure Sky Media"
	"twilioAccountId" = "ACdfe3eb7a775c5874dd3d89b70bfc9ee9"
	"twilioAccountToken" = "dc9de4c840e1cd74495a4d451c261263"
	"twilioMessageFrom" = "+13109058223"
}

$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
if (!$resourceGroup)
{
	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
}
New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFile -TemplateParameterObject $templateParameters 

$regionLocation = "East US"

$resourceGroupName = $appName + "-US.East"

$templateParameters = @{
	"storageServiceName" = "SkyMediaUSEast"
	"storageServiceHttpsOnly" = $true
	"storageServiceEncryptionEnabled" = $true
	"appServicePlanName" = "SkyMediaUSEast"
	"appServicePlanTier" = "Standard"
	"appServicePlanNodeSize" = "S1"
	"appServicePlanNodeCountMinimum" = 2
	"appServicePlanNodeCountMaximum" = 5
	"webAppName" = "SkyMedia-USEast-Web"
	"webAppHostNames" = "www.skymedia.tv", "live.skymedia.tv", "render.skymedia.tv"
	"webAppPfxCertificateBase64" = $pfxCertificateBase64
	"webAppPfxCertificatePassword" = $pfxCertificatePassword
	"functionAppName" = "SkyMedia-USEast-Function"
	"functionIngestSchedule" = ""
	"functionPublishUrl" = "https://skymedia-useast-function.azurewebsites.net/api/MediaPublishHttpPost?code=Pz4rLomqV3zjXk5FqaFyi6J9JdjuRDYxRl7VTUKEG9lOsCHXeXC0Hw=="
	"globalServicesResourceGroup" = $appName + "-US.Central"
	"directoryTenantId" = "7fe6884f-bc15-434b-902d-90f9252495f8"
	"directoryIssuerUrl" = "https://login.microsoftonline.com/{0}/v2.0/"
	"directoryDiscoveryPath" = ".well-known/openid-configuration"
	"directoryPolicyIdSignUpIn" = "B2C_1_SignUpIn"
	"directoryClientId" = "66caa1db-0ccb-46a6-a0fd-44ff62e344d5"
	"directoryClientSecret" = "ZX{liJ5As1[EL6DY"
	"directoryClientIdStaging" = "2f0b5e56-a49d-457a-b603-4e9bd694648b"
	"directoryClientSecretStaging" = "4H#/M:7|uQo%A13n"
	"databaseServiceName" = "SkyMedia"
	"databaseServiceRegions" = "East US;West US"
	"databaseIdentifier" = "Media"
	"searchServiceName" = "SkyMedia"
	"searchAdminKey" = "D4DF45727ED82D97B54A27E72D1032C3"
	"searchQueryKey" = "2DA38B638A6107194C59A9814F0C0371"
	"appRegionName" = "US East"
	"appInsightsName" = "Azure Sky Media"
	"twilioAccountId" = "ACdfe3eb7a775c5874dd3d89b70bfc9ee9"
	"twilioAccountToken" = "dc9de4c840e1cd74495a4d451c261263"
	"twilioMessageFrom" = "+13109058223"
}

$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore
if (!$resourceGroup)
{
	New-AzureRmResourceGroup -Name $resourceGroupName -Location $regionLocation
}
New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $templateFile -TemplateParameterObject $templateParameters