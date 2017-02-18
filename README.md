# Azure Sky Media

Welcome! This repository contains the sample Azure media web application that is deployed at http://www.skymedia.io

Here is a summary list of the key integrated capabilities within this sample Azure ASP.NET Core MVC web application:

* Self-service user and account management across Azure Media Services, Azure Storage, Signiant Flight, Aspera FASP, etc

* Upload and process media files via encoding, indexing, content protection, metadata generation, adaptive streaming, etc

* Define media workflows using multiple tasks (executed parallelly and/or sequentially) across various media processors

* Background functions that track and publish media workflow task outputs via parameters specified at submission time

* Generate video asset subclips and/or dynamic streaming filters via integration of the Azure Media Video Editor plugin

* Optionally enable SMS text notification of media workflow completion status via integrated user profile management

To see some screenshots of a few key application modules, check out https://github.com/RickShahid/SkyMedia/wiki

To enable the capabilities that are listed above as well as others, the following Azure platform services are leveraged:

* Active Directory B2C - http://azure.microsoft.com/en-us/services/active-directory-b2c/

* Storage - http://azure.microsoft.com/en-us/services/storage/

* Document DB - http://azure.microsoft.com/en-us/services/documentdb/

* Media Services - http://azure.microsoft.com/en-us/services/media-services/

 * Media Analytics - http://azure.microsoft.com/en-us/services/media-services/media-analytics/
 
 * Media Player - http://azure.microsoft.com/en-us/services/media-services/media-player/

* Cognitive Services - http://www.microsoft.com/cognitive-services/

* App Service - http://azure.microsoft.com/en-us/services/app-service/

 * Web - http://azure.microsoft.com/en-us/services/app-service/web/

 * API - http://azure.microsoft.com/en-us/services/app-service/api/
 
 * Mobile - http://azure.microsoft.com/en-us/services/app-service/mobile/
 
 * Functions - http://azure.microsoft.com/en-us/services/functions/

* Search - http://azure.microsoft.com/en-us/services/search/

* Redis Cache - http://azure.microsoft.com/en-us/services/cache/

* Traffic Manager - http://azure.microsoft.com/en-us/services/traffic-manager/

* Content Delivery Network - http://azure.microsoft.com/en-us/services/cdn/

In addition to the Azure platform services listed above, the following Azure partner services are also integrated:

* Signiant Flight - http://azure.microsoft.com/en-us/marketplace/partners/signiant/flight/

* Aspera FASP - http://azure.microsoft.com/en-us/marketplace/partners/aspera/sod/

The set of Azure Media Services processors that are integrated include the following (more are on the way):

* Encoder Standard - https://docs.microsoft.com/en-us/azure/media-services/media-services-media-encoder-standard-formats

* Encoder Premium - https://docs.microsoft.com/en-us/azure/media-services/media-services-premium-workflow-encoder-formats

* Encoder Ultra - https://azure.microsoft.com/en-us/blog/encoding-and-delivery-of-uhd-content/

* Indexer v1 - https://docs.microsoft.com/en-us/azure/media-services/media-services-index-content

* Indexer v2 - https://docs.microsoft.com/en-us/azure/media-services/media-services-process-content-with-indexer2

* Face Detection - https://docs.microsoft.com/en-us/azure/media-services/media-services-face-and-emotion-detection

* Face Redaction - https://docs.microsoft.com/en-us/azure/media-services/media-services-face-redaction

* Motion Detection - https://docs.microsoft.com/en-us/azure/media-services/media-services-motion-detection

* Motion Hyperlapse - https://docs.microsoft.com/en-us/azure/media-services/media-services-hyperlapse-content

* Video Summarization - https://docs.microsoft.com/en-us/azure/media-services/media-services-video-summarization

* Character Recognition - https://docs.microsoft.com/en-us/azure/media-services/media-services-video-optical-character-recognition

If you run into an issue, please let me know.

Thanks.

Rick
