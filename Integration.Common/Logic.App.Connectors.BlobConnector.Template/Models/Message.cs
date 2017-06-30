using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace $safeprojectname$.Models
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