namespace Interprit.Common

open System
open FSharpx


module Option =
        let bimap f g = function | Some(v) -> f v | None -> g()         
        
        let toOption v = if isNull v then None else Some v
        
        let contains v = function | Some(x) -> x = v | None -> false;
        let check f = function | Some(x) -> f(x) | None -> false;
        

module Choice =
        let flip = function | Choice1Of2(v) -> Choice2Of2(v) | Choice2Of2(v) -> Choice1Of2(v)        
        let fromErrorOption = function | None -> Choice1Of2(())
                                       | Some(x) -> Choice2Of2(x)     
        let reduce = function | Choice1Of2((f,s)) -> (f,s)
                              | Choice2Of2((f,s)) -> (f,s)      
        
//
// Utilties
//
module Utilties = 
        open System
        open Suave
        open Suave.Filters
        open Suave.Operators
        open Suave.Successful
        open Suave.Json
        open System.Runtime.Serialization
        open System.Text
        open Suave.Operators
        open System.Net
        open Suave.Filters
        open Newtonsoft.Json
        open System
        open Microsoft.FSharp.Reflection
        open Newtonsoft.Json
        open Newtonsoft.Json.Converters
        open System
        open Microsoft.FSharp.Reflection
        open Newtonsoft.Json
        open Newtonsoft.Json.Converters
        open System.IO

        type OptionConverter() =
                inherit JsonConverter()

                override x.CanConvert(t) = 
                        t.IsGenericType && t.GetGenericTypeDefinition() = typedefof<option<_>>

                override x.WriteJson(writer, value, serializer) =
                                let value = 
                                        if isNull(value) then null
                                        else 
                                                let _,fields = FSharpValue.GetUnionFields(value, value.GetType())
                                                fields.[0]  
                                serializer.Serialize(writer, value)

                override x.ReadJson(reader, t, existingValue, serializer) =        
                                let innerType = t.GetGenericArguments().[0]
                                let innerType = 
                                        if innerType.IsValueType then (typedefof<Nullable<_>>).MakeGenericType([|innerType|])
                                        else innerType        
                                let value = serializer.Deserialize(reader, innerType)
                                let cases = FSharpType.GetUnionCases(t)
                                if isNull(value) then FSharpValue.MakeUnion(cases.[0], [||])
                                else FSharpValue.MakeUnion(cases.[1], [|value|])


        let setCORSHeaders = Writers.setHeader  "Access-Control-Allow-Origin" "*" >=> Writers.setHeader "Access-Control-Allow-Headers" "content-type"

        let toJson o = JsonConvert.SerializeObject(o, new OptionConverter())
        let toJsonOK o : WebPart = setCORSHeaders >=> OK (JsonConvert.SerializeObject(o, new OptionConverter())) >=> Writers.setMimeType "application/json; charset=utf-8"
        let fromJson<'a> json = JsonConvert.DeserializeObject(json, typeof<'a>, new OptionConverter()) :?> 'a

        let writeAllTextToFile fileName (text : string) = File.WriteAllText(fileName, text)        

        let getResourceFromReq<'a>(req : HttpRequest) =
                let getString rawForm = System.Text.Encoding.UTF8.GetString(rawForm)
                req.rawForm |> getString |>  fromJson<'a>  

        let convertFromBase64StringToASCIIString (s : string) =            
                s |> Convert.FromBase64String |> System.Text.ASCIIEncoding.ASCII.GetString
        
        let convertFileFromBase64 (inFile : string) (outFile : string) =                           
              inFile |> System.IO.File.ReadAllText
                     |> Convert.FromBase64String
                     |> System.Text.ASCIIEncoding.ASCII.GetString
                     |> (fun s -> File.WriteAllText(outFile, s))

module Http = 
         open Suave
         open Suave.Filters
         open Suave.Operators
         open Suave.Successful
         open Suave.Json
         open System.Runtime.Serialization
         open System.Text
         open Suave.Operators
         open System.Net
         open Suave.Filters
         open Newtonsoft.Json
         open FSharpx
         open FSharpx.Choice
         let fromHttpRequestToObject<'a>(req : HttpRequest) =
                try 
                        let getString rawForm = System.Text.Encoding.UTF8.GetString(rawForm)
                        req.rawForm |> getString |> Utilties.fromJson<'a> |> Choice1Of2
                with
                | ex -> Choice2Of2(ex.ToString())

