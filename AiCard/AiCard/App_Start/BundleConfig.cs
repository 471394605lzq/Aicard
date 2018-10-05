using System;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace AiCard
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));
            bundles.Add(new StyleBundle("~/Content/css").Include(
                       "~/Content/site.css"));
            bundles.Add(new StyleBundle("~/Content/bgcss").Include(
                        "~/Content/bootstrap.css",
                        "~/Content/site.css"));
            bundles.Add(new ScriptBundle("~/bundles/comm").Include(
                        "~/Scripts/Comm/jsEx.js",
                        "~/Scripts/Comm/jQueryEx.js",
                        "~/Scripts/Comm/canvas-to-blob.min.js",
                        "~/Scripts/Comm/check.js",
                        "~/Scripts/Comm/comm.js",
                        "~/Scripts/Comm/uploadfile.js",
                        "~/Scripts/Comm/imageResizeUpload.js"
                ));

            bundles.Add(new ScriptBundle($"~/bundles/cloud").Include(
              "~/Scripts/Comm/cloud.js"));
            #region 控件
            //control
            //自动命名，如果控件带有css，路径会分为css和js
            Action<string, string[]> addControl = (name, jscss) =>
              {
                  var js = jscss.Where(s => s.ToLower().EndsWith(".js")).ToArray();
                  var css = jscss.Where(s => s.ToLower().EndsWith(".css")).ToArray();
                  if (css.Length > 0)
                  {
                      bundles.Add(new StyleBundle($"~/bundles/{name}/css").Include(css));
                  }
                  if (js.Length > 0)
                  {
                      if (css.Length > 0)
                      {
                          bundles.Add(new ScriptBundle($"~/bundles/{name}/js").Include(js));
                      }
                      else
                      {
                          bundles.Add(new ScriptBundle($"~/bundles/{name}").Include(js));
                      }
                  }
              };
            addControl("datetimepicker", new string[] {
                "~/Scripts/datetimepicker/css/bootstrap-datetimepicker.css",
                "~/Scripts/datetimepicker/js/bootstrap-datetimepicker.js",
                "~/Scripts/datetimepicker/js/locales/bootstrap-datetimepicker.zh-CN.js"
            });
            addControl("cloud", new string[] { "~/Scripts/Comm/cloud.js" });
            #endregion


            //bundles.Add(new StyleBundle("~/bundles/datetimepicker/css").Include(
            //          "~/Scripts/datetimepicker/css/bootstrap-datetimepicker.css"));
            //bundles.Add(new ScriptBundle("~/bundles/datetimepicker/js").Include(
            //         "~/Scripts/datetimepicker/js/bootstrap-datetimepicker.js",
            //         "~/Scripts/datetimepicker/js/locales/bootstrap-datetimepicker.zh-CN.js"));
            #region 页面js
            //view js
            Action<string, string[]> addViewScripts = (name, js) =>
            {
                js = js.Select(s => s.Contains("~") ? s : $"~/Scripts/Views/{s}").ToArray();
                bundles.Add(new ScriptBundle($"~/bundles/{name}").Include(js));
            };

            addViewScripts("roleGroup", new string[] { "roleGroup.js" });
            #endregion


        }
    }
}
