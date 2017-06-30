open Expecto
open Interprit.Common.Test.Expecto  
open Interprit.Common.Test
open System     
open FSharp.Data
open FSharpx
open System
      
let getTests (c : TestConfig) = 
        testList "PGR - CDM To Belkin" [
            testXsltTransformWithConfig "PGR - CDM To Belkin - XSLT Basic Test"
                              ({ InputXmlFileName = """..\..\TestData\In\$(TESTCASE_NAME).xml"""
                               ; XsltFileName = """..\..\..\Integration.Account\Maps\CDM.PostGoodsReceipt-To-Belkin.X12.00401.944.xslt"""
                               ; OutputXmlFileName = """..\..\TestData\Out\$(TESTCASE_NAME).xml"""
                               ; ExpectedXmlFileName = """..\..\TestData\Expected\$(TESTCASE_NAME).xml""" 
                               ; XsltArgList = [] })      

            testQ2QWithConfig "PGR - CDM To Belkin - Q2Q Basic Test" 
                              ({ InputMessageFileName = """..\..\TestData\In\$(TESTCASE_NAME).xml"""            
                               ; InputMessageContextProperties = (getTestDataDirPath(c.Env) + """\In\PGR - CDM To Belkin - Q2Q Basic Test - Context Properties.txt""") |> getMessageContextPropertiesFromFile                  
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