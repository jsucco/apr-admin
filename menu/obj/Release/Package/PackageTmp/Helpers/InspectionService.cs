using menu.Models.EF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace menu.Helpers
{
    public class InspectionService
    {
        private InspectionContext context;

        private AprManager MangContext; 

        public string ErrorMessage;

        public int PrimaryKey; 

        public InspectionService(InspectionContext _context = null)
        {
            context = (_context != null) ? _context : getNewContext(); 
        }

        private InspectionContext getNewContext()
        {
            return new InspectionContext(); 
        }

        private AprManager getNewMangContext()
        {
            return new AprManager(); 
        }

        private Models.EF.LocationMaster[] locations_data = new Models.EF.LocationMaster[] { };

        private WorkRoom[] getDBWorkrooms()
        {
            try
            {
                var rows =  (from x in context.WorkRooms select x).ToArray();
                if (rows.Length > 0) 
                    HttpContext.Current.Cache.Insert("Inspection_Workrooms", rows, null, DateTime.Now.AddMonths(12), System.Web.Caching.Cache.NoSlidingExpiration);
                return rows; 
            } catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); 
            }
            return new WorkRoom[] { };
        }

        public Models.EF.WorkRoom[] getWorkrooms()
        {
            return getDBWorkrooms();
        }

        public Models.WorkroomsUserObject[] getLocWorkrooms(string CID)
        {
            Models.WorkroomsUserObject[] data = new Models.WorkroomsUserObject[] { };

            var cached = getCacheLocWorkrooms(CID); 

            if (cached != null)
            {
                return cached; 
            }

            return GetActiveWorkrooms(CID);      
        }

        public Models.WorkroomsUserObject[] getCacheLocWorkrooms(string CID)
        {
            if (HttpContext.Current.Cache["Inspection_Workrooms_loc" + CID] != null)
            {
                try
                {
                    return (Models.WorkroomsUserObject[])HttpContext.Current.Cache["Inspection_Workrooms_loc" + CID];
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                }
            }
            return null;
        }

        public void ClearCachedWorkroomUserObject(string CID)
        {
            HttpContext.Current.Cache.Remove("Inspection_Workrooms_loc" + CID); 
        }

        public void CacheWorkroomUserObject(string CID)
        {
            if (CID.Trim().Length != 6)
                return;

            Models.WorkroomsUserObject[] dbrecs = GetActiveWorkrooms(CID);

            if (dbrecs.Length > 0)
            {
                cacheDBLocWorkrooms(dbrecs, CID); 
            }
        }

        private void cacheDBLocWorkrooms(Models.WorkroomsUserObject[] recs, string CID)
        {
            try
            {
                if (recs.Length > 0)
                    HttpContext.Current.Cache.Insert("Inspection_Workrooms_loc" + CID.Trim(), recs, null, DateTime.Now.AddMonths(2), System.Web.Caching.Cache.NoSlidingExpiration);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }

        public menu.Models.WorkroomsUserObject[] GetActiveWorkrooms(string CID)
        {
            try
            {
                MangContext = getNewMangContext();

                var locid = (from x in MangContext.LocationMasters where x.CID == CID.Trim() select x.id).FirstOrDefault();
                if (locid > 0)
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    var warr1 = (from x in context.WorkroomActivators
                                 join wr in context.WorkRooms on x.WorkroomId equals wr.Id
                                 where x.LocationMasterId == locid && x.Status == true
                                 select new menu.Models.WorkroomsUserObject { Id = wr.Id, Name = wr.Name, Abbreviation = wr.Abbreviation, Status = x.Status }).ToArray();
                    return warr1;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return new menu.Models.WorkroomsUserObject[] { };
        }

        public Models.EF.LocationMaster[] getLocations()
        {
            try
            {
                AprManager context = new AprManager();
                context.Configuration.ProxyCreationEnabled = false; 
                locations_data = (from x in context.LocationMasters where x.InspectionResults == true select x).ToArray();
                cacheInspectionLocs(); 
            } catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                ErrorMessage = ex.Message; 
            }
            return locations_data; 
        }

        private void cacheInspectionLocs()
        {
            if (locations_data.Length == 0)
                return;

            HttpContext.Current.Cache.Insert("Inspection_Locations", locations_data, null, DateTime.Now.AddDays(7), System.Web.Caching.Cache.NoSlidingExpiration); 
        }

        public bool AddWorkroom(string Name_, string Abbreviation_, string Type_)
        {
            bool result = true; 
            try
            {
                if (Name_.Trim().Length == 0)
                    throw new Exception("Workroom name length must be greater than 0.");

                if (Abbreviation_.Trim().Length == 0)
                    throw new Exception("Workroom abbreviation length must be greater than 0.");

                if (Type_.Trim().Length == 0)
                    throw new Exception("Workroom Type length must be greater than 0.");

                WorkRoom newrow = new WorkRoom() { Id = 0, Name = Name_, Abbreviation = Abbreviation_, Type = Type_ };
                context.WorkRooms.Add(newrow);
                context.Database.Connection.Close(); 
                if (context.SaveChanges() < 1)
                    throw new Exception("failed to save changes.");
                else
                    PrimaryKey = newrow.Id; 

            } catch (Exception ex)
            {
                ErrorMessage = ex.Message; 
                result = false;
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return result;
        }

        public bool EditWorkroom(int PrimaryKey, string Name_, string Abbreviation_)
        {
            bool result = true; 
            try
            {
                if (Name_.Trim().Length == 0)
                    throw new Exception("Workroom name length must be greater than 0.");

                if (Abbreviation_.Trim().Length == 0)
                    throw new Exception("Workroom abbreviation must be greater than 0.");

                var newrow = (from x in context.WorkRooms where x.Id == PrimaryKey select x).FirstOrDefault();

                if (newrow != null)
                {
                    newrow.Name = Name_;
                    newrow.Abbreviation = Abbreviation_;
                    if (context.SaveChanges() < 1)
                        throw new Exception("failed to save changes.");
                } else
                    throw new Exception("failed to retrieve existing data row.");

            } catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                result = false;
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); 
            }
            return result; 
        }

        public string[] Get400Workrooms()
        {
            var connstr = ConfigurationManager.ConnectionStrings["InspectionContext"]
                .ConnectionString;
            List<string> results = new List<string>(); 

            using (SqlConnection conn = new SqlConnection(connstr))
            {
                conn.Open(); 
                using (SqlCommand cmd = new SqlCommand("SP_AS400_GetUnqWorkrooms", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandTimeout = 60000;

                    SqlDataReader reader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    if (reader != null)
                    {
                        IDataRecord record = null; 
                        while (reader.Read())
                        {
                            record = (IDataRecord)reader; 
                            if (record.FieldCount > 0)
                            {
                                try
                                {
                                    var c = Convert.ToString(record[0]);
                                    results.Add(c);
                                } catch (Exception ex)
                                {

                                }
                                
                            }
                        }
                    }
                    reader.Close(); 
                }
            }
            return results.ToArray();
        }

        public void Cache400Workrooms(string[] content)
        {
            if (content != null && content.Length > 0)
            {
                HttpContext.Current.Cache.Insert("Inspection_as400Workrooms", content, null, DateTime.Now.AddMonths(1), System.Web.Caching.Cache.NoSlidingExpiration);
            }
        }

        public string[] GetCached400Workrooms()
        {
            if (HttpContext.Current.Cache["Inspection_as400Workrooms"] != null)
            {
                try
                {
                    return (string[])HttpContext.Current.Cache["Inspection_as400Workrooms"];
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                }
            }
            return null;
        }

        public bool DeleteWorkroom(int PrimaryKey)
        {
            bool result = true; 
            try
            {
                if (PrimaryKey == 0)
                    throw new Exception("Primary key cannot be zero.");

                context.WorkRooms.Remove((from x in context.WorkRooms where x.Id == PrimaryKey select x).FirstOrDefault());
                if (context.SaveChanges() < 1)
                    throw new Exception("failed to save changes."); 

            } catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                result = false;
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); 
            }
            return result; 
        }

        private void LoadInsLocationsContex()
        {
            if (HttpContext.Current.Cache["Inspection_Locations"] != null)
            {
                locations_data = (Models.EF.LocationMaster[])HttpContext.Current.Cache["Inspection_Locations"];
                return;
            }

            locations_data = getLocations();
            return; 
        }

        public bool AddWorkroomActiv(int PrimaryKey)
        {
            try
            {
                LoadInsLocationsContex(); 
                if (locations_data.Length > 0)
                {
                    List<Models.EF.WorkroomActivator> walst = new List<Models.EF.WorkroomActivator>(); 
                    foreach (var item in locations_data)
                    {
                        Models.EF.WorkroomActivator wa = new Models.EF.WorkroomActivator();
                        wa.LocationMasterId = item.id;
                        wa.Status = false;
                        wa.WorkroomId = PrimaryKey;
                        wa.Inserted_Timestamp = DateTime.Now;
                        context.WorkroomActivators.Add(wa); 
                        //walst.Add(wa); 
                    }
                    //context.WorkroomActivators.AddRange(walst as IEnumerable<Models.EF.WorkroomActivator>);
                    if (context.SaveChanges() != locations_data.Length)
                        return false;
                }
            } catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
            return true; 
        }

        public bool DelWorkroomActive(int PrimaryKey)
        {
            try
            {
                LoadInsLocationsContex(); 
                if (locations_data.Length > 0)
                {
                    var ext_wa = (from x in context.WorkroomActivators where x.WorkroomId == PrimaryKey select x);

                    foreach (var item in ext_wa)
                    {
                        context.WorkroomActivators.Remove((from x in context.WorkroomActivators where x.WorkroomId == PrimaryKey select x).FirstOrDefault());
                    }
                    if (context.SaveChanges() != locations_data.Length)
                        return false; 
                }
            } catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false; 
            }
            return true; 
        }

        public bool SetActivator(int LocationId, int WorkroomId, bool StatusVal)
        {
            try
            {
                var ar = (from x in context.WorkroomActivators where x.LocationMasterId == LocationId && x.WorkroomId == WorkroomId select x).FirstOrDefault();

                if (ar == null )
                {
                    ErrorMessage = "no record found.";
                    return false; 
                }

                ar.Status = StatusVal;

                if (context.SaveChanges() < 1)
                    return false; 
            } catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false; 
            }
            return true; 
        }

        public Models.WorkroomsUserObject[] GetWorkroomActivators(int locid)
        {
            try
            {
                context.Configuration.ProxyCreationEnabled = false; 
                var warr1 = (from x in context.WorkroomActivators
                        join wr in context.WorkRooms on x.WorkroomId equals wr.Id
                        where x.LocationMasterId == locid  && x.Status == true
                        select new Models.WorkroomsUserObject {Id = wr.Id, Name = wr.Name, Abbreviation = wr.Abbreviation, Status = x.Status }).ToArray();
                return warr1; 
            } catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return new Models.WorkroomsUserObject[] { }; 
            }  
        }

        public void Dispose()
        {
            context.Dispose(); 
        }
    }
}