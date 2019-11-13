using Autofac;
using Autofac.Integration.WebApi;
using Core;
using Core.FileProcessor;
using Core.Implementation;
using Core.Implementation.FileProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using WebServer.Infrastructure;

namespace WebServer.App_Start
{
    public class AutofacConfig
    {
        public static void ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            var config = GlobalConfiguration.Configuration;
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterWebApiFilterProvider(config);
            builder.RegisterWebApiModelBinderProvider();

            builder.RegisterType<WebApiExceptionFilter>().AsWebApiExceptionFilterForAllControllers().SingleInstance();

            builder.RegisterType<FileService>().As<IFileService>().SingleInstance();
            builder.RegisterType<FileProcessorFactory>().As<IFileProcessorFactory>().SingleInstance();
            builder.RegisterType<WebConfigConfigurationManager>().As<IConfigurationManager>().SingleInstance();

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}