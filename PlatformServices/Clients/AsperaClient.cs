using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal class AsperaClient
    {
        private string _serviceGateway;
        private string _serviceWorker;
        private WebClient _serviceClient;
        private string _accountId;
        private string _accountKey;
        private string _iterationToken;

        public AsperaClient(string authToken)
        {
            User authUser = new User(authToken);
            _serviceGateway = authUser.AsperaServiceGateway;

            string[] serviceGateway = _serviceGateway.Split('.');
            serviceGateway[0] = string.Concat(serviceGateway[0], Constant.Storage.Partner.AsperaWorker);
            _serviceWorker = string.Join(".", serviceGateway);

            _accountId = authUser.AsperaAccountId;
            _accountKey = authUser.AsperaAccountKey;
            _serviceClient = new WebClient(_accountId, _accountKey);

            string settingKey = Constant.AppSettingKey.AsperaTransferInfo;
            string transferInfo = AppSetting.GetValue(settingKey);
            string transferApi = string.Concat(_serviceWorker, transferInfo);

            using (HttpRequestMessage request = _serviceClient.GetRequest(HttpMethod.Get, transferApi))
            {
                JObject transfer = _serviceClient.GetResponse<JObject>(request);
                _iterationToken = transfer["info_result"]["iteration_token"].ToString();
            }
        }

        public JObject GetTransferSpecs(string storageRoot, string containerName, string[] filePaths, bool fileDownload)
        {
            string settingKey = Constant.AppSettingKey.AsperaUploadSetup;
            string uploadSetup = AppSetting.GetValue(settingKey);
            string transferApi = string.Concat(_serviceGateway, uploadSetup);
            if (fileDownload)
            {
                settingKey = Constant.AppSettingKey.AsperaDownloadSetup;
                string downloadSetup = AppSetting.GetValue(settingKey);
                transferApi = string.Concat(_serviceGateway, downloadSetup);
            }

            TransferRequest transferRequest = new TransferRequest();
            if (fileDownload)
            {
                transferRequest.SourceRoot = storageRoot;
            }
            else
            {
                transferRequest.DestinationRoot = storageRoot;
            }

            List<TransferPath> transferPaths = new List<TransferPath>();
            foreach (string filePath in filePaths)
            {
                TransferPath transferPath = new TransferPath()
                {
                    Source = filePath,
                    Destination = string.Concat("/", containerName, "/", Path.GetFileName(filePath))
                };
                transferPaths.Add(transferPath);
            }
            transferRequest.Paths = transferPaths.ToArray();

            AsperaRequest asperaRequest = new AsperaRequest()
            {
                TransferRequest = transferRequest
            };

            AsperaTransfer asperaTransfer = new AsperaTransfer()
            {
                TransferRequests = new AsperaRequest[] { asperaRequest }
            };

            ServicePointManager.ServerCertificateValidationCallback += delegate
                (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
            {
                return true;
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            JObject webResponse;
            JToken transferSettings;
            using (HttpRequestMessage request = _serviceClient.GetRequest(HttpMethod.Post, transferApi, asperaTransfer))
            {
                webResponse = _serviceClient.GetResponse<JObject>(request);
                transferSettings = webResponse["transfer_specs"][0];
            }

            JToken transferSpec = transferSettings["transfer_spec"];
            transferSpec["remote_user"] = _accountId;
            transferSpec["remote_password"] = _accountKey;

            JObject connectSpec = new JObject();
            connectSpec.Add("allow_dialogs", true);
            connectSpec.Add("return_paths", true);
            connectSpec.Add("use_absolute_destination_path", true);

            transferSettings["aspera_connect_settings"] = connectSpec;
            return webResponse;
        }
    }
}