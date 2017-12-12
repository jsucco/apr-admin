using System.Web.Mvc;
using menu.Models;
using System.Web.Script.Serialization;
using System.Configuration;
using System.Web.Security;
using System;

namespace menu.Controllers
{
    public class ManageController : Controller
    {
        private JavaScriptSerializer jser = new JavaScriptSerializer();
        private string CID;
        
        [OutputCache(Duration = 604800, Location = System.Web.UI.OutputCacheLocation.Server, VaryByParam ="nav")]
        public ActionResult Index()
        {

            SetCID();

            var nav = (string)this.RouteData.Values["nav"]; 

            Models.ManageIndexViewModel model = getIVM(nav); 

            return View(model);
        }

        public PartialViewResult FlagboardsPartial()
        {
            FbPartialViewModel m = new FbPartialViewModel();

            SetCID(); 

            m = getFbVM();

            return PartialView("_FlagboardsPartial", m);
        }

        public PartialViewResult QAPartial()
        {
            QAPartialViewModel m = new QAPartialViewModel();

            Helpers.InspectionService service = new Helpers.InspectionService();

            m = new QAPartialViewModel()
            {
                wrdata = service.getWorkrooms(),
                locdata = service.getLocations()
            };

            return PartialView("_QAPartial", m); 
        }

        public PartialViewResult ALPartial()
        {
            AlPartialViewModel m = new AlPartialViewModel();

            Helpers.AdminService service = new Helpers.AdminService();

            m = new AlPartialViewModel()
            {
                aldata = service.GetAlertEmails("INS"),
                LocSerOptions = service.GetSerLocs(), 
                ErrorMessage = "ERR: " + service.ErrorMessage
            };

            return PartialView("_AlertsPartial", m); 
        }

        public PartialViewResult UtilityPartial()
        {
            UtilityViewModel m = new UtilityViewModel();

            var url = ConfigurationManager.AppSettings["UTILURL"];

            if (url != null)
                m.IframeUrl = url; 

            return PartialView("_UtilityPartial", m); 
        }

        [HttpPost]
        public JsonResult RefreshWorkrooms()
        {
            Helpers.InspectionService service = new Helpers.InspectionService();
            return Json(service.getWorkrooms()); 
        }

        [HttpPost]
        public JsonResult RefreshFB(string CID)
        {
            Helpers.CtxService service = new Helpers.CtxService(null, CID);
            return Json(service.GetFlagboards()); 
        }

        [HttpPost]
        public JsonResult RefreshEM()
        {
            Helpers.AdminService service = new Helpers.AdminService();
            return Json(service.GetAlertEmails("INS")); 
        }

        [HttpPost]
        public JsonResult ActivateWorkroom(int WorkroomId, int LocationMasterId)
        {
            MethodResponse response = new MethodResponse() { Result = false, ErrorMessage = "Error Activating Workroom" };

            Helpers.InspectionService service = new Helpers.InspectionService();
            response.Result = service.SetActivator(LocationMasterId, WorkroomId, true);
            response.ErrorMessage = service.ErrorMessage; 

            return Json(response); 
        }

        [HttpPost]
        public JsonResult SaveWorkroom(string method, string Name, string Abbreviation, string dataid, string Type, string nav)
        {
            var rObject = new MethodResponse()
            {
                Result = true, 
                ErrorMessage = ""
            };
            Helpers.InspectionService service = new Helpers.InspectionService(); 
            switch(method)
            {
                case "wrlst_add":
                    rObject.Result = service.AddWorkroom(Name, Abbreviation, Type);
                    if (!rObject.Result)
                        rObject.ErrorMessage = service.ErrorMessage;
                    else
                    {
                        //service.AddCachedWorkroom(service.PrimaryKey, Name, Abbreviation);
                        service.AddWorkroomActiv(service.PrimaryKey); 
                    }
                    break;
                case "wrlst_edit":
                    int id = 0;
                    if (int.TryParse(dataid, out id))
                    {
                        rObject.Result = service.EditWorkroom(int.Parse(dataid), Name, Abbreviation);
                        if (!rObject.Result)
                            rObject.ErrorMessage = service.ErrorMessage;
                        //else
                        //    service.EditCachedWorkroom(id, Name, Abbreviation); 
                    }
                        
                     break;
                case "wrlst_delete":
                    int id_ = 0; 
                    if (int.TryParse(dataid, out id_))
                    {
                        rObject.Result = service.DeleteWorkroom(id_);
                        if (!rObject.Result)
                            rObject.ErrorMessage = service.ErrorMessage;
                        else
                        {
                            //service.DeleteCachedWorkroom(id_);
                            service.DelWorkroomActive(id_);
                        }
                            
                    } else
                    {
                        rObject.Result = false;
                        rObject.ErrorMessage = "failed to parse dataid"; 
                    }      
                    break; 
            }

            if (rObject.Result == true)
                removeCacheHTML(nav); 

            return Json(rObject); 
        }

