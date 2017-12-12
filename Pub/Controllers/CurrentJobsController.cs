using System.Collections.Generic;
using System.Web.Http.Cors; 
using System.Web.Http;

namespace menu.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [AllowAnonymous]
    public class CurrentJobsController : ApiController
    {
        // GET: api/CurrentJobs
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/CurrentJobs/5
        [HttpGet]
        [Route("api/CurrentJobs/{paramone}/{paramtwo}")]
        public string Get(int paramone, int paramtwo)
        {
            return (paramone + paramtwo).ToString();
        }

        // POST: api/CurrentJobs
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/CurrentJobs/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/CurrentJobs/5
        public void Delete(int id)
        {
        }
    }
}
