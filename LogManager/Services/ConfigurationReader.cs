using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using LogManager.Helpers;

namespace LogManager.Services
{
    public class ConfigurationReader
    {
        public static List<LogPathEntry> ReadLogFolders()
        {
            var results = new List<LogPathEntry>();

            var logPathsLocation = HttpContext.Current.Server.MapPath(@"/App_Data/LogPaths.xml");
            var xDoc = new XmlDocument();
            xDoc.Load(logPathsLocation);

            XmlNode logPathsNode = xDoc.SelectNodes("LogPaths")[0];
            foreach (XmlNode pathNode in logPathsNode.ChildNodes)
            {
                results.Add(new LogPathEntry
                    {
                        Name = pathNode.Attributes["name"].GetValue(),
                        Path = pathNode.Attributes["path"].GetValue(),
                        Default = pathNode.Attributes["default"].GetValue() == "true"
                    });
            }

            return results;
        }

        public class LogPathEntry
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public bool Default { get; set; }
        }
    }


}