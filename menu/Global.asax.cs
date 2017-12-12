using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Security.Principal;
using System.Configuration;
using System.Web.Http;

namespace menu
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);     
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            string debugMode = ConfigurationManager.AppSettings["debugMode"];

            if (debugMode == "true")
            {
                GenericIdentity testIdentity = new GenericIdentity(@"textile\jsucco");
                string[] groups = { "Admin" };
                HttpContext.Current.User = new GenericPrincipal(testIdentity, groups);
                return;
            }

            if (HttpContext.Current.Request.Url.AbsolutePath.Contains("api"))
            {
                return;
            }

            HttpCookie authcookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authcookie == null)
            {
                UnAuthorized();
                return;
            }

            

            try
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authcookie.Value);

                if (ticket != null)
                {
                    string[] groups = { "" };
                    HttpContext.Current.User = new GenericPrincipal(new FormsIdentity(ticket), groups);
                }
                else
                {
                    UnAuthorized();
                }
            }
            catch (Exception ex)
            {
                UnAuthorized();
            }
        }

        private void UnAuthorized()
        {
            string LoginAddress = ConfigurationManager.AppSettings["Login"];
            Response.Redirect(LoginAddress + "?returnUrl=" + HttpContext.Current.Request.Url.AbsoluteUri);
        }

    }
}