module AppSetttings = 
         open System.Configuration         
         open System

         let getAppSettingString (key : string) = 
                try                         
                        Choice1Of2(ConfigurationManager.AppSettings.[key])         
                with 
                | ex -> Choice2Of2(ex.ToString())
         
         let getAppSettingBoolean (key : string) = 
                try
                        Choice1Of2(Boolean.Parse(ConfigurationManager.AppSettings.[key]))
                with
                | ex -> Choice2Of2(ex.ToString()) 

module Json =
        open Newtonsoft.Json
        open Newtonsoft.Json.Converters
        open System.IO
        open Microsoft.FSharp.Reflection
        
        type OptionConverter() =
                inherit JsonConverter()

                override x.CanConvert(t) = 
                        t.IsGenericType && t.GetGenericTypeDefinition() = typedefof<option<_>>

                override x.WriteJson(writer, value, serializer) =
                                let value = 
                                        if isNull(value) then null
                                        else 
                                                let _,fields = FSharpValue.GetUnionFields(value, value.GetType())
                                                fields.[0]  
                                serializer.Serialize(writer, value)

                override x.ReadJson(reader, t, existingValue, serializer) =        
                                let innerType = t.GetGenericArguments().[0]
                                let innerType = 
                                        if innerType.IsValueType then (typedefof<Nullable<_>>).MakeGenericType([|innerType|])
                                        else innerType        
                                let value = serializer.Deserialize(reader, innerType)
                                let cases = FSharpType.GetUnionCases(t)
                                if isNull(value) then FSharpValue.MakeUnion(cases.[0], [||])
                                else FSharpValue.MakeUnion(cases.[1], [|value|])
        
        let fromObjectToJson o : Choice<string, string> = 
                try
                        Choice1Of2(JsonConvert.SerializeObject(o, new OptionConverter()))
                with
                | ex -> Choice2Of2(ex.ToString());

        let fromJsonToObject<'a> json = 
                try
                        Choice1Of2(JsonConvert.DeserializeObject(json, typeof<'a>, new OptionConverter()) :?> 'a)
                with
                | ex -> Choice2Of2(ex.ToString())

        let writeJsonToFile<'a> fileName (v : 'a)=
              use file = File.CreateText(fileName)
              let serializer = new JsonSerializer();
              serializer.Serialize(file, v);

        let readJsonFromFile<'a> fileName =
              JsonConvert.DeserializeObject<'a>(File.ReadAllText(fileName)); 

