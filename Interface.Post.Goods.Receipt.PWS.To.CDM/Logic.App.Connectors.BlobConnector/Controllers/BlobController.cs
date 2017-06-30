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
    public class BlobController : ApiController
    {
        private CloudBlobClient blobClient;
        private CloudStorageAccount storageAccount;

        public BlobController()
        {
            storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            blobClient = storageAccount.CreateCloudBlobClient();
        }

        public async Task<IHttpActionResult> Post(Message message)
        {
            var messageString = JsonConvert.SerializeObject(message);
            System.Diagnostics.Trace.TraceInformation($"BlobName: {message.BlobName} {messageString}");

            #region Validation

            if (!isContainerNameValid(message.ContainerName))
            {
                throw new ArgumentException(@"InterfaceName must only contain letters, numbers and hyphens, "
                                            + "and must begin and end with a letter or number.");
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

                await blockBlob.UploadFromByteArrayAsync(byteData, 0, byteData.Length);

                return Content(HttpStatusCode.OK, $"Created: {message.BlobName}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
                return Content(HttpStatusCode.InternalServerError, ex);
            }
        }

        bool isContainerNameValid(string name)
        {
            Regex regEx = new Regex("^[a-z0-9](?:[a-z0-9]|(\\-(?!\\-))){1,61}[a-z0-9]$|^\\$root$");
            return regEx.IsMatch(name);
        }
    }
}
