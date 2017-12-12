using System.Web.Optimization; 

namespace menu
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/jqGrid").Include(
                        "~/Scripts/grid.locale-en.js",
                        "~/Scripts/jquery.jqGrid.min.js"
                ));
        }
    }
}