        [HttpPost]
        public JsonResult SaveFB(string method, int MFBId, string Desc, string CID)
        {
            var rObject = new MethodResponse()
            {
                Result = true,
                ErrorMessage = "",
                Content = new Models.WorkroomsUserObject[] { }
            };
            Helpers.CtxService service = new Helpers.CtxService(null, CID);

            try
            {

                if (CID.Length < 6)
                    throw new Exception("could not find location variable");              

                switch (method)
                {
                    case "fb_add":
                        rObject.Result = service.AddFB(Desc);
                        if (!rObject.Result)
                            rObject.ErrorMessage = service.errorMessage;
                        break;
                    case "fb_edit":

                        rObject.Result = service.EditFB(MFBId, Desc);
                        if (!rObject.Result)
                            rObject.ErrorMessage = service.errorMessage;
                        //else
                        //    service.EditCachedWorkroom(id, Name, Abbreviation); 

                        break;
                }
            } catch (Exception ex)
            {
                rObject.Result = false;
                rObject.ErrorMessage = ex.Message; 
            }

            if (rObject.Result == true)
                removeCacheHTML();
            else
                rObject.ErrorMessage = service.errorMessage;

            return Json(rObject);
        }

        [HttpPost]
        public JsonResult SaveEM(string method, string Email, string CID, bool INSCOMP, bool AUTOCOMP, bool INSCOMPEX, string Code, string nav)
        {
            var rObject = new MethodResponse()
            {
                Result = true,
                ErrorMessage = "",
                Content = new Models.WorkroomsUserObject[] { }
            };

            Helpers.AdminService service = new Helpers.AdminService();
            try
            {

                if (Code.Trim().Length == 0)
                    throw new Exception("alert code not recieved");

                if (Email.Trim().Length == 0)
                    throw new Exception("Email address not recieved");

                var aO = new AlertManager()
                {
                    Email = Email.Trim(),
                    Code = Code,
                    CID = CID, 
                    INSCOMP = INSCOMP,
                    AUTOCOMP = AUTOCOMP,
                    INSCOMPEX = INSCOMPEX,
                };          
                
                switch(method)
                {
                    case "em_add":
                        rObject.Result = service.AddEmail(aO); 
                        break;
                    case "em_edit":
                        rObject.Result = service.EditActiveAlert(aO); 
                        break;
                    case "em_delete":
                        rObject.Result = service.DeleteAAEmail(aO); 
                        break; 
                    default:
                        rObject.ErrorMessage = "control handler error";
                        rObject.Result = false; 
                        break;
                }
            } catch (Exception ex)
            {
                rObject.Result = false;
                rObject.ErrorMessage = ex.Message; 
            }

            if (rObject.Result == true)
                removeCacheHTML(nav);
            else
                rObject.ErrorMessage = service.ErrorMessage; 

            return Json(rObject); 
        }

