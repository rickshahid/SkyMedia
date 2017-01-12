# Azure Sky Media

Welcome! This repository contains the sample Azure media web application that is deployed at http://www.skymedia.io

Here is a breakdown of the integrated capabilities that are provided within this sample ASP.NET Core MVC web application:

* Self-service user and account registration across Azure Media Services, Azure Storage, Signiant Flight & Aspera FASP

* Define media workflows with multiple job tasks (executed parallelly and/or sequentially) across various media processors

* Upload and process media files for encoding, indexing, content protection, metadata generation, adaptive streaming, etc

* Background processing to automatically publish media workflow output based upon parameters captured at submission

* Optionally enable SMS text notification of media workflow completion status via integrated user profile management

For screenshot examples of key modules, check out the Wiki page at https://github.com/RickShahid/SkyMedia/wiki

To enable the range of capabilities that are listed above, the following Azure platform services are leveraged:

* Active Directory B2C - http://azure.microsoft.com/en-us/services/active-directory-b2c/

* Storage - http://azure.microsoft.com/en-us/services/storage/

* Document DB - http://azure.microsoft.com/en-us/services/documentdb/

* Media Services - http://azure.microsoft.com/en-us/services/media-services/

 * Media Analytics - http://azure.microsoft.com/en-us/services/media-services/media-analytics/
 
 * Media Player - http://azure.microsoft.com/en-us/services/media-services/media-player/

* App Service - http://azure.microsoft.com/en-us/services/app-service/

 * Web - http://azure.microsoft.com/en-us/services/app-service/web/

 * API - http://azure.microsoft.com/en-us/services/app-service/api/
 
 * Mobile - http://azure.microsoft.com/en-us/services/app-service/mobile/
 
 * Functions - http://azure.microsoft.com/en-us/services/functions/

* Logic Apps - http://azure.microsoft.com/en-us/services/logic-apps/

* Traffic Manager - http://azure.microsoft.com/en-us/services/traffic-manager/

* Content Delivery Network (CDN) - http://azure.microsoft.com/en-us/services/cdn/

In addition to the native Azure services listed above, the following Azure partner services have also been incorporated:

* Signiant Flight - http://azure.microsoft.com/en-us/marketplace/partners/signiant/flight/

* Aspera FASP - http://azure.microsoft.com/en-us/marketplace/partners/aspera/sod/

The set of Azure Media Services processors that are integrated within the app include the following (more are on the way):

* Encoder Standard - https://docs.microsoft.com/en-us/azure/media-services/media-services-media-encoder-standard-formats

* Encoder Premium - https://docs.microsoft.com/en-us/azure/media-services/media-services-premium-workflow-encoder-formats

* Indexer v1 - https://docs.microsoft.com/en-us/azure/media-services/media-services-index-content

* Indexer v2 - https://docs.microsoft.com/en-us/azure/media-services/media-services-process-content-with-indexer2

* Face Detection - https://docs.microsoft.com/en-us/azure/media-services/media-services-face-and-emotion-detection

* Face Redaction - https://docs.microsoft.com/en-us/azure/media-services/media-services-face-redaction

* Motion Detection - https://docs.microsoft.com/en-us/azure/media-services/media-services-motion-detection

* Motion Hyperlapse - https://docs.microsoft.com/en-us/azure/media-services/media-services-hyperlapse-content

* Video Summarization - https://docs.microsoft.com/en-us/azure/media-services/media-services-video-summarization

* Character Recognition - https://docs.microsoft.com/en-us/azure/media-services/media-services-video-optical-character-recognition

If you run into an issue, please let me know. As always, feedback and/or suggestions is highly encouraged and appreciated.

Thanks.

Rick
