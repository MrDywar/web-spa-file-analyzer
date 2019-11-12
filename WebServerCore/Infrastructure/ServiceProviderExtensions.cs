using Core;
using Core.FileProcessor;
using Core.Implementation;
using Core.Implementation.FileProcessor;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebServerCore.Infrastructure
{
    public static class ServiceProviderExtensions
    {
        public static void AddFileAnalyser(this IServiceCollection services)
        {
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IFileProcessorFactory, FileProcessorFactory>();
            services.AddSingleton<IConfigurationManager, WebConfigConfigurationManager>();
        }
    }
}
