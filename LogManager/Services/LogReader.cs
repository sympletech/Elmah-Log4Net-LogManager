using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using LogManager.Helpers;
using LogManager.Services.ServiceModels;

namespace LogManager.Services
{
    public class LogReader
    {
        public List<string> LogPathsLoaded { get; set; }
        public List<string> LogXmlFiles { get; set; }
        public List<LogEntry> LogEntries { get; set; }
        public List<LogGrouping> GroupedLogEntries { get; set; }

        public LogReader()
        {
            this.LogPathsLoaded = new List<string>();
            this.LogXmlFiles = new List<string>();
            this.LogEntries = new List<LogEntry>();
            this.GroupedLogEntries = new List<LogGrouping>();
        }

        public void LoadLogFolder(string environmentName, string logFolder)
        {
            //Remove Existing LogXmlFiles Entries from folder
            var existingLogXmlFiles = this.LogXmlFiles.Where(x => x.StartsWith(logFolder)).ToList();
            foreach (var existingLogXmlFile in existingLogXmlFiles)
            {
                this.LogXmlFiles.Remove(existingLogXmlFile);
            }

            //Remove Existing LogEntries Entries from folder
            var existingLogEntries = this.LogEntries.Where(x => x.FilePath.StartsWith(logFolder)).ToList();
            foreach (var existingLogEntry in existingLogEntries)
            {
                this.LogEntries.Remove(existingLogEntry);
            }

            //Clear The GroupedLogEntries
            this.GroupedLogEntries = new List<LogGrouping>();

            //Read in the new folder 
            var xmlfileList = ReadLogsInFolder(logFolder);
            foreach (var xmlFile in xmlfileList)
            {
                this.LogEntries.Add(ReadLogXmlFile(xmlFile, environmentName, logFolder));
            }
            this.LogEntries = this.LogEntries.OrderByDescending(x => x.Time).ToList();
            GroupLogEntries();

            //Mark Log Folder as loaded
            this.LogPathsLoaded.Add(logFolder);
            this.LogPathsLoaded = this.LogPathsLoaded.Distinct().ToList();
        }

        //-- Get Logs In Folder

        public string[] ReadLogsInFolder(string logFolder)
        {
            if (Directory.Exists(logFolder))
            {
                string[] xmlfileList = Directory.GetFiles(logFolder);
                this.LogXmlFiles.AddRange(xmlfileList);

                return xmlfileList;
            }

            return new string[]{};
        }

        //-- Read Logs

        public LogEntry ReadLogXmlFile(string xmlLogFileLocation, string environmentName, string logFolder)
        {
            var xDoc = new XmlDocument();
            xDoc.Load(xmlLogFileLocation);

            var logEntry = ReadErrorNodeAttributes(xDoc);
            logEntry.FilePath = xmlLogFileLocation;

            var serverVariablesNode = xDoc.SelectNodes("error/serverVariables")[0];
            logEntry.ServerVariables = ReadDetailsNodeCollection(serverVariablesNode);

            var queryStringsNode = xDoc.SelectNodes("error/queryString")[0];
            logEntry.QueryStrings = ReadDetailsNodeCollection(queryStringsNode);

            var cookiesNode = xDoc.SelectNodes("error/cookies")[0];
            logEntry.Cookies = ReadDetailsNodeCollection(cookiesNode);

            logEntry.Url = logEntry.ServerVariables.ContainsKey("URL") ? logEntry.ServerVariables["URL"] : "";
            logEntry.EnvironmentName = environmentName;
            logEntry.LogDirectory = logFolder;

            return logEntry;
        }

        private static LogEntry ReadErrorNodeAttributes(XmlDocument xDoc)
        {
            var xmlNodeList = xDoc.SelectNodes("error");
            if (xmlNodeList != null)
            {
                var errorNode = xmlNodeList[0];
                var logEntry = new LogEntry
                    {
                        ErrorId = errorNode.Attributes["errorId"].GetValue(),
                        Application = errorNode.Attributes["application"].GetValue(),
                        Host = errorNode.Attributes["host"].GetValue(),
                        ErrorType = errorNode.Attributes["type"].GetValue(),
                        Message = errorNode.Attributes["message"].GetValue(),
                        Source = errorNode.Attributes["source"].GetValue(),
                        Detail = errorNode.Attributes["detail"].GetValue(),
                        User = errorNode.Attributes["user"].GetValue(),
                        Time = errorNode.Attributes["time"].GetValue().Parse<DateTime>(),
                        StatusCode = errorNode.Attributes["statusCode"].GetValue()
                    };
                return logEntry;
            }
            else
            {
                return new LogEntry();
            }
        }

        private Dictionary<string, string> ReadDetailsNodeCollection(XmlNode xNode)
        {
            Dictionary<string, string> serverVariables = new Dictionary<string, string>();
            if (xNode != null)
            {
                foreach (XmlNode itemNode in xNode.ChildNodes)
                {
                    string key = itemNode.Attributes["name"].GetValue();
                    string value = itemNode.ChildNodes[0].Attributes["string"].GetValue();

                    serverVariables.Add(key, value);
                }
            }

            return serverVariables;
        }

        //-- Group Logs

        public void GroupLogEntries()
        {
            var groupedEntries = this.LogEntries.GroupBy(x => new{x.Detail, x.Url});
            foreach (var entries in groupedEntries)
            {
                LogEntry lastEntry = entries.First();

                this.GroupedLogEntries.Add(new LogGrouping
                    {
                        LastErrorID = lastEntry.ErrorId,
                        ErrorType = lastEntry.ErrorType,
                        ErrorMessage = lastEntry.Message,//.MaxLength(500),
                        ErrorDetail = lastEntry.Detail,
                        Url = lastEntry.Url,
                        ErrorCount = entries.Count(),
                        LastReport = entries.Max(x=>x.Time.ToString("hh:mm tt | MM/dd/yyyy")),
                        ServerVariables = lastEntry.ServerVariables,
                        QueryStrings = lastEntry.QueryStrings,
                        Cookies = lastEntry.Cookies,
                        LogEntries = entries.Select(x=>new LogEntry
                            {
                                Time = x.Time,
                                ErrorId = x.ErrorId,
                                EnvironmentName = x.EnvironmentName
                            }) .ToList()
                    });
            }
        }

        //-- Clear Logs

        public void ClearLogEntriesInGroup(string lastErrorId)
        {
            var errorEntry = this.LogEntries.First(x => x.ErrorId == lastErrorId);

            var environmentName = errorEntry.EnvironmentName;
            var logDirectory = errorEntry.LogDirectory;
            
            if(errorEntry != null)
            {
                var logEntries = this.LogEntries.Where(x => x.Detail == errorEntry.Detail).ToList();
                foreach (var logEntry in logEntries)
                {
                    File.Delete(logEntry.FilePath);
                    this.LogEntries.Remove(logEntry);
                }
            }
            this.LoadLogFolder(environmentName, logDirectory);
        }        

    
    }
}