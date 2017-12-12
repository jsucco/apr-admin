using System;
using System.Web.Mvc;
using System.Web.Security;

namespace menu.Controllers
{
    public class AccountController : Controller
    {
        [HttpPost]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            Response.Cookies["APRKeepMeIn"].Expires = DateTime.Now.AddDays(-1);
            return Redirect("http://apr.standardtextile.com/Login.aspx?returnUrl=menu.standardtextile.com");
        }
    }
}