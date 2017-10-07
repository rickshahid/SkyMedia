# Azure Sky Media

Welcome! This repository contains the multi-tenant Azure media solution sample that is deployed at http://www.skymedia.io

As an example, here is an introductory Azure Media Services stream playing within the site via Azure Media Player integration.

![](http://skystorage.azureedge.net/Snip01.ApplicationHome.png)

For more screenshots of key application modules and functionality, refer to http://github.com/RickShahid/SkyMedia/wiki

The following set of media functionality has been integrated and enabled within this Azure ASP.NET Core MVC web app:

* Scalable video (adaptive) streaming to a broad spectrum of devices and platforms (iOS / macOS, Android, Windows)

* Self-service user account registration and profile management across storage, transfer and media services accounts

* Secure content upload, storage and processing (transcoding, indexing, dynamic encryption, dynamic packaging, etc)

* Discover and extract actionable insights from your video content across a wide range of cognitive / analytic services

* Define workflows with multiple tasks (executed in parallel and/or sequence) that integrate various media processors

* Optionally enable SMS text message notification of workflow completion via integrated user profile management

To enable the core application functionality that is listed above, the following Azure platform services are leveraged:

![](http://skystorage.azureedge.net/Snip02.ApplicationArchitecture.png)

* **Active Directory B2C** - http://azure.microsoft.com/services/active-directory-b2c/

* **Storage** - http://azure.microsoft.com/services/storage/

* **Cosmos DB** - http://azure.microsoft.com/services/cosmos-db/

* **Media Services** - http://azure.microsoft.com/services/media-services/

  * **Encoding** - http://azure.microsoft.com/services/media-services/encoding/

  * **Streaming** - https://azure.microsoft.com/services/media-services/live-on-demand/
  
  * **Analytics** - http://azure.microsoft.com/services/media-services/media-analytics/

  * **Indexer** - http://azure.microsoft.com/services/cognitive-services/video-indexer/

    * **Cognitive Services** - http://azure.microsoft.com/services/cognitive-services/

    * **Search** - http://azure.microsoft.com/services/search/

  * **Player** - http://azure.microsoft.com/services/media-services/media-player/

* **App Insights** - http://azure.microsoft.com/services/application-insights/

* **App Service** - http://azure.microsoft.com/services/app-service/

* **Bot Service** - http://azure.microsoft.com/services/bot-service/

* **Function App** - http://azure.microsoft.com/services/functions/

* **Logic App** - http://azure.microsoft.com/services/logic-apps/

* **Redis Cache** - http://azure.microsoft.com/services/cache/

* **Content Delivery Network** - http://azure.microsoft.com/services/cdn/

* **Traffic Manager** - http://azure.microsoft.com/services/traffic-manager/

In addition, the following Azure partner services have been integrated for high-speed file transfer into Azure Storage:

* **Signiant Flight** - http://www.signiant.com/signiant-flight-for-fast-large-file-transfers-to-azure-blob-storage/

* **Aspera Server On Demand** - http://azuremarketplace.microsoft.com/marketplace/apps/aspera.sod

If you have any issues or suggestions, please let me know.

Thanks.

Rick Shahid

rick.shahid@live.com

**Data Tier**

[![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FRickShahid%2FSkyMedia%2Fmaster%2FResourceManager%2FTemplate.DataTier.json)

**App Tier (Under Construction)**

[![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FRickShahid%2FSkyMedia%2Fmaster%2FTemplate.AppTier.json)

**Web Tier (Under Construction)**

[![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FRickShahid%2FSkyMedia%2Fmaster%2FTemplate.WebTier.json)
