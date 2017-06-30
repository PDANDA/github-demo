#r "Microsoft.WindowsAzure.Storage"
#r "Newtonsoft.Json"

using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Configuration;

public static void Run(MessageBoxBlob trigger, string newBlob, CloudBlockBlob consumedBlob, TraceWriter log)
{
    var logicAppName = "TransformToBelkinLogicApp";
    var client = new WebClient();
    client.Headers.Add("Content-Type", "application/json");
    client.UploadString(ConfigurationManager.AppSettings[$"{logicAppName}Uri"], newBlob);
    consumedBlob.Metadata.Add(logicAppName, "Consumed");
    consumedBlob.SetMetadata();
}

public class MessageBoxBlob
{
    public string Id { get; set; }
    public string Type { get; set; }
}