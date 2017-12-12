using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using menu.Models.CtxEF;

namespace menu.Helpers
{
    public class CtxService
    {
        public bool error = false;
        public string errorMessage = ""; 

        public CtxService(CtxContext _context = null, string CID_ = "")
        {
            CID = CID_.Trim();

            if (CID.Length < 6)
                throw new Exception("CID incorrect length");

            context = (_context != null) ? _context : getContext(CID);  
        }

        private CtxContext context;

        private CtxManagerService manager = new CtxManagerService(); 

        private string CID;

        private CtxContext getContext(string CID)
        {
            return new CtxContext(manager.BuildConnectionString(CID));
        }

        public Maintenance_Flagboard[] GetFlagboards()
        {
            try
            {
                using (var ctx = context)
                {
                    ctx.Configuration.LazyLoadingEnabled = false; 

                    return (from x in ctx.Maintenance_Flagboard
                            orderby x.MFB_Id
                            select x).ToArray(); 
                }
            } catch (Exception ex)
            {
                error = true;
                errorMessage = ex.Message; 
            }

            return new Maintenance_Flagboard[] { }; 
        }

        public bool EditFB(int key, string Desc)
        {
            bool result = true; 
            try
            {
                if (Desc.Trim().Length == 0)
                    throw new Exception("Location description length must be greater than 0.");

                var row = (from x in context.Maintenance_Flagboard where x.MFB_Id == key select x).FirstOrDefault(); 

                if (row != null)
                {
                    row.Loc_Description = Desc;

                    if (context.SaveChanges() < 1)
                        throw new Exception("failed to save changes"); 
                } else
                {
                    throw new Exception("failed to retrieve existing data row"); 
                }
            } catch (Exception ex)
            {
                error = true;
                errorMessage = ex.Message;
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); 
            }
            return result; 
        }

        public bool AddFB(string Desc)
        {
            bool result = true; 
            try
            {
                var LastFB = (from x in context.Maintenance_Flagboard
                              orderby x.MFB_Id descending
                              select x.MFB_Id).Take(1).FirstOrDefault();

                var nUserID = "fb" + (LastFB + 1).ToString();

                var idCnt = (from x in context.Securities
                             where x.UserID == nUserID
                             select x.UserID).Count(); 

                if (idCnt == 0)
                {
                    Models.CtxEF.Security ns = new Models.CtxEF.Security();

                    ns.PM_Data_Entry = true;
                    ns.PM_Forms = true;
                    ns.PM_Master = true;
                    ns.PM_Reports = true;
                    ns.PM_View = true;
                    ns.UserID = nUserID;
                    ns.Password = genPass(LastFB + 1);

                    context.Securities.Add(ns);

                    if (context.SaveChanges() < 1)
                        throw new Exception("failed to add new fb user");
                }              

                Models.CtxEF.Maintenance_Flagboard nfb = new Models.CtxEF.Maintenance_Flagboard();

                nfb.MFB_Id = LastFB + 1; 
                nfb.UserID = nUserID;
                nfb.Loc_Description = Desc;

                context.Maintenance_Flagboard.Add(nfb);

                if (context.SaveChanges() < 1)
                    throw new Exception("failed to add new fb"); 
                
            } catch (Exception ex)
            {
                error = true;
                errorMessage = ex.Message;
                result = false; 
            }
            finally
            {
                context.Database.Connection.Close(); 
            }

            return result; 
        }

        private string genPass(int MFBId)
        {
            var ab = new string[26] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            int fi = MFBId - 1;
            string pass = "FB";

            while (true)
            {
                if (fi > 25)
                {
                    fi = fi - 25;
                    pass = pass + "A";
                    continue;
                } else
                {
                    pass = pass + ab[fi];
                    break;
                }
            }
            return pass; 
        }
    }
}