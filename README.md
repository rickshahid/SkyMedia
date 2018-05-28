# Azure Sky Media

Welcome! This repository contains the multi-tenant Azure media solution sample that is deployed at http://www.skymedia.tv

As a sample illustration, the screenshot below is an introductory <a href="http://azure.microsoft.com/services/media-services/" target="_blank">Azure Media Services</a> stream that is playing within an Azure App Service Web App via <a href="http://azure.microsoft.com/services/media-services/media-player/" target="_blank">Azure Media Player</a> integration. On-demand and live video content that is stored and managed in Azure Media Services is adaptively streamed, scaled and globally consumable across a wide range of devices and platforms.

![](http://skymedia.azureedge.net/Snip01.ApplicationWebSite.png)

For additional screenshots of key application views and functionality, refer to http://github.com/RickShahid/SkyMedia/wiki

The following set of media functionality has been integrated and enabled within an Azure ASP.NET Core MVC web app:

* Scalable video (adaptive) streaming to a broad spectrum of devices and platforms (iOS / macOS, Android, Windows)

* Multi-tenant, self-service user account registration and profile management across both storage and media accounts

* Secure content upload, storage and processing (transcoding, indexing, dynamic encryption, dynamic packaging, etc)

* Discover and extract actionable insights from your video content across a wide range of analytic & cognitive services

The following application architecture overview diagram depicts the sample deployment at http://www.skymedia.tv

![](http://skymedia.azureedge.net/Snip02.ApplicationArchitecture.png)

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

* **Team Services** - http://azure.microsoft.com/services/visual-studio-team-services/

* **Active Directory** - http://azure.microsoft.com/services/active-directory/

* **Active Directory B2C** - http://azure.microsoft.com/services/active-directory-b2c/

* **Storage** - http://azure.microsoft.com/services/storage/

* **Cosmos DB** - http://azure.microsoft.com/services/cosmos-db/

* **Media Services** - http://azure.microsoft.com/services/media-services/

  * **Player** - http://azure.microsoft.com/services/media-services/media-player/

  * **Encoding** - http://azure.microsoft.com/services/media-services/encoding/

  * **Indexing** - http://azure.microsoft.com/services/cognitive-services/video-indexer/

  * **Streaming** - http://azure.microsoft.com/services/media-services/live-on-demand/

  * **Protection** - http://azure.microsoft.com/services/media-services/content-protection/

* **Content Delivery Network** - http://azure.microsoft.com/services/cdn/

* **Traffic Manager** - http://azure.microsoft.com/services/traffic-manager/

* **App Insights** - http://azure.microsoft.com/services/application-insights/

* **App Service** - http://azure.microsoft.com/services/app-service/

* **Bot Service** - http://azure.microsoft.com/services/bot-service/

* **Function App** - http://azure.microsoft.com/services/functions/

* **Search** - http://azure.microsoft.com/services/search/

If you have any issues or suggestions, please let me know.

Thanks.

Rick Shahid

rick.shahid@live.com
