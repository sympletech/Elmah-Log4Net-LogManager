using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LogManager.Models;
using LogManager.Services;
using System.Web;
using LogManager.Services.ServiceModels;
using LogManager.Helpers;

namespace LogManager.Controllers.api
{
    public class LogEntriesController : ApiController
    {
        const int ITEMSPERPAGE = 10;
        
        public LogReader myLogReader
        {
            get
            {
                if (HttpContext.Current.Cache["LogReader"] == null)
                {
                    HttpContext.Current.Cache["LogReader"] = new LogReader();
                }
                return (LogReader)HttpContext.Current.Cache["LogReader"];
            }
            set { HttpContext.Current.Cache["LogReader"] = value; }
        }

        // GET api/logentries
        public object Get(string currentLogFolder, string searchTerm, int page, bool reload)
        {
            if (string.IsNullOrEmpty(currentLogFolder))
            {
                return "";
            }

            if (reload == true)
            {
                myLogReader = new LogReader();
            }

            //Load Log Folder (if not already loaded)
            if (myLogReader.LogPathsLoaded.Contains(currentLogFolder) != true)
            {
                var logFolders = ConfigurationReader.ReadLogFolders();
                var folderEntry = logFolders.First(x => x.Name == currentLogFolder);
                if (folderEntry != null)
                {
                    myLogReader.LoadLogFolder(folderEntry.Name, folderEntry.Path);
                }
            }

            var results = myLogReader.GroupedLogEntries.ToList();

            //Apply Search Critera if provided
            if(string.IsNullOrEmpty(searchTerm) != true)
            {
                results = results.Where(x => 
                    x.ErrorMessage.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    x.ErrorDetail.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            int pageCount = results.Count / ITEMSPERPAGE;
            if((results.Count % ITEMSPERPAGE) > 0)
            {
                pageCount++;
            }

            //Apply Paging
            int skip = (page - 1) * ITEMSPERPAGE;
            results = results.Skip(skip).Take(ITEMSPERPAGE).ToList();

            return new
                {
                    Items = results,
                    PageCount = pageCount
                };
        }

        // GET api/logentries/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/logentries
        public void Post([FromBody]string value)
        {
        }

        // PUT api/logentries/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/logentries/5
        public void Delete(string id)
        {
            myLogReader.ClearLogEntriesInGroup(id);
        }
    

        public string Test()
        {
            return "Hello";
        }
    
    
    }
}
