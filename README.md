# Azure Sky Media

Welcome! This repository contains the multi-tenant Azure media serverless solution that is deployed at www.skymedia.tv

As an example illustration, the screenshot below is an introductory <a href="http://azure.microsoft.com/services/media-services/" target="_blank">Azure Media Services</a> stream that is delivered to an Azure App Service Web App via <a href="http://azure.microsoft.com/services/media-services/media-player/" target="_blank">Azure Media Player</a> integration. Both on-demand and live video content that is stored and managed in Azure Media Services is adaptively streamed, scaled and globally consumable across a wide range of devices and browsers.

![](https://skymedia.azureedge.net/docs/01.10-ApplicationIntroduction.png)

Refer to http://github.com/RickShahid/SkyMedia/wiki for additional screenshots of key application functionality, including:

* Multi-tenant, self-service account registration and profile management, including user, media and storage accounts

* Secure content upload, storage and processing via encoding, indexing, dynamic encryption and dynamic packaging

* Discover and extract actionable insights from your media content via integrated video / audio intelligence services

The following architecture overview diagram depicts the solution sample deployment at http://www.skymedia.tv

![](https://skymedia.azureedge.net/docs/02.12-SolutionArchitecture.png)

To deploy the parameterized solution sample within your Azure subscription, click the "Deploy to Azure" button below:

<table>
  <tr>
    <td>
      <b>Global Services Template</b>
    </td>
    <td>
      <a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FRickShahid%2FSkyMedia%2Fmaster%2FResourceManager%2FTemplate.Global.json" title="Deploy Global Services" target="_blank"><img src="http://azuredeploy.net/deploybutton.png"></a>
    </td>
    <td>
      <a href="http://armviz.io/#/?load=https%3A%2F%2Fraw.githubusercontent.com%2FRickShahid%2FSkyMedia%2Fmaster%2FResourceManager%2FTemplate.Global.json" title="Visualize Global Services" target="_blank"><img src="http://armviz.io/visualizebutton.png"></a>
    </td>
  </tr>
  <tr>
    <td>
      <b>Regional Services Template</b>
    </td>
    <td>
      <a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FRickShahid%2FSkyMedia%2Fmaster%2FResourceManager%2FTemplate.Regional.json" title="Deploy Regional Services" target="_blank"><img src="http://azuredeploy.net/deploybutton.png"></a>
    </td>
    <td>
      <a href="http://armviz.io/#/?load=https%3A%2F%2Fraw.githubusercontent.com%2FRickShahid%2FSkyMedia%2Fmaster%2FResourceManager%2FTemplate.Regional.json" title="Visualize Regional Services" target="_blank"><img src="http://armviz.io/visualizebutton.png"></a>
    </td>
  </tr>
</table>

The following Azure platform services were leveraged to create this sample media application:

* **Azure Active Directory (B2C)** - http://azure.microsoft.com/services/active-directory-b2c/

* **Azure Key Vault** - http://azure.microsoft.com/services/key-vault/

* **Azure Storage** - http://azure.microsoft.com/services/storage/

  * **Data Box** - http://azure.microsoft.com/services/storage/databox/

* **Azure Cosmos DB** - http://azure.microsoft.com/services/cosmos-db/

* **Azure Logic App** - http://azure.microsoft.com/services/logic-apps/

* **Azure Functions** - http://azure.microsoft.com/services/functions/

* **Azure Event Grid** - http://azure.microsoft.com/services/event-grid/

* **Azure Media Services** - http://azure.microsoft.com/services/media-services/

  * **Media Encoding** - http://azure.microsoft.com/services/media-services/encoding/

  * **Media Streaming** - http://azure.microsoft.com/services/media-services/live-on-demand/

  * **Content Protection** - http://azure.microsoft.com/services/media-services/content-protection/
  
  * **Media Player** - http://azure.microsoft.com/services/media-services/media-player/
  
  * **Video Indexer** - http://azure.microsoft.com/services/media-services/video-indexer/

* **Azure Content Delivery Network** - http://azure.microsoft.com/services/cdn/

* **Azure App Service** - http://azure.microsoft.com/services/app-service/

* **Azure App Insights** - http://azure.microsoft.com/services/application-insights/

* **Azure Traffic Manager** - http://azure.microsoft.com/services/traffic-manager/

* **Azure DevOps** - http://azure.microsoft.com/services/devops/

If you have any issues or suggestions, please let me know.

Thanks.

Rick Shahid

rick.shahid@microsoft.com

Azure Architect & Developer

Media & Entertainment Solutions
