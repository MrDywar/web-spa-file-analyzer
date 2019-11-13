using Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebServerCore.Infrastructure
{
    public class WebConfigConfigurationManager : IConfigurationManager
    {
        private readonly IConfiguration _configuration;
        private readonly string AppDataPath;

        public WebConfigConfigurationManager(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            AppDataPath = Path.Combine(env.ContentRootPath, "App_Data");
        }

        public string GetAppSetting(string key)
        {
            return _configuration[key];
        }

        public string GetRootFolderPath() => AppDataPath;
    }
}
