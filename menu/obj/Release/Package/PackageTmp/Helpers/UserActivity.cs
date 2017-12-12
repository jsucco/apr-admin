using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace menu.Helpers
{
    public class UserActivity
    {
        
        public static async Task log(string username, string deviceType, string activityType)
        {
            if (string.IsNullOrEmpty(username))
                username = "ANONYMOUS";
            try
            {
                using (Models.EF.AprManager _db = new Models.EF.AprManager())
                {
                    Models.EF.UserActivityLog entry = new Models.EF.UserActivityLog();
                    entry.DBOrigin = "APR";
                    entry.UserID = username;
                    entry.EntryTimestamp = DateTime.Now;
                    entry.DeviceType = deviceType;
                    entry.IPAddress = getIPAdress(); 
                    entry.CID = getCID();
                    entry.ActivityType = activityType;
                    _db.UserActivityLogs.Add(entry);
                    _db.SaveChanges(); 
                }
            } catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
            }
        }
        public static string getCID()
        {
            string cid = "000000";
            HttpCookie location = HttpContext.Current.Request.Cookies["APRKeepMeIn"];

            if (location !=null && location["CID_Print"] != null)
            {
                cid = location["CID_Print"];
            }
            return cid;
        }
        public static string getCorporateName()
        {
            string name = "GLOBAL STC";
            HttpCookie location = HttpContext.Current.Request.Cookies["APRKeepMeIn"];

            if (location != null && location["CorporateName"] != null)
            {
                name = location["CorporateName"];
            }

            return name;
        }
        public static string getIPAdress()
        {
            HttpContext context = HttpContext.Current;
            string sIPAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(sIPAddress) == true)
            {
                sIPAddress = context.Request.ServerVariables["REMOTE_ADDR"];
            } else
            {
                string[] ipParsed = sIPAddress.Split(new char[] { ',' });
                if (ipParsed != null && ipParsed.Length > 0)
                {
                    sIPAddress = ipParsed[0];
                }
            }

            if (string.IsNullOrEmpty(sIPAddress))
                sIPAddress = "IPERROR";

            return sIPAddress; 
        }
        public static Models.EF.LocationMaster getUserLocation(string uc)
        {
            Models.EF.LocationMaster location = null;

            using (Models.EF.AprManager _db = new Models.EF.AprManager())
            {
                try
                {
                    if (string.IsNullOrEmpty(uc) == false)
                    {
                        location = (from x in _db.LocationMasters where x.CID == uc select x).First();
                    }
                } catch (Exception e)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(e); 
                }
                
            }

            return location; 
        }
    }
}