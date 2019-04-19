//using System.Xml;
//using System.Text;
//using System.Net.Http;
//using System.IdentityModel.Tokens.Jwt;

//using Microsoft.CognitiveServices.Speech;
//using Microsoft.CognitiveServices.Speech.Audio;
//using Microsoft.CognitiveServices.Speech.Intent;
//using Microsoft.CognitiveServices.Speech.Translation;

//using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class CognitiveClient
    {
        //private static XmlDocument GetSpeechXml(string speechText)
        //{
        //    XmlDocument speechXml = new XmlDocument();
        //    XmlElement speakElement = speechXml.CreateElement("speak");
        //    XmlElement voiceElement = speechXml.CreateElement("voice");
        //    voiceElement.InnerText = speechText;
        //    speakElement.AppendChild(voiceElement);
        //    speechXml.AppendChild(speakElement);
        //    return speechXml;
        //}

        //public static void GetSpeech(string speechText, bool neuralVoice)
        //{
        //    JwtSecurityToken authToken = GetAuthToken(neuralVoice);
        //    WebClient webClient = new WebClient(authToken);
        //    string requestUrl = "https://westus.tts.speech.microsoft.com/cognitiveservices/v1";
        //    if (neuralVoice)
        //    {
        //        requestUrl = "https://eastus.tts.speech.microsoft.com/cognitiveservices/v1";
        //    }
        //    XmlDocument speechXml = GetSpeechXml(speechText);
        //    HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Post, requestUrl);
        //    //webRequest.Content = new StringContent(speechXml.InnerXml, Encoding.UTF8, "application/ssml+xml");

        //    string x = @"<speak version='1.0' xmlns='https://www.w3.org/2001/10/synthesis' xml:lang='en-US'><voice name='Microsoft Server Speech Text to Speech Voice (en-US, ZiraRUS)'>Hello Friend</voice></speak>";
        //    webRequest.Content = new StringContent(x, Encoding.UTF8, "application/ssml+xml");
        //    //webRequest.Headers.Add("X-Microsoft-OutputFormat", "audio-24khz-160kbitrate-mono-mp3");
        //    webRequest.Headers.Add("X-Microsoft-OutputFormat", "riff-24khz-16bit-mono-pcm");
        //    webRequest.Headers.Add("User-Agent", "SkyMediaSpeechWest");
        //    byte[] webResponse = webClient.GetResponse<byte[]>(webRequest);

        //    //CloudBlockBlob blockBlob;
        //    //blockBlob.UploadFromByteArrayAsync(webResponse, 0, webResponse.Length);
        //}
    }
}