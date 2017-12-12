using System.Web.Http;
using menu.Helpers;
using System.Web.Http.Cors;
using System.Web.Script.Serialization;
using System;

namespace menu.Controllers
{
    [AllowAnonymous]
    public class WorkRoomsController : ApiController
    {
        private InspectionService service = new InspectionService();

        private JavaScriptSerializer jser = new JavaScriptSerializer();

        private Models.MethodResponse response = new Models.MethodResponse(); 

        // GET: api/WorkRooms
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Helpers.CustomAuthenication]
        [HttpGet]
        public Models.MethodResponse Get(string id)
        {
            try
            {         
                if (id.Length == 6)
                {
                    response.Content = service.getLocWorkrooms(id);
                    response.Result = true;

                    return response;
                } else
                {
                    response.ErrorMessage = "CID variable has incorrect length.";
                    response.Content = new Models.WorkroomsUserObject[] { };
                    response.Result = false;
                }
            } catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                response.Content = new Models.WorkroomsUserObject[] { };
                response.Result = false;
            }

            return response;
        }
        
    }
}