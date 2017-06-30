using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Interprit.Azure.LogicApps.Connectors.BlobConnector
{
    public static class StringExtensions
    {
        public static bool IsValidBlobContainerName(this string containerName)
        {
            Regex regEx = new Regex("^[a-z0-9](?:[a-z0-9]|(\\-(?!\\-))){1,61}[a-z0-9]$|^\\$root$");
            return regEx.IsMatch(containerName);
        }
    }
}