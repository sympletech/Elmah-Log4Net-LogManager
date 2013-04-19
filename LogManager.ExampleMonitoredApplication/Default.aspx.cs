using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;


namespace LogManager.ExampleMonitoredApplication
{
    public partial class _Default : System.Web.UI.Page
    {
        ILog _logger;
        ILog Logger
        {
            get
            {
                if (_logger == null)
                {
                    log4net.Config.XmlConfigurator.Configure();
                    _logger = log4net.LogManager.GetLogger(this.GetType());
                }
                return _logger;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Logger.Debug("Page Load Start");

            try
            {
                throw new Exception("Page_Load_Exception");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            Logger.Debug("Page Load End");
        }

        protected void Button1_OnClick(object sender, EventArgs e)
        {
            Logger.Debug("Button1_OnClick Start");
            
            //Throw An Uncaught Exception
            throw new NotImplementedException("Button1_OnClick Exception");
            
            Logger.Debug("Button1_OnClick End");
        }

        protected void Button2_OnClick(object sender, EventArgs e)
        {
            Logger.Debug("Button2_OnClick Start");
            
            //Throw An Exception Caught and Logged
            try
            {
                throw new NotImplementedException("Button2_OnClick Exception");    
            }catch(Exception ex)
            {
                Logger.Error(ex);
            }
            
            
            Logger.Debug("Button2_OnClick End");
        }
    }
}
