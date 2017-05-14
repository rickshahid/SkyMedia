# Azure Sky Media

Welcome! This repository contains the sample Azure media web application that is deployed at http://www.skymedia.io

The following set of functional capabilities are integrated and enabled via this Azure ASP.NET Core MVC web application:

* Adaptive video streaming across a broad spectrum of devices and platforms including iOS, Android and Windows

* Consumer user identity and platform service account management for Azure Storage, Azure Media Services, etc

* Securely upload and process your media via transcoding, indexing, thumbnail generation, content protection, etc

* Generate metadata via content analytics such as face, object and motion detection to enable discovery and action

* Create subclips (either as dynamic filters or as new assets) via a standard web extension to the Azure Media Player

* Define workflows with multiple tasks (executed parallelly and/or sequentially) that utilize various media processors

* Track content workflow with automatic output publishing based upon parameters specified at job submission time

* Optionally enable SMS text message notification of workflow completion via self-service user profile management

For screenshots of the core application modules in action, refer to http://github.com/RickShahid/SkyMedia/wiki

To enable the various capabilities that are listed above, the following Azure platform services are leveraged:

* Active Directory B2C - http://azure.microsoft.com/en-us/services/active-directory-b2c/

* Key Vault - http://azure.microsoft.com/en-us/services/key-vault/

* Storage - http://azure.microsoft.com/en-us/services/storage/

* Cosmos DB - http://azure.microsoft.com/en-us/services/cosmos-db/

* Search - http://azure.microsoft.com/en-us/services/search/

* Redis Cache - http://azure.microsoft.com/en-us/services/cache/

* Media Services - http://azure.microsoft.com/en-us/services/media-services/

  * Media Analytics - http://azure.microsoft.com/en-us/services/media-services/media-analytics/
  
  * Media Streaming - https://azure.microsoft.com/en-us/services/media-services/live-on-demand/
  
  * Media Player - http://azure.microsoft.com/en-us/services/media-services/media-player/
  
* Cognitive Services - http://azure.microsoft.com/en-us/services/cognitive-services/

* API Management - http://azure.microsoft.com/en-us/services/api-management/

* App Service - http://azure.microsoft.com/en-us/services/app-service/

* Function App - http://azure.microsoft.com/en-us/services/functions/

* Logic App - http://azure.microsoft.com/en-us/services/logic-apps/

* Content Delivery Network - http://azure.microsoft.com/en-us/services/cdn/

* Traffic Manager - http://azure.microsoft.com/en-us/services/traffic-manager/

* App Insights - http://azure.microsoft.com/en-us/services/application-insights/

In addition, the following Azure partner services have also been integrated for high-speed file transfer:

* Signiant Flight - http://www.signiant.com/signiant-flight-for-fast-large-file-transfers-to-azure-blob-storage/

* Aspera FASP - http://azuremarketplace.microsoft.com/en-us/marketplace/apps/aspera.sod

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
