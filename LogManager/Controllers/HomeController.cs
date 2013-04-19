using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LogManager.Models;
using LogManager.Services;

namespace LogManager.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            List<SelectListItem> ddlLogFolders = ConfigurationReader.ReadLogFolders().Select(x => new SelectListItem { Text = x.Name, Value = x.Name }).ToList();

            HomeModel viewModel = new HomeModel
                {
                    LogFolderChoices = ddlLogFolders,
                };

            return View(viewModel);
        }
    }
}