//
// Azure Services
//
module AzureBlobStorage =         
        open System
        open System.IO
        open Microsoft.Azure // Namespace for CloudConfigurationManager
        open Microsoft.WindowsAzure.Storage // Namespace for CloudStorageAccount
        open Microsoft.WindowsAzure.Storage.Blob // Namespace for Blob storage types
        open System.Linq
        open System.Text        
        open FSharpx
        open FSharpx.Choice
        
        type AzureBlobStorageConfig = {
                ConnectionString : string;    
                CreateIfNotExists : bool;                         
                ContainerReference : string;                                
        }  

        type DomainMessage = CouldNotGetBlobRef of reason : Exception                            
                           | CouldNotDownloadBlobContentsAsText of string
                           | CouldNotRetrieveBlobContentsAndMetaData of string
                           | CouldNotDeleteBlob of Exception
                           | CouldNotGetBlobNamesByPrefix of Exception 
                           | CouldNotCreateBlob of CouldNotCreateBlobReason                                   
                           | CouldNotRetrieveContainerList of string
        and CouldNotCreateBlobReason = CouldNotCreateBlobDomainMessage of DomainMessage
                                      | CouldNotCreateBlobException of Exception

        
        type CreateBlobRequest = {
                BlobName : string;
                MetaData : Map<string,string>;
                Contents : string;
        }

        let private getBlockBlobRef (config : AzureBlobStorageConfig) (blobName : string) =
                let sa = CloudStorageAccount.Parse(config.ConnectionString)
                let blobClient = sa.CreateCloudBlobClient()
                let container = blobClient.GetContainerReference(config.ContainerReference)                
                let success = if config.CreateIfNotExists then container.CreateIfNotExists() else true;
                let blockBlob = container.GetBlockBlobReference(blobName)
                blockBlob

        let checkBlobConnection (config : AzureBlobStorageConfig) =
                try                        
                        let sa = CloudStorageAccount.Parse(config.ConnectionString)
                        let blobClient = sa.CreateCloudBlobClient()
                        let container = blobClient.GetContainerReference(config.ContainerReference)                                                 
                        Choice1Of2(container.Exists())                        
                with
                | :? StorageException as se -> Choice2Of2(CouldNotGetBlobRef(se.InnerException.InnerException))
                | ex -> Choice2Of2(CouldNotGetBlobRef(ex))  
                              
        let getBlob blobName config = 
                try                  
                    Choice1Of2(getBlockBlobRef config blobName)
                with
                | :? StorageException as se -> Choice2Of2(CouldNotGetBlobRef(se.InnerException.InnerException))
                | ex -> Choice2Of2(CouldNotGetBlobRef(ex))

        let getBlobContentsAsText config blobName : Choice<string, DomainMessage> =                                
                try 
                        let ref = getBlockBlobRef config blobName
                        Choice1Of2(ref.DownloadText())
                with
                | :? StorageException as se -> Choice2Of2(CouldNotGetBlobRef(se.InnerException.InnerException))
                | ex -> Choice2Of2(CouldNotDownloadBlobContentsAsText(ex.ToString()))       

        let getBlobContentsWithMetaDataAndProperties config blobName : Choice<_, DomainMessage> =                                
                try 
                        let ref = getBlockBlobRef config blobName
                        let contents = ref.DownloadText()                        
                        ref.FetchAttributes()                                               
                        let ps = ref.Metadata |> Seq.map (|KeyValue|) |> Map.ofSeq 
                        
                        Choice1Of2((contents, ps, ref.Properties))
                with
                | :? StorageException as se -> Choice2Of2(CouldNotGetBlobRef(se.InnerException.InnerException))
                | ex -> Choice2Of2(CouldNotRetrieveBlobContentsAndMetaData(ex.ToString())) 

        let deleteBlob config blobName =
                try 
                        let ref = getBlockBlobRef config blobName
                        Choice1Of2(ref.Delete())
                with
                | ex -> Choice2Of2(CouldNotDeleteBlob(ex))
                
                                          
        let getBlobsByPrefix prefix (config : AzureBlobStorageConfig) =                                                 
                try 
                       let sa = CloudStorageAccount.Parse(config.ConnectionString)
                       let blobClient = sa.CreateCloudBlobClient()
                       let container = blobClient.GetContainerReference(config.ContainerReference)                                       

                       Choice1Of2(container.ListBlobs(prefix, true, BlobListingDetails.Metadata, null, null).OfType<ICloudBlob>())            
                with 
                | ex -> Choice2Of2(CouldNotGetBlobNamesByPrefix(ex))
         
        let exists predicate config =
                choose {
                    let! blobs = getBlobsByPrefix String.Empty config
                    
                    return blobs.Take(100) |> Seq.exists predicate
                }

        let saveBlob (config : AzureBlobStorageConfig) (r : CreateBlobRequest) =                               
                try
                    let sa = CloudStorageAccount.Parse(config.ConnectionString)
                    let blobClient = sa.CreateCloudBlobClient()
                    let container = blobClient.GetContainerReference(config.ContainerReference)                
                    let success = if config.CreateIfNotExists then container.CreateIfNotExists() else true;
                    let blockBlob = container.GetBlockBlobReference(r.BlobName)
                                                                        
                    let b = getBlockBlobRef config r.BlobName
                                                
                    do b.UploadText(r.Contents)                
                    
                    r.MetaData |> Map.iter(fun k v -> b.Metadata.Add(k, v))
                                                
                    b.SetMetadata()  

                    None    
                with
                | ex -> Some(CouldNotCreateBlob(CouldNotCreateBlobException(ex)))               
                
        let getContainerList (config : AzureBlobStorageConfig) =
                try
                    let storageAccount = CloudStorageAccount.Parse(config.ConnectionString);

                  // Create the blob client.
                    let blobClient = storageAccount.CreateCloudBlobClient();
      
                    Choice1Of2(blobClient.ListContainers())  
                with
                | ex -> Choice2Of2(CouldNotRetrieveContainerList(ex.ToString()))                                     
                
