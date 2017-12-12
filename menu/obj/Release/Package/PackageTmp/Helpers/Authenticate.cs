using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using System.Web.Security;
using System.DirectoryServices.AccountManagement;
using System.Threading.Tasks;
namespace menu.Helpers
{
    public class Authenticate
    {
        public static async Task<bool> ActiveDirectory(string Username, string Password)
        {
            bool Authenticated = false;

            if (!string.IsNullOrEmpty(Username) || !string.IsNullOrEmpty(Password))
            {
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, "standardtextile.com", null, ContextOptions.Negotiate))
                {
                    if (pc.ValidateCredentials(Username, Password, ContextOptions.SimpleBind))
                    {
                        AddAuthCookie(Username);
                        GenericIdentity id = new GenericIdentity(Username, "LdapAuthentication");
                        string[] groups = {""};
                        GenericPrincipal principal = new GenericPrincipal(id, groups);
                        HttpContext.Current.User = principal;
                        Authenticated = true;
                    }
                }
            }

            return Authenticated;
        }
        public static void AddAuthCookie(string Username)
        {
            FormsAuthentication.Initialize();
            DateTime expires = DateTime.Now.AddYears(2);
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                                                Username,
                                                DateTime.Now, 
                                                expires,
                                                true,
                                                ".STCSSOAUTH",
                                                FormsAuthentication.FormsCookiePath);
            string encryptedTicket = FormsAuthentication.Encrypt(ticket);
            HttpCookie authcookie = new HttpCookie(FormsAuthentication.FormsCookieName,
                                                    encryptedTicket);
            authcookie.Expires = expires;
            HttpContext.Current.Response.Cookies.Add(authcookie);
        }
    }
}