        [HttpPost]
        public ActionResult GetWorkroomsSubgrid()
        {
            int locid, RowNum, PageNum = 0;

            Models.jqdata response = new Models.jqdata();

            var loc_qv = HttpContext.Request.Params["LocationId"];
            var rn_qv = HttpContext.Request.Params["RowNum"];
            var pg_qv = HttpContext.Request.Params["PageNum"]; 

            if (int.TryParse(loc_qv, out locid) && int.TryParse(rn_qv, out RowNum) && int.TryParse(pg_qv, out PageNum))
            {
                Helpers.InspectionService service = new Helpers.InspectionService();
                var arr = service.GetWorkroomActivators(locid);

                if (arr.Length > 0)
                {
                    response.total = System.Math.Round((double)arr.Length / (double)RowNum);
                    response.page = PageNum;
                    response.records = arr.Length;
                    response.rows = arr;
                }

                return Json(response, JsonRequestBehavior.AllowGet);
            }
            return Json(new Models.EF.WorkroomActivator[] { }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult EditWorkroom()
        {
            var rObject = new MethodResponse()
            {
                Result = true,
                ErrorMessage = ""
            };

            int locid, wrid = 0;
            var loc_qv = HttpContext.Request.Params["LocationId"];
            var wr_qv = HttpContext.Request.Params["Id"];
            var cid = HttpContext.Request.Params["CIDv"];

            if (int.TryParse(loc_qv, out locid) && int.TryParse(wr_qv, out wrid))
            {
                bool status = false; 
                if (bool.TryParse(HttpContext.Request.Params["Status"], out status))
                {
                    Helpers.InspectionService service = new Helpers.InspectionService();

                    rObject.Result = service.SetActivator(locid, wrid, status);

                    if (rObject.Result && cid.Length == 6)
                    {
                        service.ClearCachedWorkroomUserObject(cid);
                        service.CacheWorkroomUserObject(cid); 
                    }                

                    rObject.ErrorMessage = service.ErrorMessage; 
                } else
                {
                    rObject.Result = false;
                    rObject.ErrorMessage = "Error Parsing Status";
                    return Json(rObject, JsonRequestBehavior.AllowGet);
                }
            } else
            {
                rObject.Result = false;
                rObject.ErrorMessage = "Error Parsing Index id's";
                return Json(rObject, JsonRequestBehavior.AllowGet); 
            }
            return Json(true, JsonRequestBehavior.AllowGet); 
        }

        [HttpPost]
        public ActionResult Index(ManageIndexViewModel model, string navitem)
        {
            if (navitem == "nav-row-back")
                return RedirectToMenu();

            model = getIVM(navitem);

            setNavParam(navitem);

            if (!ValidateAdminPriv())
                return RedirectToMenu();

            return View(model); 
        }

        #region helpers

        private string getWorkroomOptions()
        {
            Helpers.InspectionService s = new Helpers.InspectionService();

            string[] wrs = s.GetCached400Workrooms();

            if (wrs == null)
            {
                wrs = s.Get400Workrooms();
                s.Cache400Workrooms(wrs); 
            }

            if (wrs != null && wrs.Length > 0)
            {
                string wrStr = ""; 
                for (var i = 0; i < wrs.Length; i++)
                {
                    if (i == wrs.Length - 1)
                        wrStr = wrStr + wrs[i] + ":" + wrs[i];
                    else
                        wrStr = wrStr + wrs[i] + ":" + wrs[i] + ";";
                }
                return wrStr;
            }  

            return "";
        }

        private Models.ManageIndexViewModel getIVM(string navitem = "qa")
        {
            Models.ManageIndexViewModel m = new Models.ManageIndexViewModel();

            var sess = (string)Session["nav"];

            navitem = (navitem == null) ? sess : navitem; 

            switch (navitem)
            {
                case "utility":
                    m.UtilitySelected = true;
                    setNavParam(navitem);
                    break;
                case "alerts":
                    m.AlertsSelected = true;
                    setNavParam(navitem);
                    break;
                case "flagboards":
                    m.FlagboardsSelected = true;
                    setNavParam(navitem);
                    break;
                case "activity":
                    m.ActivitySelected = true;
                    setNavParam(navitem);
                    break;
                case "qa":
                    m.QASelected = true;
                    setNavParam(navitem);
                    break;
                default:
                    m.QASelected = true;
                    break; 
            }

            m.NavRowSel = navitem; 

            return m;
        }

        private Models.FbPartialViewModel getFbVM()
        {
            Models.FbPartialViewModel m = new Models.FbPartialViewModel();

            Helpers.CtxManagerService s = new Helpers.CtxManagerService();

            if (CID != null && CID.Length == 6)
            {
                Helpers.CtxService ctxs = new Helpers.CtxService(null, CID);

                m.fbdata = ctxs.GetFlagboards();
                m.SelCID = CID;        
            }
            else
            {
                m.fbdata = new Models.CtxEF.Maintenance_Flagboard[] { };
                m.SelCID = "000000";
            }

            removeCacheHTML("flagboards");

            m.corpdata = s.getLocations();  

            return m; 
        }

        private bool ValidateAdminPriv()
        {
            string debugMode = ConfigurationManager.AppSettings["debugMode"];

            if (debugMode == "true")
                return true; 

            var authcookie = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authcookie == null)
            {
                RedirectToMenu();
            }

            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authcookie.Value);

            if (ticket.Name.Length == 0)
                RedirectToMenu();

            Helpers.AdminService service = new Helpers.AdminService();

            Models.EF.EmailMaster[] admins = service.GetAdminEmails();
            var rawUserName = service.ParseRawUserName(ticket.Name);

            foreach (Models.EF.EmailMaster item in admins)
            {
                var rawAdminUser = item.Address.Split(new char[] { '@' });

                if (rawAdminUser.Length > 1)
                {
                    if (rawUserName == rawAdminUser[0])
                        return true; 
                }                 
            }
            return false; 
        }

        private RedirectResult RedirectToMenu()
        {
            var menuUrl = ConfigurationManager.AppSettings["APRMENUURL"]; 
            if (menuUrl == null || menuUrl.Length == 0)
            {
                throw new System.Exception("AppSetting APRMENUURL cannot be null or length of zero"); 
            }
            CID = (Session["CID"] != null) ? Session["CID"].ToString() : CID; 
            return Redirect(menuUrl + "?UC=" + CID); 
        }

        private void SetCID()
        {
            if (HttpContext.Request.QueryString["CID"] != null)
            {
                CID = HttpContext.Request.QueryString["CID"];
                Session["CID"] = CID;
            }
        }

        private void setNavParam(string nav)
        {
            if (nav != null && nav.Length > 0)
            {
                Session["nav"] = nav;
            }
        }

        private void removeCacheHTML(string nu = "")
        {
            try
            {
                var nav = (nu.Length == 0) ? Session["nav"] : nu;
                if (nav != null)
                {
                    string sn = (string)nav;
                    var url = Url.Action(
                        "Index",
                        "Manage",
                        new { nav=sn }
                        );
                    Response.RemoveOutputCacheItem(url);
                }
                
            } catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); 
            }

        }
        #endregion

    }

}