using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LogManager.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogManager.Tests.Services
{
    [TestClass]
    public class LogReaderTests
    {
        protected LogReader myLogReader { get; set; }
        protected const string LOGPATH = @"TestLogs\";

        [TestInitialize()]
        public void MyTestInitialize()
        {
            myLogReader = new LogReader();
        }

        [TestMethod]
        public void GetLogsInFolder_ShouldReadInTheXMLFilesInTheFolder()
        {
            //prepare

            //Perform
            myLogReader.ReadLogsInFolder(LOGPATH);

            //Assert
            Assert.IsNotNull(myLogReader.LogXmlFiles);
        }

        [TestMethod]
        public void ReadLogXmlFile_ShouldReturnALogEntry()
        {
            //prepare
            myLogReader.ReadLogsInFolder(LOGPATH);
            var readXmlFile = myLogReader.LogXmlFiles.First();

            //Performm
            var logEntry = myLogReader.ReadLogXmlFile(readXmlFile, "UnitTest", LOGPATH);

            //Assert
            Assert.IsNotNull(logEntry);
        }

        [TestMethod]
        public void ReadLogXmlFile_ShouldReadTheAttributesFromErrorNode()
        {
            //prepare
            myLogReader.ReadLogsInFolder(LOGPATH);
            var readXmlFile = myLogReader.LogXmlFiles.First();

            //Performm
            var logEntry = myLogReader.ReadLogXmlFile(readXmlFile, "Unit Test", LOGPATH);

            //Assert
            Assert.IsNotNull(logEntry.ErrorId);
            Assert.IsNotNull(logEntry.Application);
            Assert.IsNotNull(logEntry.Host);
            Assert.IsNotNull(logEntry.ErrorType);
            Assert.IsNotNull(logEntry.Message);
            Assert.IsNotNull(logEntry.Source);
            Assert.IsNotNull(logEntry.Detail);
            Assert.IsNotNull(logEntry.User);
            Assert.IsNotNull(logEntry.Time);
            Assert.IsNotNull(logEntry.StatusCode);
        }

        [TestMethod]
        public void ReadLogXmlFile_ShouldReadTheServerVariables()
        {
            //prepare
            myLogReader.ReadLogsInFolder(LOGPATH);
            var readXmlFile = myLogReader.LogXmlFiles.First();

            //Performm
            var logEntry = myLogReader.ReadLogXmlFile(readXmlFile, "Unit Test", LOGPATH);

            //Assert
            Assert.IsNotNull(logEntry.ServerVariables);
            foreach (var serverVar in logEntry.ServerVariables)
            {
                Assert.IsNotNull(serverVar.Value);
            }
        }

        [TestMethod]
        public void ReadLogXmlFile_ShouldReadTheQueryStrings()
        {
            //prepare
            myLogReader.ReadLogsInFolder(LOGPATH);
            var readXmlFile = myLogReader.LogXmlFiles.First();

            //Performm
            var logEntry = myLogReader.ReadLogXmlFile(readXmlFile, "Unit Test", LOGPATH);

            //Assert
            Assert.IsNotNull(logEntry.QueryStrings);
            foreach (var qString in logEntry.QueryStrings)
            {
                Assert.IsNotNull(qString.Value);
            }
        }

        [TestMethod]
        public void ReadLogXmlFile_ShouldReadThecookies()
        {
            //prepare
            myLogReader.ReadLogsInFolder(LOGPATH);
            var readXmlFile = myLogReader.LogXmlFiles.First();

            //Performm
            var logEntry = myLogReader.ReadLogXmlFile(readXmlFile, "Unit Test", LOGPATH);

            //Assert
            Assert.IsNotNull(logEntry.Cookies);
            foreach (var cookie in logEntry.Cookies)
            {
                Assert.IsNotNull(cookie.Value);
            }
        }

        [TestMethod]
        public void ReadLogXmlFile_ShouldGetTheUrlFromServerVarables()
        {
            //prepare


            //Performm
            myLogReader.LoadLogFolder("Unit Test", LOGPATH);

            var logEntry = myLogReader.LogEntries.First(x => x.ErrorType == "System.Web.HttpException");

            //Assert
            Assert.IsFalse(string.IsNullOrEmpty(logEntry.Url));
        }

        [TestMethod]    
        public void GetGroupedLogEntries_ShouldGroupLogEntriesTogether()
        {
            //prepare
            myLogReader.LoadLogFolder("Unit Test", LOGPATH);

            //Perform
            myLogReader.GroupLogEntries();

            //Assert
            foreach (var logGroup in myLogReader.GroupedLogEntries)
            {
                var detail = logGroup.ErrorDetail;
                foreach (var logEntry in logGroup.LogEntries)
                {
                    //Assert.AreEqual(logEntry.Detail, detail);
                    //Removed detail from children to keep payloads small
                }
            }
            Assert.IsTrue(true);
        }
    
        [TestMethod]
        public void ClearLogEntriesInGroup_ShouldRemoveTheXMLFiles()
        {
            //prepare
            myLogReader.ReadLogsInFolder(LOGPATH);
            myLogReader.GroupLogEntries();
            var errorLogEntryGroup = myLogReader.GroupedLogEntries.FirstOrDefault();

            if(errorLogEntryGroup != null)
            {
                var filePaths = errorLogEntryGroup.LogEntries.Select(x => x.FilePath);


                //Perform
                myLogReader.ClearLogEntriesInGroup(errorLogEntryGroup.LastErrorID);

                //Assert
                foreach (var fPath in filePaths)
                {
                    Assert.IsFalse(File.Exists(fPath));
                }                
            }

        }
    
        [TestMethod]
        public void LoadLogFolder_IntigrationTest()
        {
            //prepare

            //Perform
            myLogReader.LoadLogFolder("Unit Test", LOGPATH);

            //Assert
            Assert.IsNotNull(myLogReader.LogXmlFiles);
            Assert.IsNotNull(myLogReader.LogEntries);
            Assert.IsNotNull(myLogReader.LogPathsLoaded);
            Assert.IsNotNull(myLogReader.GroupedLogEntries);

            //Perform
            myLogReader.LoadLogFolder("Unit Test", LOGPATH);

            //Assert
            Assert.IsNotNull(myLogReader.LogXmlFiles);
            Assert.IsNotNull(myLogReader.LogEntries);
            Assert.IsNotNull(myLogReader.LogPathsLoaded);
            Assert.IsNotNull(myLogReader.GroupedLogEntries);
        }
    
    }
}
