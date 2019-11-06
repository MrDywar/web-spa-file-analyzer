using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;

namespace WebServer.App_Start
{
    public class WebConfigConfigurationManager : IConfigurationManager
    {
        public string GetAppSetting(string key)
        {
            return WebConfigurationManager.AppSettings[key];
        }

        public string GetRootFolderPath()
        {
            return HostingEnvironment.MapPath("~/App_Data");
        }
    }
}