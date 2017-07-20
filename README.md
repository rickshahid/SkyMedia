# Azure Sky Media

Welcome! This repository contains the sample Azure media web application that is deployed at http://www.skymedia.io

![](http://skystorage.azureedge.net/Snip1.AzureSkyMedia.png)

For additional screenshots of key application modules and functionality, refer to http://github.com/RickShahid/SkyMedia/wiki

The following set of media application functionality is enabled and integrated within this Azure ASP.NET Core MVC web app:

* Adaptive and scalable video streaming across a broad spectrum of devices with iOS / macOS, Android, Windows, etc

* User account registration and self-service profile management with linked Azure Storage, Azure Media Services, etc

* Secure upload, storage and media processing via transcoding, indexing, content protection, dynamic packaging, etc

* Discover and extract actionable insights from your video content via Cognitive Services and Artificial Intelligence (AI)

* Create subclips (either as dynamic filters or as new assets) via an Azure Media Player extension (Under Construction)

* Define workflows with multiple tasks (executed in parallel and/or sequence) that integrate various media processors

* Publish workflow outputs automatically based upon key input parameters that are specified at job submission time

* Optionally enable SMS text message notification of workflow completion via integrated user profile management

To enable the core application functionality that is listed above, the following Azure platform services are leveraged:

* Active Directory B2C - http://azure.microsoft.com/en-us/services/active-directory-b2c/

* Storage - http://azure.microsoft.com/en-us/services/storage/

* Cosmos DB - http://azure.microsoft.com/en-us/services/cosmos-db/

* Media Services - http://azure.microsoft.com/en-us/services/media-services/

  * Encoding - http://azure.microsoft.com/en-us/services/media-services/encoding/

  * Streaming - https://azure.microsoft.com/en-us/services/media-services/live-on-demand/
  
  * Analytics - http://azure.microsoft.com/en-us/services/media-services/media-analytics/

  * Indexer - http://azure.microsoft.com/en-us/services/cognitive-services/video-indexer/

    * Cognitive Services - http://azure.microsoft.com/en-us/services/cognitive-services/

    * Search - http://azure.microsoft.com/en-us/services/search/

  * Player - http://azure.microsoft.com/en-us/services/media-services/media-player/

* App Insights - http://azure.microsoft.com/en-us/services/application-insights/

* App Service - http://azure.microsoft.com/en-us/services/app-service/

* Function App - http://azure.microsoft.com/en-us/services/functions/

* Logic App - http://azure.microsoft.com/en-us/services/logic-apps/

* Redis Cache - http://azure.microsoft.com/en-us/services/cache/

* Content Delivery Network - http://azure.microsoft.com/en-us/services/cdn/

* Traffic Manager - http://azure.microsoft.com/en-us/services/traffic-manager/

In addition, the following Azure partner services have been integrated for high-speed file transfer into Azure Storage:

* Signiant Flight - http://www.signiant.com/signiant-flight-for-fast-large-file-transfers-to-azure-blob-storage/

* Aspera FASP Server On Demand - http://azuremarketplace.microsoft.com/en-us/marketplace/apps/aspera.sod

The set of Azure Media Services processors that have been integrated and enabled include the following:

* Encoder Standard - http://docs.microsoft.com/en-us/azure/media-services/media-services-media-encoder-standard-formats

* Encoder Premium - http://docs.microsoft.com/en-us/azure/media-services/media-services-premium-workflow-encoder-formats

* Speech To Text - http://docs.microsoft.com/en-us/azure/media-services/media-services-process-content-with-indexer2

* Face Detection - http://docs.microsoft.com/en-us/azure/media-services/media-services-face-and-emotion-detection

* Face Redaction - http://docs.microsoft.com/en-us/azure/media-services/media-services-face-redaction

* Video Summarization - http://docs.microsoft.com/en-us/azure/media-services/media-services-video-summarization

* Character Recognition - http://docs.microsoft.com/en-us/azure/media-services/media-services-video-optical-character-recognition

* Motion Detection - http://docs.microsoft.com/en-us/azure/media-services/media-services-motion-detection

* Motion Hyperlapse - http://docs.microsoft.com/en-us/azure/media-services/media-services-hyperlapse-content

If you have any comments, enhancement suggestions or if you run into an issue, please let me know.

Thanks.

Rick Shahid

rick.shahid@live.com
