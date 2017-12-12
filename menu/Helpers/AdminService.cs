using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using menu.Models.EF;
using System.Data.SqlClient;

namespace menu.Helpers
{
    public class AdminService
    {
        private AprManager MangContext;

        public bool ErrorFlag = false;
        public string ErrorMessage = ""; 

        public AdminService(AprManager _MangContext = null)
        {
            MangContext = (_MangContext == null) ? new AprManager() : _MangContext; 
        }

        public EmailMaster[] GetAdminEmails()
        {
            EmailMaster[] admins = new EmailMaster[] { }; 
            try
            {
                admins = (from e in MangContext.EmailMasters where e.ADMIN == true select e).ToArray(); 
            } catch (Exception ex)
            {
                ErrorFlag = true;
                ErrorMessage = ex.Message; 
            }
            return admins; 
        }

        public Models.AlertManager[] GetAlertEmails(string code)
        {
            try
            {
                using (var context = new AprManager())
                {
                    var codeparam = new SqlParameter("@code", code);

                    var result = context.Database
                        .SqlQuery<Models.AlertManager>("GetAlertEmails @code", codeparam)
                        .ToArray();

                    if (result != null)
                        return result;
                }
            } catch (Exception ex)
            {
                ErrorFlag = true;
                ErrorMessage = ex.Message; 
            }         

            return new Models.AlertManager[] { }; 
        }

        public bool AddEmail(Models.AlertManager aO)
        {
            try
            {
                using (var context = MangContext)
                {
                    var crs = (from x in context.ActiveAlerts
                               where x.Name == "ACTIVE" && x.Value == aO.Email
                               select x).Count();

                    if (crs > 0)
                        throw new Exception("Email already exists");

                    if (aO.Email.Split('@').Length != 2)
                        throw new Exception("Incorrect format"); 

                    context.ActiveAlerts.Add(new ActiveAlert() { Code = "INS", CID = aO.CID, Name = "ACTIVE", Value = aO.Email });

                    if (context.SaveChanges() > 0)
                        return true; 
                }
            } catch (Exception ex)
            {
                ErrorFlag = false;
                ErrorMessage = ex.Message; 
            }

            return false; 
        }

        public bool EditActiveAlert(Models.AlertManager aO)
        {
            try
            {
                using (var context = MangContext)
                {
                    var arecs = (from x in context.ActiveAlerts 
                                 where x.Code == aO.Code && x.Value == aO.Email
                                 select x);

                    IEnumerable<ActiveAlert> icEnts = arecs.Where(o => o.Name == "INSCOMP").Select(o => o);

                    int icCnt, acCnt, icxCnt = 0;

                    icCnt = icEnts.ToList().Count(); 

                    if (aO.INSCOMP == false && icCnt > 0)
                        context.ActiveAlerts.RemoveRange(icEnts);
                    else if (aO.INSCOMP == true && icCnt == 0)
                        context.ActiveAlerts.Add(new ActiveAlert() { Code = aO.Code, CID = aO.CID, Name = "INSCOMP", Value = aO.Email });

                    IEnumerable<ActiveAlert> acEnts = arecs.Where(o => o.Name == "AUTOCOMP").Select(o => o);

                    acCnt = acEnts.ToList().Count(); 

                    if (aO.AUTOCOMP == false && acCnt > 0)
                        context.ActiveAlerts.RemoveRange(acEnts);
                    else if (aO.AUTOCOMP == true && acCnt == 0)
                        context.ActiveAlerts.Add(new ActiveAlert() { Code = aO.Code, CID = aO.CID, Name = "AUTOCOMP", Value = aO.Email });

                    IEnumerable<ActiveAlert> icxEnts = arecs.Where(o => o.Name == "INSCOMPEX").Select(o => o);

                    icxCnt = icxEnts.ToList().Count();

                    if (aO.INSCOMPEX == false && icxCnt > 0)
                        context.ActiveAlerts.RemoveRange(icxEnts);
                    else if (aO.INSCOMPEX == true && icxCnt == 0)
                        context.ActiveAlerts.Add(new ActiveAlert() { Code = aO.Code, CID = aO.CID, Name = "INSCOMPEX", Value = aO.Email });

                    var rCID = (from x in arecs select x.CID).FirstOrDefault();
                    
                    if (rCID != aO.CID)
                    {
                        foreach (var item in arecs)
                            item.CID = aO.CID; 
                    }

                    if (context.SaveChanges() > 0)
                        return true;

                }
            } catch (Exception ex)
            {
                ErrorFlag = false;
                ErrorMessage = ex.Message; 
            }

            return false; 
        }

        public bool DeleteAAEmail(Models.AlertManager aO)
        {
            try
            {
                using (var context = MangContext)
                {
                    var arecs = (from x in context.ActiveAlerts
                                 where x.Code == aO.Code && x.Value == aO.Email
                                 select x);

                    if (arecs.Count() == 0)
                        throw new Exception("record does not exist");

                    context.ActiveAlerts.RemoveRange(arecs);

                    if (context.SaveChanges() > 0)
                        return true; 
                }
            } catch (Exception ex)
            {
                ErrorFlag = true;
                ErrorMessage = ex.Message;
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); 
            }
            return false; 
        }

        public string ParseRawUserName(string user)
        {
            if (user.Length == 0)
                return "";

            char[] delemits = new char[] { '\\' };
            var usersplit = user.Split(delemits).ToArray();

            if (usersplit.Length == 1)
                return user;
            else if (usersplit.Length > 1)
                return usersplit[1];

            return "";
        }

        public string GetSerLocs()
        {
            var s = "000000:ALL;";

            using (var context = new AprManager())
            {
                var lrecs = (from x in context.LocationMasters
                             select x).ToArray(); 

                for (int i = 0; i < lrecs.Length; i++)
                {
                    if (i == lrecs.Length -1)
                    {
                        s = s + lrecs[i].CID.Trim() + ":" + lrecs[i].Name;
                    } else
                    {
                        s = s + lrecs[i].CID.Trim() + ":" + lrecs[i].Name + ";"; 
                    }
                }
            }

            return s; 
        }
    }
}