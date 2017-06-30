open Expecto
open Interprit.Common.Test.Expecto  
open Interprit.Common.Test
open System     
open FSharp.Data
open FSharpx
open System

//type CDMSchema = XmlProvider<"TestData/Schemas/CDM.xml">

let getTests (c : TestConfig) =   
        testList "PGR - PWS To CDM " [
            testXsltTransformWithConfig "PGR - PWS To CDM - XSLT Basic Test"
                              ({ InputXmlFileName = """..\..\TestData\In\$(TESTCASE_NAME).xml"""
                               ; XsltFileName = """..\..\..\Integration.Account\Maps\PWS.PostGoodsReceipt-To-CDM.PostGoodsReceipt.xslt"""
                               ; OutputXmlFileName = """..\..\TestData\Out\$(TESTCASE_NAME).xml"""
                               ; ExpectedXmlFileName = """..\..\TestData\Expected\$(TESTCASE_NAME).xml""" 
                               ; XsltArgList = [] })  
                               
            testQ2QWithConfig "PGR - PWS To CDM - Q2Q Basic Test" 
                             ({ InputMessageFileName = """..\..\TestData\In\$(TESTCASE_NAME).xml"""            
                              ; InputMessageContextProperties = """..\..\TestData\In\ASN - PWS To CDM - Q2Q Basic Test - Context Properties.txt""" |> getMessageContextPropertiesFromFile                  
                              ; OutputMessageFileName = """..\..\TestData\Out\$(TESTCASE_NAME).xml"""
                              ; OutputContextPropertiesFileName = """..\..\TestData\Out\$(TESTCASE_NAME).json"""
                              ; InBoundTopicName = "publish-postgoodsreceipt-pwssoap"
                              ; OutBoundTopicName = "publish-postgoodsreceipt-cdm"                         
                              ; ServiceBusConnectionString = c.ServiceBusConnectionString })                      
                              (fun (config, outBoundMsg) -> //let cdm = CDMSchema.Parse(outBoundMsg.MessageBody)
                                                            //Expect.isTrue (cdm.MessageHeader.GlnCode = "22868") "Invalid GlnCode"                                                                                                
                                                            Expect.isTrue (outBoundMsg.ContextProperties.Count > 0) "Invalid number of context properties")                                   
        ] |> filterTestCasesByConfig c 
        
[<EntryPoint>]
let main args =                            
        args |> getTestConfig 
             |> getTests            
             |> runTests defaultConfig                                      