// module AzureSearch = 
//         open Microsoft.Azure.Search
//         open Microsoft.Azure.Search.Models
//         open FSharpx
//         open FSharpx.Choice

//         type AzureSearchConfig = {
//                 ApiKey : string;
//                 ServiceName : string; 
//                 IndexName : string;               
//         }

//         type AzureSearchRequest = {
//                 SearchText : string Option;
//                 Params : SearchParameters;
//         }        
        
//         type DomainMessage = CouldNotCreateClient of Exception
//                            | CouldNotDeleteIndex of Exception
//                            | CouldNotCreateIndex of Exception               
//                            | CouldNotSearchIndex of Exception
        
//         let createSearchServiceClient (c : AzureSearchConfig) : Choice<ISearchServiceClient, DomainMessage>=
//                 try 
//                     Choice1Of2(new SearchServiceClient(c.ServiceName, new SearchCredentials(c.ApiKey)) :> ISearchServiceClient)
//                 with
//                 | ex -> Choice2Of2(CouldNotCreateClient(ex))
                                
//         let createSearchIndexClient (c : AzureSearchConfig) : Choice<ISearchIndexClient, DomainMessage>=
//                 try 
//                     Choice1Of2(new SearchIndexClient(c.ServiceName, c.IndexName, new SearchCredentials(c.ApiKey)) :> ISearchIndexClient)
//                 with
//                 | ex -> Choice2Of2(CouldNotCreateClient(ex))
                                
//         let deleteIndexIfExists (config : AzureSearchConfig) (c : ISearchServiceClient)  =
//                 try 
//                     if c.Indexes.Exists(config.IndexName) then 
//                         c.Indexes.Delete(config.IndexName); 
                    
//                     Choice1Of2(())                     
//                 with
//                 | ex -> Choice2Of2(CouldNotDeleteIndex(ex))                                

//         let createIndex (c : ISearchServiceClient) (i : Index) =   
//                 try                  
//                     Choice1Of2(c.Indexes.Create(i))
//                 with 
//                 | ex -> Choice2Of2(CouldNotCreateIndex(ex))

//         let searchBy<'T when 'T : not struct>(r : AzureSearchRequest) (c: ISearchIndexClient)=
//                 let searchText = r.SearchText |> function | Some (v) -> v | None -> "*"
//                 Choice.protect (fun _ -> c.Documents.Search<'T>(searchText, r.Params)) () |> Choice.mapSecond CouldNotSearchIndex   

