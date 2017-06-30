#r @"..\Interprit.Common\packages\Newtonsoft.Json\lib\net45\Newtonsoft.Json.dll"
#r @"C:\interprit\interfaces\Integration.Common\Interprit.Common\packages\FSharpx.Collections\lib\net40\FSharpx.Collections.dll"
#r @"C:\interprit\interfaces\Integration.Common\Interprit.Common\packages\FSharpx.Extras\lib\net45\FSharpx.Extras.dll"

open System
open System.IO
open Newtonsoft
open Newtonsoft.Json
open Newtonsoft.Json.Linq
open FSharpx

module JValue =
            let get fieldName (v : obj) = match v with
                                          | :? JObject as x -> match x.[fieldName] with 
                                                               | :? JValue as y -> y
                                                               | _ -> failwith (sprintf "Can't get field %s" fieldName)
                                          | _ -> failwith (sprintf "Can't get field %s" fieldName)                   

            let getString (fieldName : string) (v : obj) = v |> get fieldName |> (fun v -> v.ToString())     

            let isEqual fieldName value (v : obj) = match v with        
                                                    | :? JObject as x -> match x.[fieldName] with
                                                                         | :? JValue as y -> y.Value = value
                                                                         | _ -> false
                                                    | _ -> false 

module JObject = 
            let get fieldName (v : obj) = match v with
                                          | :? JObject as x -> match x.[fieldName] with 
                                                               | :? JObject as y -> y 
                                                               | _ -> failwith (sprintf "Can't get field %s" fieldName)
                                          | _ -> failwith (sprintf "Can't get field %s" fieldName)  

module JArray = 
            let get (fieldName : string) (v : obj) = match v with
                                                     | :? JObject as x -> match x.[fieldName] with 
                                                                          | :? JArray as y -> y 
                                                                          | _ -> failwith (sprintf "Can't get field %s" fieldName)
                                                     | _ -> failwith (sprintf "Can't get field %s" fieldName)     

            let tryGet (fieldName : string) (v : obj) = match v with
                                                        | :? JObject as x -> match x.[fieldName] with 
                                                                             | :? JArray as y -> Some(y) 
                                                                             | null -> None
                                                                             | _ -> failwith (sprintf "Can't get field %s" fieldName)
                                                        | _ -> failwith (sprintf "Can't get field %s" fieldName)  
            let tryFind = Seq.tryFind
            let exists = Seq.exists

let diagnosticResourceJson = """{
    "type": "providers/diagnosticSettings",
    "name": "Microsoft.Insights/service",
    "dependsOn": [
        "[resourceId('Microsoft.Logic/workflows', variables('logicAppName'))]"
    ],
    "apiVersion": "2015-07-01",
    "properties": {
        "serviceBusResourceId": "[variables('eventHubResourceId')]",
        "serviceBusRuleId": "[concat(variables('eventHubResourceId'), '/authorizationrules/', parameters('eventHubRuleId'))]",
        "logs": [
            {
                "category": "WorkflowRuntime",
                "enabled": true,
                "retentionPolicy": {
                    "days": 0,
                    "enabled": false
                }
            }
        ],
        "metrics": [
            {
                "timeGrain": "PT1M",
                "enabled": true,
                "retentionPolicy": {
                    "enabled": true,
                    "days": 1
                }
            }
        ]
    }
}""";;

let dirs = Directory.GetDirectories("""C:\interprit\interfaces\""") 
           |> Array.collect (Directory.GetDirectories)           
           |> Array.collect (Directory.GetDirectories)                    
           |> Array.filter(fun dir -> dir.Contains("LogicApps"))
let updateParametersFileJson (fileName : string) =
            let json = fileName |> File.ReadAllText |> JObject.Parse               
            let parametersJson : JObject = json |> JObject.get "parameters"
            
            if (isNull parametersJson.["eventHubName"]) then
                parametersJson.Add("eventHubName", JObject.Parse("""{ "value ": "eh-common"}"""))
                parametersJson.Add("eventHubRuleId", JObject.Parse("""{ "value": "Reporting"} """))

            (fileName, json.ToString());;                                      


let updateLogicAppFileJson (fileName : string) =
            let json = fileName |> File.ReadAllText |> JObject.Parse               
            let parametersJson = json |> JObject.get "parameters"
            let variablesJson = json |> JObject.get "variables"
            let resourcesJson =  json |> JArray.get "resources"

            if (isNull parametersJson.["eventHubName"]) then                
                parametersJson.Add("eventHubName", JObject.Parse("""{ "value ": "eh-common"}"""))
                parametersJson.Add("eventHubRuleId", JObject.Parse("""{ "value": "Reporting"} """))

            if (isNull variablesJson.["eventHubName"]) then
                variablesJson.Add("eventHubName", JValue("""[concat(parameters('environment'), '-', parameters('eventHubName'))]"""))
                variablesJson.Add("eventHubRuleId", JValue("""[concat('/subscriptions/', subscription().subscriptionId, '/resourceGroups/', variables('commonResourceGroupName'), '/providers/Microsoft.EventHub/namespaces/', variables('eventHubName'))]"""))
            
            let workflow = resourcesJson |> JArray.tryFind  (JValue.isEqual "type" "Microsoft.Logic/workflows")
                                         |> Option.get 
                                         :?> JObject                                                             

            let rs = workflow |> JArray.tryGet "resources" 
                              |> Option.getOrElseF (fun () -> workflow.Add("resources", JArray())
                                                              workflow.["resources"] :?> JArray)

            let exists = rs |> JArray.exists(JValue.isEqual "type" "providers/diagnosticSettings")

            if not exists then rs.Add(JObject.Parse(diagnosticResourceJson))

            (fileName, json.ToString());;

dirs |> Array.collect(Directory.GetFiles)
     |> Array.filter(fun fileName -> fileName.Contains("parameters.json"))    
     |> Array.map(updateParametersFileJson)
     |> Array.iter(fun (fileName, json) -> File.WriteAllText("C:\\Temp\\LogicApps\\" + fileName.[ fileName.LastIndexOf("\\") + 1 .. ], json))

dirs |> Array.collect(Directory.GetFiles)
     |> Array.filter(fun fileName -> (not (fileName.Contains("parameters.json"))))    
     |> Array.map(updateLogicAppFileJson)
     |> Array.iter(fun (fileName, json) -> File.WriteAllText("C:\\Temp\\LogicApps\\" + fileName.[ fileName.LastIndexOf("\\") + 1 .. ], json))
   
// Uploading a 