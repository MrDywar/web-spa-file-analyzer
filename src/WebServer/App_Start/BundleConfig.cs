using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace WebServer.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Bundle goes without “js/css Minify” transform.

            // Scripts
            bundles.Add(new Bundle("~/bundles/angularRuntime").Include("~/Scripts/Angular/runtime*"));
            bundles.Add(new Bundle("~/bundles/angularPolyfills").Include("~/Scripts/Angular/polyfills*"));
            bundles.Add(new Bundle("~/bundles/angularVendor").Include("~/Scripts/Angular/vendor*"));
            bundles.Add(new Bundle("~/bundles/angularMain").Include("~/Scripts/Angular/main*"));

            // Styles
            bundles.Add(new Bundle("~/bundles/angularStyles").Include("~/Scripts/Angular/styles*"));
        }
    }
}