module AzureServiceBus = 
         open Microsoft.ServiceBus.Messaging
         open Microsoft.ServiceBus;
         open System.IO
         open System.Text
         
         type AzureServiceBusConfig = {
                 QueueName : string 
                 ConnectionString : AzureServiceBusConnectionString;                 
         }
         and AzureServiceBusConnectionString = string

         type DomainMessage = | PublishMessageToTopicFailure of message : string    
                              | NoMessageReceived 
                              | SubscriptionExists
                              | GetMessageFromSubscriptionFailure of message : string
                              | CreateSubscriptionFailure of message : string
                              | DeleteSubscriptionFailure of message : string

         type SendMessageRequest = {
                MessageBody : string;
                ContextProperties : Map<string, string>;
                CorrelationId : string;
         }                                  

         type ReceivedMessageResponse = {
                MessageBody : string;
                ContextProperties : Map<string, string>;
                CorrelationId : string
         }                          

         type CreateSubscriptionRequest = {
                TopicName : string;
                SubscriptionName : string;
                Filter : Filter Option
         }   

         let sendMessageToTopic (r : SendMessageRequest) (c : AzureServiceBusConfig) =
                try 
                    use m = new BrokeredMessage(new MemoryStream(Encoding.UTF8.GetBytes(r.MessageBody), true))                                                                          
                    m.CorrelationId <- r.CorrelationId
                    
                    r.ContextProperties |> Map.toList |> List.iter (m.Properties.Add) 

                    let client = TopicClient.CreateFromConnectionString(c.ConnectionString, c.QueueName)
                    
                    client.Send(m)

                    None
                with 
                | ex -> Some(PublishMessageToTopicFailure(ex.ToString())) 

         let getMessageFromSubscription subscription (c : AzureServiceBusConfig)  : Choice<ReceivedMessageResponse, DomainMessage> =
                try                     
                    let client = SubscriptionClient.CreateFromConnectionString(c.ConnectionString, c.QueueName, subscription)

                    let m = client.Receive(TimeSpan.FromSeconds(30.0))   

                    if isNull m then
                        Choice2Of2(NoMessageReceived)
                    else         
                        let newMessage = { MessageBody = (new StreamReader(m.GetBody<Stream>())).ReadToEnd()
                                         ; ContextProperties = m.Properties |> Seq.map (|KeyValue|) 
                                                                            |> Seq.map(fun (k, v) -> (k, v.ToString()))
                                                                            |> Map.ofSeq 
                                         ; CorrelationId = m.CorrelationId }                        
                        m.Complete()
                        Choice1Of2(newMessage) 
                with 
                | ex -> Choice2Of2(GetMessageFromSubscriptionFailure(ex.ToString()))  

         let createSubscription (r : CreateSubscriptionRequest) (c : AzureServiceBusConfig) =                                                    
                try
                        let nm = NamespaceManager.CreateFromConnectionString(c.ConnectionString);

                        if (nm.SubscriptionExists(r.TopicName, r.SubscriptionName)) then
                            None
                        else
                            match r.Filter with 
                            | Some(f) -> nm.CreateSubscription(r.TopicName, r.SubscriptionName, f)
                            | None -> nm.CreateSubscription(r.TopicName, r.SubscriptionName)
                            |> ignore

                            None
                with 
                | ex -> Some(CreateSubscriptionFailure(ex.ToString()))  
         let deleteSubscription topicName subscriptionName (c : AzureServiceBusConfig) =                                                    
                try
                        let nm = NamespaceManager.CreateFromConnectionString(c.ConnectionString);

                        if (nm.SubscriptionExists(topicName, subscriptionName)) then
                            nm.DeleteSubscription(topicName, subscriptionName) |> ignore;                                
                                                    
                        None
                with 
                | ex -> Some(DeleteSubscriptionFailure(ex.ToString())) 

module IO = 
        open System.IO
        let readFileContents (path : string) = 
                try 
                   Choice1Of2(File.ReadAllLines(path))
                with
                | ex -> Choice2Of2(ex.ToString())

