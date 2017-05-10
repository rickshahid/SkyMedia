
# Azure Sky Media

Welcome! This open repository contains the sample Azure media web application that is deployed at http://www.skymedia.io

The following set of functional capabilities are integrated and enabled with this Azure ASP.NET Core MVC web application:

* Self-service user and service account management across Azure Media Services, Azure Storage, Signiant, Aspera, etc

* Upload and process media via encoding, indexing, content protection, metadata generation, adaptive streaming, etc

* Define media workflows using multiple tasks (executed parallelly and/or sequentially) across various media processors

* Track media workflow tasks and automatically publish outputs based on parameters specified at job submission time

* Generate media metadata such as face, emotion and object detection to drive discoverability and actionable insights

* Generate media subclips (either filtered views or rendered assets) via web clipper integration with Azure Media Player

* Optionally enable SMS text notification of media workflow completion status via integrated user profile management

For screenshots of key application modules, take a look at https://github.com/RickShahid/SkyMedia/wiki

To enable the various capabilities listed above, the following Azure platform services are leveraged:

* Active Directory B2C - http://azure.microsoft.com/en-us/services/active-directory-b2c/

* Key Vault - http://azure.microsoft.com/en-us/services/key-vault/

* Storage - http://azure.microsoft.com/en-us/services/storage/

* Cosmos DB - http://azure.microsoft.com/en-us/services/cosmos-db/

* Media Services - http://azure.microsoft.com/en-us/services/media-services/

  * Media Analytics - http://azure.microsoft.com/en-us/services/media-services/media-analytics/
  
  * Media Streaming - https://azure.microsoft.com/en-us/services/media-services/live-on-demand/
  
  * Media Player - http://azure.microsoft.com/en-us/services/media-services/media-player/
  
* Cognitive Services - http://www.microsoft.com/cognitive-services/

* App Service - http://azure.microsoft.com/en-us/services/app-service/

  * Web App - http://azure.microsoft.com/en-us/services/app-service/web/

  * API App - http://azure.microsoft.com/en-us/services/app-service/api/
 
  * Function App - http://azure.microsoft.com/en-us/services/functions/

* Logic App - http://azure.microsoft.com/en-us/services/logic-apps/

* Search - http://azure.microsoft.com/en-us/services/search/

* Redis Cache - http://azure.microsoft.com/en-us/services/cache/

* Content Delivery Network - http://azure.microsoft.com/en-us/services/cdn/

* Traffic Manager - http://azure.microsoft.com/en-us/services/traffic-manager/

* Application Insights - http://azure.microsoft.com/en-us/services/application-insights/

In addition to the Azure platform services listed above, the following Azure partner services are also integrated:

* Signiant Flight - http://www.signiant.com/signiant-flight-for-fast-large-file-transfers-to-azure-blob-storage/

* Aspera FASP - http://azuremarketplace.microsoft.com/en-us/marketplace/apps/aspera.sod

The set of Azure Media Services processors that are integrated include the following (more are on the way):

* Encoder Standard - http://docs.microsoft.com/en-us/azure/media-services/media-services-media-encoder-standard-formats

* Encoder Premium - http://docs.microsoft.com/en-us/azure/media-services/media-services-premium-workflow-encoder-formats

* Speech To Text - http://docs.microsoft.com/en-us/azure/media-services/media-services-process-content-with-indexer2

* Face Detection - http://docs.microsoft.com/en-us/azure/media-services/media-services-face-and-emotion-detection

* Face Redaction - http://docs.microsoft.com/en-us/azure/media-services/media-services-face-redaction

* Video Summarization - http://docs.microsoft.com/en-us/azure/media-services/media-services-video-summarization

* Character Recognition - http://docs.microsoft.com/en-us/azure/media-services/media-services-video-optical-character-recognition

* Motion Detection - http://docs.microsoft.com/en-us/azure/media-services/media-services-motion-detection

* Motion Hyperlapse - http://docs.microsoft.com/en-us/azure/media-services/media-services-hyperlapse-content

If you have an enhancement suggestion or if you run into an issue, please let me know.

Thanks.

Rick Shahid

rick.shahid@live.com
