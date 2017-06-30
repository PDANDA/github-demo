using Interprit.Azure.LogicApps.Connectors.BlobConnector.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;

namespace Interprit.Azure.LogicApps.Connectors.BlobConnector.Controllers
{
    public class ImageBlobController : ApiController
    {
        private CloudBlobClient blobClient;
        private CloudStorageAccount storageAccount;

        public ImageBlobController()
        {
            this.storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            this.blobClient = storageAccount.CreateCloudBlobClient();
        }

        public async Task<IHttpActionResult> Post(Image image)
        {
            System.Diagnostics.Trace.TraceInformation($"BlobName: {image.BlobName}");

            #region Validation
            if (!image.ContainerName.IsValidBlobContainerName())
            {
                throw new ArgumentException("InterfaceName must only contain letters, numbers and hyphens, and must begin and end with a letter or number.");
            }
            #endregion

            try
            {
                // Parse message
                byte[] byteData = Convert.FromBase64String(image.Base64ImageString);

                // Write to blob
                var container = blobClient.GetContainerReference(image.ContainerName);
                await container.CreateIfNotExistsAsync();

                // Generate unique blob name
                var blockBlob = container.GetBlockBlobReference(image.BlobName);

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
