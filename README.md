# Azure Sky Media

Welcome!!!! This repository contains the <a href="https://azure.microsoft.com/en-us/solutions/serverless/" target="_blank">Azure serverless</a> media solution that is currently deployed at www.skymedia.tv. The solution is based upon the core <a href="http://azure.microsoft.com/services/media-services/" target="_blank">Azure Media Services</a> capabilities combined with several other Azure platform services.

As an example illustration, the screenshot below is an introductory media stream that is incorporated within a multi-region <a href="http://azure.microsoft.com/services/app-service/web/" target="_blank">Azure Web App</a> via <a href="http://azure.microsoft.com/services/media-services/media-player/" target="_blank">Azure Media Player</a> and <a href="http://azure.microsoft.com/services/cdn/" target="_blank">Azure Content Delivery Network</a> integration. Both on-demand and live video content can be adaptively streamed and globally consumed across a wide variety of modern devices and web browsers.

![](https://skymedia.azureedge.net/docs/01.04-ApplicationIntroduction.png)

Refer to http://github.com/RickShahid/SkyMedia/wiki for additional screenshots of key application functionality, including

* Multi-tenant, self-service account registration and profile management, including user, media and storage accounts

* Secure content upload, storage and processing via encoding, indexing, dynamic encryption and dynamic packaging

* Discover and extract actionable insights from your media content via integrated video / audio intelligence services

The following architecture overview diagram depicts the solution sample that is deployed at http://www.skymedia.tv

![](https://skymedia.azureedge.net/docs/02.04-SolutionArchitecture.png)

To deploy this media solution sample into your Azure subscription, leverage the "Deploy to Azure" template buttons below:

<table>
  <tr>
    <td>
      <b>Global Services Template</b>
    </td>
    <td>
      <a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FRickShahid%2FSkyMedia%2Fmaster%2FResourceTemplates%2FTemplate.Global.json" title="Deploy Global Services" target="_blank"><img src="http://azuredeploy.net/deploybutton.png"></a>
    </td>
  </tr>
  <tr>
    <td>
      <b>Regional Services Template</b>
    </td>
    <td>
      <a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FRickShahid%2FSkyMedia%2Fmaster%2FResourceTemplates%2FTemplate.Regional.json" title="Deploy Regional Services" target="_blank"><img src="http://azuredeploy.net/deploybutton.png"></a>
    </td>
  </tr>
</table>

The following Azure serverless platform services were integrated to create this sample media solution:

* **Azure Active Directory (B2C)** - http://azure.microsoft.com/services/active-directory-b2c/

* **Azure Key Vault** - http://azure.microsoft.com/services/key-vault/

* **Azure Storage** - http://azure.microsoft.com/services/storage/

  * **Data Box** - http://azure.microsoft.com/services/storage/databox/

* **Azure Cosmos DB** - http://azure.microsoft.com/services/cosmos-db/

* **Azure Search** - http://azure.microsoft.com/services/search/

  * **Cognitive Search** - http://docs.microsoft.com/azure/search/cognitive-search-concept-intro

* **Azure Functions** - http://azure.microsoft.com/services/functions/

  * **Durable Functions** - http://docs.microsoft.com/azure/azure-functions/durable/

* **Azure Event Grid** - http://azure.microsoft.com/services/event-grid/

  * **Media Events** - http://docs.microsoft.com/azure/media-services/latest/media-services-event-schemas

* **Azure Media Services** - http://azure.microsoft.com/services/media-services/

  * **Player** - http://azure.microsoft.com/services/media-services/media-player/

  * **Streaming** - http://azure.microsoft.com/services/media-services/live-on-demand/

  * **Protection** - http://azure.microsoft.com/services/media-services/content-protection/

  * **Encoding** - http://azure.microsoft.com/services/media-services/encoding/
  
  * **Indexing** - http://azure.microsoft.com/services/media-services/video-indexer/

* **Azure Cognitive Services** - http://azure.microsoft.com/services/cognitive-services/

* **Azure Content Delivery Network** - http://azure.microsoft.com/services/cdn/

* **Azure Traffic Manager** - http://azure.microsoft.com/services/traffic-manager/

* **Azure App Service** - http://azure.microsoft.com/services/app-service/

* **Azure Bot Service** - http://azure.microsoft.com/services/bot-service/

* **Azure Monitor** - http://azure.microsoft.com/services/monitor/

  * **App Insights** - http://docs.microsoft.com/azure/azure-monitor/app/app-insights-overview

* **Azure DevOps** - http://azure.microsoft.com/services/devops/

If you have any issues or suggestions, please let me know.

Thanks.

Rick Shahid

rick.shahid@microsoft.com

Azure Architect & Developer

Media & Entertainment Solutions
