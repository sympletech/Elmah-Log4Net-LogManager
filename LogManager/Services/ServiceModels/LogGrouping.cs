using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogManager.Services.ServiceModels
{
    public class LogGrouping
    {
        public string LastErrorID { get; set; }
        public string ErrorType { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDetail { get; set; }
        public string Url { get; set; }

        public int ErrorCount { get; set; }
        public String LastReport { get; set; }

        public List<LogEntry> LogEntries { get; set; }

        public Dictionary<string, string> ServerVariables { get; set; }
        public Dictionary<string, string> QueryStrings { get; set; }
        public Dictionary<string, string> Cookies { get; set; }
    }
}