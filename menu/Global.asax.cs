using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Security.Principal;
using System.Configuration;
using System.Web.Http;
using System.Collections.Generic;

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

                Helpers.AdminService service = new Helpers.AdminService();

                service.AddAdmin(new Models.AdminsGridView()
                {
                    Address = "test",
                    Status = false
                });

                return;
            }

            //if (HttpContext.Current.Request.Url.AbsolutePath.Contains("api"))
            //{
            //    return;
            //}

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

                    if (!ValidateAdminPriv())
                        RedirectToMenu();
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

        private bool ValidateAdminPriv()
        {
            string debugMode = ConfigurationManager.AppSettings["debugMode"];

            if (debugMode == "true")
                return true;

            HttpCookie authcookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authcookie == null)
            {
                RedirectToMenu();
            }

            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authcookie.Value);

            if (ticket == null)
                RedirectToMenu();

            Helpers.AdminService service = new Helpers.AdminService();

            service.AddAdmin(new Models.AdminsGridView()
            {
                Address = ticket.Name,
                Status = false
            });         

            HashSet<string> admins = service.GetAdminHash();

            var rawUserName = service.ParseRawUserName(ticket.Name);    

            if (admins.Contains(rawUserName.ToUpper()))
            {
                Session["username"] = rawUserName; 
                return true;
            }
                

            return false;
        }

        private void RedirectToMenu()
        {
            var menuUrl = ConfigurationManager.AppSettings["APRMENUURL"];

            if (menuUrl == null || menuUrl.Length == 0)
            {
                throw new System.Exception("AppSetting APRMENUURL cannot be null or length of zero");
            }

            Response.Redirect(menuUrl);
        }
    }
}
