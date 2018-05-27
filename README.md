# Azure Sky Media

Welcome! This repository contains the multi-tenant Azure media solution sample that is deployed at http://www.skymedia.tv

As an illustration, here is an introductory <a href="http://azure.microsoft.com/services/media-services/" target="_blank">Azure Media Services</a> stream that is playing via <a href="http://azure.microsoft.com/services/media-services/media-player/" target="_blank">Azure Media Player</a> integration. Azure Media Services content is adaptive, scaled and globally consumable across a wide range of devices and platforms.

![](http://skymedia.azureedge.net/Shot01.ApplicationWebSite.png)

For sample screenshots of key application views and functionality, refer to http://github.com/RickShahid/SkyMedia/wiki

The following set of media functionality has been integrated and enabled within this Azure ASP.NET Core MVC web app:

* Scalable video (adaptive) streaming to a broad spectrum of devices and platforms (iOS / macOS, Android, Windows)

* Multi-tenant, self-service user account registration and profile management across both storage and media accounts

* Secure content upload, storage and processing (transcoding, indexing, dynamic encryption, dynamic packaging, etc)

* Define workflows using multiple tasks (executed in parallel and/or sequence) that integrate various media processors

* Discover and extract actionable insights from your video content across a wide range of analytic & cognitive services

The following application architecture overview diagram depicts the sample deployment at http://www.skymedia.tv

![](http://skymedia.azureedge.net/Shot02.ApplicationArchitecture.png)

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

  * **Encoding** - http://azure.microsoft.com/services/media-services/encoding/

  * **Streaming** - http://azure.microsoft.com/services/media-services/live-on-demand/
  
  * **Analytics** - http://azure.microsoft.com/services/media-services/media-analytics/

  * **Indexer** - http://azure.microsoft.com/services/cognitive-services/video-indexer/

    * **Cognitive Services** - http://azure.microsoft.com/services/cognitive-services/

    * **Search** - http://azure.microsoft.com/services/search/

  * **Player** - http://azure.microsoft.com/services/media-services/media-player/

* **App Insights** - http://azure.microsoft.com/services/application-insights/

* **App Service** - http://azure.microsoft.com/services/app-service/

* **Bot Service** - http://azure.microsoft.com/services/bot-service/

* **Function App** - http://azure.microsoft.com/services/functions/

* **Content Delivery Network** - http://azure.microsoft.com/services/cdn/

* **Traffic Manager** - http://azure.microsoft.com/services/traffic-manager/

* **App Center** - http://azure.microsoft.com/services/app-center/

If you have any issues or suggestions, please let me know.

Thanks.

Rick Shahid

rick.shahid@live.com
