using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Interprit.Azure.LogicApps.Connectors.BlobConnector.Models
{
    public class MessageProperties
    {
        public string From { get; set; }
        public string To { get; set; }
        public string AS2From { get; set; }
        public string AS2To { get; set; }
        public string X12From { get; set; }
        public string X12To { get; set; }
        public string ReceiverPartnerName { get; set; }
        public string SenderPartnerName { get; set; }
        public string Agreement { get; set; }
        public string DocumentType { get; set; }
        public string DocumentSubType { get; set; }
        public string EntityVersion { get; set; }
        public string InterchangeControlNumber { get; set; }
        public string ServiceBusTriggerContent { get; set; }
        public string CorrelationId { get; set; }
        public string Interface { get; set; }
        public string MessageType { get; set; }
        public string ReceivedDateTime { get; set; }
        public string Status { get; set; }
        public string TestIndicator { get; set; }
        public string Q2QTestIndicator { get; set; }
        public string WorkflowGuid { get; set; }
        public string WorkflowName { get; set; }
        public string GLNCode { get; set; }
        public string CustomerName { get; set; }
        public string WarehouseLocationCode { get; set; }
        public string AssociatedServiceBusTopicName { get; set; }
        public string CustomerMessageId { get; set; }
    }
}