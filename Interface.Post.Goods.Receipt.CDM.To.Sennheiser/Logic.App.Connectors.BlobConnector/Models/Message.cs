using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Interprit.Azure.LogicApps.Connectors.BlobConnector.Models
{
    public class Message
    {
        [Required]
        public string BlobName { get; set; }

        [Required]
        public string MessageBody { get; set; }

        [Required]
        public string ContainerName { get; set; }
    }
}