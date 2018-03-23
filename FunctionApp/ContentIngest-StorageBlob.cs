using System.IO;
using System.Collections.Generic;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class ContentIngestStorageBlob
    {
        [FunctionName("ContentIngest-StorageBlob")]
        public static void Run([BlobTrigger("ams/{name}")] Stream blob, string name, TraceWriter log)
        {
            log.Info($"Content Ingest: {name}");
            if (name.EndsWith(Constant.Media.FileExtension.ManifestAsset))
            {
                StreamReader streamReader = new StreamReader(blob);

                MediaAccount mediaAccount = new MediaAccount()
                {
                    DomainName = streamReader.ReadLine(),
                    EndpointUrl = streamReader.ReadLine(),
                    ClientId = streamReader.ReadLine(),
                    ClientKey = streamReader.ReadLine()
                };

                List<string> fileNames = new List<string>();
                while (!streamReader.EndOfStream)
                {
                    string fileName = streamReader.ReadLine();
                    fileNames.Add(fileName);
                }

                MediaClient mediaClient = new MediaClient(mediaAccount);
                string assetName = Path.GetFileNameWithoutExtension(name);
                string assetId = mediaClient.CreateAsset(assetName, fileNames.ToArray());
                log.Info($"Asset Id: {assetId}");
            }
        }
    }
}