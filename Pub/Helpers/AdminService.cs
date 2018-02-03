using System;
using System.Collections.Generic;
using System.Linq;
using menu.Models;
using menu.Models.EF;
using System.Data.SqlClient;
using System.Web;

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
                admins = (from e in MangContext.EmailMasters
                          where e.ADMIN == true
                          select e).ToArray(); 
            } catch (Exception ex)
            {
                ErrorFlag = true;
                ErrorMessage = ex.Message; 
            }
            return admins; 
        }

        public AdminsGridView[] GetAdminGrid()
        {
            AdminsGridView[] grid_data = new AdminsGridView[] { }; 

            try
            {
                grid_data = (from e in MangContext.EmailMasters
                             select new AdminsGridView { id = e.id, Address = e.Address.Trim(), Status = e.ADMIN }).ToArray(); 
            } catch (Exception ex)
            {
                ErrorFlag = true;
                ErrorMessage = ex.Message; 
            }
            return grid_data; 
        }

        private string AdminCacheKey = "Admin-Email-Hash"; 

        public HashSet<string> GetAdminHash()
        {
            try
            {
                var cached = HttpRuntime.Cache[AdminCacheKey];

                if (cached != null)
                    return (HashSet<string>)cached; 


            } catch(Exception ex)
            {
                ErrorFlag = true;
                ErrorMessage = ex.Message;
            }

            var dbEmails = GetAdminEmails();
            HashSet<string> hs = new HashSet<string>(); 

            if (dbEmails != null && dbEmails.Count() > 0)
            {
                foreach (var item in dbEmails)
                {
                    var rawAdminUser = item.Address.Split(new char[] { '@' });

                    if (rawAdminUser.Length > 1)
                    {
                        hs.Add(rawAdminUser[0].ToUpper());
                    }
                }

                if (hs.Count > 0)
                    HttpRuntime.Cache.Insert(AdminCacheKey, 
                        hs, null, DateTime.Now.AddDays(14), 
                        System.Web.Caching.Cache.NoSlidingExpiration);
            }

            return hs;
        }

        public void ClearAdminEmailCache()
        {
            HttpRuntime.Cache.Remove(AdminCacheKey); 
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

        public bool AddAdmin(AdminsGridView amO)
        {
            try
            {
                using (var context = MangContext)
                {
                    var exCnt = (from x in context.EmailMasters
                                 where x.Address == amO.Address.Trim()
                                 select x).Count();

                    if (exCnt > 0)
                        throw new Exception("address already exists");

                    context.EmailMasters.Add(new EmailMaster { Address = amO.Address, ADMIN = amO.Status });

                    if (context.SaveChanges() > 0)
                        return true; 
                }
            } catch (Exception ex)
            {
                ErrorFlag = true;
                ErrorMessage = ex.Message;
            }

            return false; 
        }

        public bool EditAdmin(AdminsGridView amO)
        {
            try
            {
                using (var context = MangContext)
                {
                    var sql = @"Update EmailMaster SET Address = @address, ADMIN = @status
                                WHERE id = @id"; 

                    var rows = context.Database.ExecuteSqlCommand
                        (sql, 
                        new SqlParameter("@address", amO.Address),
                        new SqlParameter("@status", amO.Status), 
                        new SqlParameter("@id", amO.id));

                    if (rows > 0)
                        return true; 
                }
            } catch (Exception ex)
            {
                ErrorFlag = true;
                ErrorMessage = ex.Message; 
            }

            return false; 
        }

        public bool DeleteAdmin(int id)
        {
            try
            {
                var sql = @"Delete from EmailMaster
                            where id = @id";

                using (var context = MangContext)
                {
                    var rows = context.Database.ExecuteSqlCommand
                        (sql,
                        new SqlParameter("@id", id));

                    if (rows > 0)
                        return true; 
                } 

            } catch (Exception ex)
            {
                ErrorFlag = true;
                ErrorMessage = ex.Message; 
            }

            return false; 
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

                    if (aO.INSCOMP == true)
                        context.ActiveAlerts.Add(new ActiveAlert() { Code = "INS", CID = aO.CID, Name = "INSCOMP", Value = aO.Email });

                    if (aO.AUTOCOMP == true)
                        context.ActiveAlerts.Add(new ActiveAlert() { Code = "INS", CID = aO.CID, Name = "AUTOCOMP", Value = aO.Email });

                    if (aO.INSCOMPEX == true)
                        context.ActiveAlerts.Add(new ActiveAlert() { Code = "INS", CID = aO.CID, Name = "INSCOMPEX", Value = aO.Email });

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

                    if (arecs == null)
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