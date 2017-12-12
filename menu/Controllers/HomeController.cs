using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace menu.Controllers
{
    public class HomeController : Controller
    {

        [OutputCache(Duration = 259200, VaryByParam = "none")]
        public ActionResult Index()
        {
            mapUserLocation();
            menu.Models.ApplicationsViewModel apps = new menu.Models.ApplicationsViewModel();

            loadAppModel2(apps);
            //apps.CorporateName = Helpers.UserActivity.getCorporateName();
            //Helpers.UserActivity.log("user", "MENU", "SiteEntry");
            return View(apps);
        }

        private static void loadAppModel2(Models.ApplicationsViewModel model)
        {
            List<Models.ButtonModel> toinclude = new List<Models.ButtonModel>();
            try
            {
                addButton(toinclude, "CONTROLTEX", "CONTROLTEX", "systems", "http://STCCAR.controltex.com/", 1, 1, 1, "CTX");
                addButton(toinclude, "QA", "QA", "search", "NA", 1, 2, 1, "QA");
                addDropdown(toinclude, "QA", "INSPECTION", "INSPECTION", "qa-dropdown", "http://apr.standardtextile.com/APP/Mob/SPCInspectionInput.aspx");
                addDropdown(toinclude, "QA", "UTILITY", "UTILITY", "qa-dropdown", "http://apr.standardtextile.com/APP/DataEntry/SPCInspectionUtility.aspx");
                addDropdown(toinclude, "QA", "RESULTS", "RESULTS", "qa-dropdown", "http://apr.standardtextile.com/APP/Presentation/InspectionVisualizer.aspx"); 
                addButton(toinclude, "ADMIN", "ADMIN", "systems", "/Manage/Index", 1, 1, 1, "ADMIN");
                addButton(toinclude, "FLAGBOARD", "FLAGBOARD", "maintenance", "http://maintenance.standardtextile.com", 1, 3, 1, "FLG");
                addButton(toinclude, "DATA-AUTOMATIONS", "DATAAUTOMATIONS", "excel", "http://m.standardtextile.com/dataautomations/launch.aspx?ReportType=A", 1, 4, 1, "DA");
                model.Apps = toinclude as IList<Models.ButtonModel>;
            }
            catch (Exception ex)
            {
                //RedirectToAction("error", new { message = ex.Message, returnUrl = "/Home/Index" });
            }
        }

        private static void loadAppModel(Models.ApplicationsViewModel model)
        {
            List<Models.ButtonModel> toinclude = new List<Models.ButtonModel>(); 
            try
            {
                addButton(toinclude, "CONTROLTEX", "CONTROLTEX", "systems", "http://STCCAR.controltex.com/", 1, 2, 1, "CTX");
                addButton(toinclude, "QA INSPECTION", "INSPECTION", "search", "http://apr.standardtextile.com/APP/Mob/SPCInspectionInput.aspx", 1, 1, 1, "QA");
                addButton(toinclude, "ADMIN", "ADMIN", "systems", "/Manage/Index", 1, 1, 1, "ADMIN");
                addButton(toinclude, "QA UTILITY", "INSPECTIONUTILITY", "search", "http://apr.standardtextile.com/APP/Mob/SPCInspectionInput.aspx", 1, 2, 1, "QA");
                addButton(toinclude, "FLAGBOARD", "FLAGBOARD", "maintenance", "http://maintenance.standardtextile.com", 1, 1, 1, "CTX");
                addButton(toinclude, "DATA-AUTOMATIONS", "DATAAUTOMATIONS", "excel-old", "http://m.standardtextile.com/dataautomations/launch.aspx?ReportType=A", 2, 1, 1, "QA");
                addButton(toinclude, "QA RESULTS", "INSPECTIONRESULTS", "search", "http://apr.standardtextile.com/APP/Presentation/InspectionVisualizer.aspx", 2, 2, 1, "QA");
                model.Apps = toinclude as IList<Models.ButtonModel>; 
            }
            catch (Exception ex)
            {
                //RedirectToAction("error", new { message = ex.Message, returnUrl = "/Home/Index" });
            }
        }

        #region helpers
        private void mapUserLocation()
        {
            string uc = Request.QueryString["UC"];

            if (string.IsNullOrEmpty(uc) == false)
            {
                Models.EF.LocationMaster location = Helpers.UserActivity.getUserLocation(uc);
                if (location != null)
                {
                    try
                    {
                        Response.Cookies["APRKeepMeIn"]["IPAddress"] = Helpers.UserActivity.getIPAdress();
                        Response.Cookies["APRKeepMeIn"]["CID_Print"] = location.CID;
                        Response.Cookies["APRKeepMeIn"]["CorporateName"] = location.Name.Trim();
                        Response.Cookies["APRKeepMeIn"]["lastVisit"] = DateTime.Now.ToShortDateString() + DateTime.Now.ToShortTimeString();
                        Response.Cookies["APRKeepMeIn"].Expires = DateTime.Now.AddYears(2);

                    }
                    catch (Exception e)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                    }
                }


            }
        }

        private static void addDropdown(List<Models.ButtonModel> buttonlist, string ParentId, string text_, string name_, string cssClass_, string AppUrl_)
        {
            if (name_.Trim().Length == 0)
                throw new Exception("must supply dropdown with name.");

            if (text_.Trim().Length == 0)
                throw new Exception("must supply dropdown with text.");

            if (ParentId.Trim().Length == 0)
                throw new Exception("must supply dropdown with ParentId."); 

            if (buttonlist != null || buttonlist.Count > 0)
            {
                foreach(var button in buttonlist)
                {
                    if (button.Id.Trim() == ParentId)
                    {
                        List<Models.OptionModel> options = new List<Models.OptionModel>();
                        if (button.Dropdowns != null && button.Dropdowns.Length > 0) options.AddRange(button.Dropdowns);
                        options.Add(new Models.OptionModel() { text = text_, name = name_, AppUrl = AppUrl_, cssClass = cssClass_, Id = name_ });
                        button.Dropdowns = options.ToArray();
                        return; 
                    }
                }
            }
        }

        private static void addButton(List<Models.ButtonModel> buttonlist, string text, string name, string cssClass, string AppUrl, int GridRow = 1, int GridCol = 1, int ColSpan = 1, string section = "")
        {
            Models.ButtonModel button = new Models.ButtonModel();
            if (string.IsNullOrEmpty(text) == false)
            {
                button.text = text;
            }
            else
            {
                throw new Exception("button text cannot be null or empty");
            }

            if (GridRow > 0)
            {
                button.GridRow = GridRow;
            } else
            {
                button.GridRow = 1; 
            }

            if (GridCol > 0)
            {
                button.GridCol = GridCol; 
            } else
            {
                button.GridCol = 1; 
            }

            if (ColSpan > 0)
            {
                button.ColSpan = ColSpan; 
            } else
            {
                button.ColSpan = 1; 
            }

            if (string.IsNullOrEmpty(name) == false)
            {
                button.Name = name;
                button.Id = name; 
            }
            else
            {
                throw new Exception("button name cannot be null or empty");
            }

            if (string.IsNullOrEmpty(cssClass) == false)
            {
                button.cssClass = cssClass;
            }
            else
            {
                throw new Exception("button picture url cannot be null or empty");
            }

            if (string.IsNullOrEmpty(AppUrl) == false)
            {
                button.AppUrl = AppUrl;
            }
            else
            {
                throw new Exception("button app url cannot be null or empty");
            }
            if (section.Trim().Length > 0)
            {
                button.section = section.Trim(); 
            } else
            {
                button.section = "QA"; 
            }

            buttonlist.Add(button);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        #endregion


    }
}