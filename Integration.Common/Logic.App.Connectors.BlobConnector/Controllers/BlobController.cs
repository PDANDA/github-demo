using Interprit.Azure.LogicApps.Connectors.BlobConnector.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;

namespace Interprit.Azure.LogicApps.Connectors.BlobConnector.Controllers
{
    public class BlobController : ApiController
    {
        private CloudBlobClient blobClient;
        private CloudStorageAccount storageAccount;

        public BlobController()
        {
            this.storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            this.blobClient = storageAccount.CreateCloudBlobClient();
        }

        public async Task<IHttpActionResult> Post(Message message)
        {
            var messageString = JsonConvert.SerializeObject(message);
            System.Diagnostics.Trace.TraceInformation($"BlobName: {message.BlobName} {messageString}");

            #region Validation

            if (!message.ContainerName.IsValidBlobContainerName())
            {
                throw new ArgumentException("InterfaceName must only contain letters, numbers and hyphens, and must begin and end with a letter or number.");
            }
            #endregion

            try
            {
                // Parse message
                byte[] byteData = System.Text.Encoding.UTF8.GetBytes(messageString);

                // Write to blob
                var container = blobClient.GetContainerReference(message.ContainerName);
                await container.CreateIfNotExistsAsync();

                // Generate unique blob name
                var blockBlob = container.GetBlockBlobReference(message.BlobName);
                if (!string.IsNullOrWhiteSpace(message.Properties?.AssociatedServiceBusTopicName))
                {
                    blockBlob.Metadata.Add("TopicName", message.Properties.AssociatedServiceBusTopicName);
                }

                await blockBlob.UploadFromByteArrayAsync(byteData, 0, byteData.Length);

                return Content(HttpStatusCode.OK, blockBlob.Uri.ToString());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
                return Content(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
