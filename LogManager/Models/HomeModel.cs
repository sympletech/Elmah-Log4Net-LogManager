using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LogManager.Models
{
    public class HomeModel
    {
        public List<SelectListItem> LogFolderChoices { get; set; }
    }
}