module Xml =
        open System.Xml.Linq;
        open System.Xml.Xsl;
        open System.Xml;
        open System.Xml.XPath
        open FSharpx
        open System.IO
        open System.Text

        type DomainMessage = | CouldNotLoadXmlDocument
                             | CouldNotFindElement

        let tryParseXml (xml : string) =
                try 
                    Choice1Of2(XDocument.Load(xml));                                         
                with
                | ex -> Choice2Of2(ex.ToString)

        let tryGetElement (elementName : string) (xpath : string) (d : XDocument) =
                try                                         
                    xpath |> d.XPathSelectElement 
                          |> Option.toOption
                          |> Choice1Of2
                with
                | ex -> Choice2Of2(ex.ToString)                
        
        let transformXslt (inputXmlFileName : string) 
                          (xsltFileName : string) 
                          (outputFileName : string) =                                                                           
                let myXslTrans = new XslCompiledTransform(); 
                myXslTrans.Load(xsltFileName); 
                myXslTrans.Transform(inputXmlFileName, outputFileName); 

        let transformXsltWithArgs (inputXmlFileName : string) 
                                  (xsltFileName : string) 
                                  (outputFileName : string)
                                  (args : XsltArgumentList) =
                let myXslTrans = new XslCompiledTransform(); 
                myXslTrans.Load(xsltFileName); 

                use sw = new StringWriter()    
                myXslTrans.Transform(inputXmlFileName, args, sw); 
                File.WriteAllText(outputFileName, XDocument.Parse(sw.ToString()).ToString())
        let isXmlSame (xml1 : string) (xml2 : string) =
                let x1 = XDocument.Parse(xml1).ToString()
                let x2 = XDocument.Parse(xml2).ToString()                
                x1 = x2
        
        let writeFormatedXmlToFile (path : string) (xml : string)  =
                File.WriteAllText(path, XDocument.Parse(xml).ToString()) 

        let compareXmlFiles (xmlFileName1 : string) (xmlFileName2 : string) =
                let xml1 = XDocument.Load(xmlFileName1).ToString()
                let xml2 = XDocument.Load(xmlFileName2).ToString()
                xml1 = xml2

        let getXmlElementValue (d : XDocument) (xnm : XmlNamespaceManager) xpath  = 
                d.XPathSelectElement(xpath, xnm) |> (fun x -> if isNull x then None else Some(x.Value) )

        let toXsltArgList (ps : (string * string * string) list) = 
                let xsltArgList = new XsltArgumentList();         
                ps |> List.iter (fun (nm, ns, v) -> xsltArgList.AddParam(nm, ns, v))
                xsltArgList        

module MessageStore =                                       
        let parseBlobName (blobName : string) = let xs = blobName.Split('_') 
                                                DateTime.Parse(xs.[0]), xs.[1], xs.[2], xs.[3]

           
