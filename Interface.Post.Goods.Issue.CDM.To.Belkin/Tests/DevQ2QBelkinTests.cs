using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.IO;
using System.Net;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System.Text;
using System.Diagnostics;

namespace Tests
{
    [TestClass]
    public class DevQ2QTests
    {
        private string devQ2QTestStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=lfxdevintegrationq2tests;AccountKey=3uq+nPQ6fa0lZlfpYv4F1bXbQowLNiToDk4w4LZ5NjhYWUdnhMrMDySychxpu9qish+CIo8fAfh1vF9H82IXXA==;EndpointSuffix=core.windows.net";
        private string devQ2QTestContainerName = "test-output";
        public string GetFileContent(string assemblyPath)
        {
            try
            {
                var assembly = Assembly.GetCallingAssembly();
                var moduleName = assembly.ManifestModule.Name;
                var highLevelQualifier = moduleName.Remove(moduleName.LastIndexOf(".dll"));
                var stream = assembly.GetManifestResourceStream($"{highLevelQualifier}.{assemblyPath}");
                return new StreamReader(stream).ReadToEnd();
            }
            catch { return null; }
        }

        [TestMethod]
        public async Task BelkinSanityCheck()
        {
            var correlationId = Guid.NewGuid().ToString();
            var inputMessage = GetFileContent("DevQ2QTestInputs.DevQ2QBelkinTestInput001.xml");
            var blobName = $"publish-belkinx12-outgoing/{correlationId}";
            var resultMessage = string.Empty;

            var webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/xml");
            webClient.Headers.Add("CorrelationID", correlationId);
            webClient.UploadString("https://linfoxintegration.azure-api.net/postgoodsissue-dev/soap/", inputMessage);

            var storage = CloudStorageAccount.Parse(this.devQ2QTestStorageConnectionString);
            var blobClient = storage.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(this.devQ2QTestContainerName);
            await container.CreateIfNotExistsAsync();
            var endBy = DateTime.Now.AddMinutes(3);
            for (;DateTime.Now < endBy;)
            {
                var blockBlobs = container.ListBlobs(prefix: blobName, useFlatBlobListing: true);
                if (blockBlobs.Count() > 0)
                {
                    var blockBlob = blockBlobs.First() as CloudBlockBlob;
                    var messageBoxContent = await blockBlob.DownloadTextAsync();
                    messageBoxContent = messageBoxContent.Substring(messageBoxContent.IndexOf('{'));
                    dynamic messageBoxObject = JsonConvert.DeserializeObject(messageBoxContent);
                    byte[] data = Convert.FromBase64String((string)messageBoxObject.Base64MessageBody);
                    resultMessage = Encoding.UTF8.GetString(data);
                    blockBlob.Delete();
                    break;
                }
                await Task.Delay(TimeSpan.FromSeconds(3));
            }
            Trace.WriteLine($"EDI Result:{Environment.NewLine}{resultMessage}");
            Assert.IsFalse(string.IsNullOrWhiteSpace(resultMessage));
        }
    }
}
