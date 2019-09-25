using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CSVORM_Magnitude.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            string[] read;
            char[] seperators = { ',' };

            StreamReader sr = new StreamReader(@"C:\Users\kumarsid\source\repos\CSVORM_Magnitude\CSVORM_Magnitude\App_Data\test.csv");

            string data = sr.ReadLine();

            while ((data = sr.ReadLine()) != null)
            {
                read = data.Split(seperators, StringSplitOptions.None);
                float longitude = float.Parse(read[1]);
                float latitude = float.Parse(read[2]);
            }
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value"+id;
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