module Test =          
        type TestEnv = DEV = 1 | SIT = 2 | UAT = 3

        type TestConfig = {
                Env : TestEnv;
                ServiceBusConnectionString : string;  
                TestNameFilter : string Option;             
        }

        let getTestDataDirPath (env : TestEnv) =
                sprintf "%s\\..\\..\\..\\TestData\\" (new Uri(Reflection.Assembly.GetCallingAssembly().CodeBase)).AbsolutePath                   

        let replaceTestVariables (testName : string) (s : string) =
                        let testdir = sprintf "%s\\..\\..\\..\\TestData\\" (new Uri(Reflection.Assembly.GetCallingAssembly().CodeBase)).AbsolutePath
                        
                        s.Replace("$(TESTDATA_DIR)", testdir)
                         .Replace("$(TESTCASE_NAME)", testName)

        let getMessageContextPropertiesFromFile (fileName : string) =
                fileName |> System.IO.File.ReadAllLines
                         |> Array.map(fun line -> let k = line.[0 .. line.IndexOf(':') - 1].Trim()
                                                  let v = line.[line.IndexOf(':') + 1 .. ].Trim()
                                                  (k, v))                 
                         |> Map.ofArray                 

        module Expecto =                        
                open FSharpx 
                open FSharpx.Choice
                open AzureServiceBus
                open System
                open Expecto
                open Xml
                open System
                open System.IO
                open Json
            
                type PublishMessageRequest = {
                        InBoundTopicName : string;
                        OutBoundTopicName : string;                  
                        CorrelationId : string;
                        ServiceBusConnectionString : string;                 
                        MessageBody : string;
                        ContextProperties : Map<string, string>;
                        CreateSubscription : bool;
                }

                type XsltTransformTestConfig = {
                          InputXmlFileName : string;
                          XsltFileName : string;
                          OutputXmlFileName : string;
                          ExpectedXmlFileName : string;
                          XsltArgList : (XsltParamName * XsltParamNamespace * XsltParamValue) list
                }
                and XsltParamName = string
                and XsltParamNamespace = string
                and XsltParamValue = string

                type Q2QTestConfig = {                
                        InputMessageFileName : string; 
                        InputMessageContextProperties : Map<string, string>
                        OutputMessageFileName : string;
                        OutputContextPropertiesFileName : string;
                        InBoundTopicName : string;
                        OutBoundTopicName : string;
                        ServiceBusConnectionString : string;       
                }

                type WebToQueueDomainMessage =
                     | CreateOutboundSubscriptionFailure of AzureServiceBus.DomainMessage
                     | SendInboundMessageFailure of string
                     | ReceiveOutboundMessageFailure of AzureServiceBus.DomainMessage    
                     | DeleteOutboundSubscriptionFailure of AzureServiceBus.DomainMessage

                type WebToQueueTestConfig = {  
                     InboundUrl : string;   
                     InboundMessage : string;
                     CorrelationId : string;
                     ContentType : string;
                     OutboundTopicName : string;
                     OutboundServiceBusConnectionString : string;
                }                                         
                            
                open Microsoft.ServiceBus.Messaging
                open Microsoft.ServiceBus;               

                let getTestConfig (args : string array) : TestConfig =         
                        match args with
                        | [| env; cs |] -> { Env = (TestEnv.Parse(typedefof<TestEnv>, env, true) :?> TestEnv)
                                           ; ServiceBusConnectionString = cs
                                           ; TestNameFilter = None }
                        | [| env; cs; filter |] -> { Env = (TestEnv.Parse(typedefof<TestEnv>, env, true) :?> TestEnv)
                                                   ; ServiceBusConnectionString = cs
                                                   ; TestNameFilter = Some(filter) }
                        | _ -> failwith "Invalid number of arguments"  
        
                let filterTestCasesByConfig (c : TestConfig) (ts : Test) = match c.TestNameFilter with
                                                                           | Some(filter) -> ts |> Test.filter (fun s -> s.Contains filter)
                                                                           | None -> ts               
                             

                let executeQueueToQueueTest (r : PublishMessageRequest) : Choice<ReceivedMessageResponse, AzureServiceBus.DomainMessage> =                            
                            let outBoundTestSubName = Guid.NewGuid().ToString() + "_sub_test"                                                                                                             
                            
                            choose {                                                
                                        let inboundCs = { QueueName = r.InBoundTopicName; ConnectionString = r.ServiceBusConnectionString };
                                        let outboudCs = { QueueName = r.OutBoundTopicName; ConnectionString = r.ServiceBusConnectionString }
                                   
                                        do! if r.CreateSubscription then                                         
                                                createSubscription ({ TopicName = r.OutBoundTopicName
                                                                    ; SubscriptionName = outBoundTestSubName
                                                                    ; Filter = Some(upcast CorrelationFilter(r.CorrelationId) ) }) outboudCs 
                                                |> Choice.fromErrorOption                                    
                                            else 
                                                Choice1Of2(())                                                                  
                                                                       
                                        do! sendMessageToTopic ({ MessageBody = r.MessageBody
                                                                ; ContextProperties = r.ContextProperties
                                                                ; CorrelationId = r.CorrelationId }) inboundCs |> Choice.fromErrorOption                        
                                    
                                        let! message = getMessageFromSubscription outBoundTestSubName outboudCs

                                        do! if r.CreateSubscription then
                                                deleteSubscription r.OutBoundTopicName outBoundTestSubName outboudCs |> Choice.fromErrorOption        
                                            else 
                                                Choice1Of2(())  

                                        return message                                    
                            }                               
                
                

                let testXsltTransformWithConfig name (c : XsltTransformTestConfig) =   
                           testCase name (fun () -> let inputXmlFileName = c.InputXmlFileName |> replaceTestVariables name
                                                    let xsltFileName = c.XsltFileName |> replaceTestVariables name
                                                    let outputXmlFileName = c.OutputXmlFileName |> replaceTestVariables name
                                                    let expectedXmlFileName = c.ExpectedXmlFileName |> replaceTestVariables name

                                                    c.XsltArgList |> toXsltArgList |> transformXsltWithArgs inputXmlFileName xsltFileName outputXmlFileName                                                                 
                                                      
                                                    Expect.isTrue (compareXmlFiles outputXmlFileName expectedXmlFileName) "Output xml is different than Expected Xml")   

                let testQ2QWithConfig name (c : Q2QTestConfig) (validateTestOutput : (Q2QTestConfig * AzureServiceBus.ReceivedMessageResponse) -> unit) = 
                           testCase name <| fun () -> let correlationId = Guid.NewGuid().ToString()                                                                
                                                      let inputMessageFileName = c.InputMessageFileName |> replaceTestVariables name
                                                      let outputFileName = c.OutputMessageFileName |> replaceTestVariables name                                                                                                            
                                                      let inputMessage = File.ReadAllText(inputMessageFileName, System.Text.Encoding.UTF8)
                                                      let outputContextPropertiesFileName = c.OutputContextPropertiesFileName |> replaceTestVariables name
                                                      
                                                      let r = executeQueueToQueueTest ({ InBoundTopicName = c.InBoundTopicName
                                                                                       ; OutBoundTopicName = c.OutBoundTopicName
                                                                                       ; CorrelationId = correlationId
                                                                                       ; ServiceBusConnectionString = c.ServiceBusConnectionString
                                                                                       ; MessageBody = inputMessage
                                                                                       ; ContextProperties = c.InputMessageContextProperties
                                                                                       ; CreateSubscription = true; })                              
                                                      
                                                      Expect.isChoice1Of2 r "Invalid return result"

                                                      let receivedMsg = r |> Choice.get

                                                      Expect.isNotEmpty receivedMsg.MessageBody "The transformed messsage must not be empty"  
                                                      Expect.isTrue (receivedMsg.ContextProperties.Count > 0) "Invalid number of context properties"                                                      

                                                      File.WriteAllText(outputFileName, receivedMsg.MessageBody)
                                                      writeJsonToFile outputContextPropertiesFileName receivedMsg.ContextProperties
                                  
                                                      validateTestOutput(c, receivedMsg)

                open FSharp.Data
                open FSharp.Data.HttpRequestHeaders

               

                let testWebToQueueWithConfig name 
                                             (c : WebToQueueTestConfig)                            
                                             (validateResult : (WebToQueueTestConfig * string * AzureServiceBus.ReceivedMessageResponse) -> unit) = 
                            testCase name <| fun () -> let correlationId = c.CorrelationId
                                                       let outboundTopicTestSubName = Guid.NewGuid().ToString() + "_sub_test"
                                                       
                                                       let outboundTopicConnection : AzureServiceBusConfig = { QueueName = c.OutboundTopicName
                                                                                                             ; ConnectionString = c.OutboundServiceBusConnectionString }
                                                       let result = choose {  
                                                                        do! createSubscription ({ TopicName = c.OutboundTopicName
                                                                                                ; SubscriptionName = outboundTopicTestSubName
                                                                                                ; Filter = Some(upcast CorrelationFilter(correlationId) ) }) outboundTopicConnection 
                                                                            |> function | None -> Choice1Of2(())
                                                                                        | Some(f) -> Choice2Of2(CreateOutboundSubscriptionFailure(f))                      
                                            
                                                                                                                                                                                                                                                                                                                                                                                                                                      
                                                                        let! httpResponse = c.InboundMessage
                                                                                            |> Choice.protect(fun msg -> Http.RequestString(c.InboundUrl, 
                                                                                                                                            httpMethod = HttpMethod.Post,
                                                                                                                                            headers = [ ContentType c.ContentType ], 
                                                                                                                                                        body = TextRequest (msg)))                               
                                                                                            |> Choice.mapSecond (fun ex -> SendInboundMessageFailure(ex.ToString()))                                                     
        
                                                                        let! message = getMessageFromSubscription outboundTopicTestSubName outboundTopicConnection
                                                                                       |> Choice.mapSecond (ReceiveOutboundMessageFailure)

                                                                        do! deleteSubscription c.OutboundTopicName outboundTopicTestSubName outboundTopicConnection
                                                                            |> function | None -> Choice1Of2(())
                                                                                        | Some(f) -> Choice2Of2(DeleteOutboundSubscriptionFailure(f))              
                                                            
                                                                        return httpResponse, message
                                                       }     
                                                      
                                                       let httpResponse, outBoundMessage = result |> Choice.get            
                                  
                                                       validateResult(c, httpResponse, outBoundMessage)
    

