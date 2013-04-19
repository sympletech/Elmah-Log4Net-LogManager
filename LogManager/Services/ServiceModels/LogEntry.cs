using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogManager.Services.ServiceModels
{
    public class LogEntry
    {
        public string ErrorId { get; set; }
        public string Application { get; set; }
        public string Host { get; set; }
        public string ErrorType { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string Detail { get; set; }
        public string User { get; set; }
        public DateTime Time { get; set; }
        public string StatusCode { get; set; }
        public string Url { get; set; }

        public Dictionary<string, string> ServerVariables { get; set; }
        public Dictionary<string, string> QueryStrings { get; set; }
        public Dictionary<string, string> Cookies { get; set; }

        public string EnvironmentName { get; set; }
        public string LogDirectory { get; set; }
        public string FilePath { get; set; }
    }
}