using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.VisualBasic.FileIO;

namespace CSVORM_Magnitude.Controllers
{
    public class Select{
        public Select(List<string> selectValues) {

        }
    }

    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get(string csvTable)
        {
            string retValue1 = "";
            Console.WriteLine(csvTable+ "the csv name");

            var path = @"C:\Users\kumarsid\source\repos\CSVORM_Magnitude\CSVORM_Magnitude\App_Data\test.csv"; 
            using (TextFieldParser csvParser = new TextFieldParser(path))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;

                // Skip the row with the column names
                string[] fields = csvParser.ReadFields();
                int selectIndex = Array.IndexOf(fields, "name");
                string whereClause = "sid";
               
                while (!csvParser.EndOfData)
                {
                    // Read current line fields, pointer moves to the next line.
                    string[] values = csvParser.ReadFields();
                    if (whereClause.Equals(values[selectIndex])) {
                        retValue1 = values[selectIndex];
                    }
                }
            }
            return new string[] { retValue1, "value2" };
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
