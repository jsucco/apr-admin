using System.Web.Mvc;
using System.Web.Routing;

namespace menu
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{nav}",
                defaults: new { controller = "Home", action = "Index", nav = UrlParameter.Optional }
            );


        }
    }
}
