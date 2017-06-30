open Expecto
open Interprit.Common.Test.Expecto  
open Interprit.Common.Test
open System     
open FSharp.Data
open FSharpx
open System
open System.IO

let getTests (c : TestConfig) =     
        testList "DGI - PWS To CDM" [
            XsltBasicTestCase "DGI - PWS To CDM - XSLT Basic Test"
                    ({ InputXmlFileName = """..\..\TestData\In\$(TESTCASE_NAME).xml"""
                     ; XsltFileName = """..\..\..\Integration.Account\Maps\PWS.PostGoodsIssue-To-CDM.PostGoodsIssue.xslt"""
                     ; OutputXmlFileName = """..\..\TestData\Out\$(TESTCASE_NAME).xml"""
                     ; ExpectedXmlFileName = """..\..\TestData\Expected\$(TESTCASE_NAME).xml""" 
                     ; XsltArgList = [] }) 
                       
            Q2QBasicTestCase "DGI - PWS To CDM - Q2Q Basic Test" 
                    ({ InputMessageFileName = """..\..\TestData\In\$(TESTCASE_NAME).xml"""            
                     ; InputMessageContextProperties = (getTestDataDirPath(c.Env) + """\In\ """) |> getMessageContextPropertiesFromFile                  
                     ; OutputMessageFileName = """..\..\TestData\Out\$(TESTCASE_NAME).xml"""
                     ; OutputContextPropertiesFileName = """..\..\TestData\Out\$(TESTCASE_NAME).json"""
                     ; InBoundTopicName = "publish-postgoodsreceipt-cdm"
                     ; OutBoundTopicName = "publish-belkinx12xml-outgoing"                         
                     ; ServiceBusConnectionString = c.ServiceBusConnectionString })                      
                    (fun (config, outBoundMsg) -> Expect.isTrue (outBoundMsg.ContextProperties.Count > 0) "Invalid number of context properties")                                                                                              
        ] |> filterTestCasesByConfig c

[<EntryPoint>]
let main args =                            
       args |> getTestConfig 
            |> getTests            
            |> runTests defaultConfig           