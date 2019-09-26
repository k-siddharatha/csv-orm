using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Microsoft.VisualBasic.FileIO;

namespace CSVORM_Magnitude.Controllers
{
    public enum Operator { e, g, l };
    public class DynamicEntity : DynamicObject
    {
        private readonly IDictionary<string, object> _values;

        public DynamicEntity(IDictionary<string, object> values)
        {
            _values = values;
        }
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _values.Keys;
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_values.ContainsKey(binder.Name))
            {
                result = _values[binder.Name];
                return true;
            }
            result = null;
            return false;
        }
    }



    public interface IRowMatch {
        bool isRowMatch(TextFieldParser csvParse, string[] conditions);
    }


    public class ValuesController : ApiController
    {

        // select name from test.csv where salary > 30
        // GET api/values
        public IEnumerable<DynamicEntity> Get(string csvTable)
        {
            var values = new Dictionary<string, object>();
            List<DynamicEntity> dynPosts = new List<DynamicEntity>();
            var path = HttpContext.Current.Server.MapPath(@"~\App_Data\" + csvTable + ".csv");
            var conditionClauses = "salary < 4000000";
            string[] conditionClause = conditionClauses.Split();

            using (TextFieldParser csvParser = new TextFieldParser(path))
            {
                string[] fields = csvParserDefaultSet(csvParser);
                int[] selectIndex = { Array.IndexOf(fields, "name"), Array.IndexOf(fields, "salary") };
              
                int conditionIndex = Array.IndexOf(fields, conditionClause[0]);
                string whereClause = conditionClause[2];
                
                while (!csvParser.EndOfData)
                {
                    string[] returnRow = csvParser.ReadFields();
                    switch (conditionClause[1]) {
                        case "=":
                            {
                                if (whereClause.Equals(returnRow[conditionIndex]))
                                {
                                    foreach (int i in selectIndex)
                                    {
                                        values.Add(fields[i], returnRow[i]);
                                    }
                                    var post = new DynamicEntity(values);
                                    dynPosts.Add(post);
                                }
                            }
                            break;
                        case "<":
                            {
                                if (int.TryParse(returnRow[conditionIndex], out int K)
                                    && int.TryParse(whereClause, out int J)
                                    && K<J)
                                {
                                    foreach (int i in selectIndex)
                                    {
                                        values.Add(fields[i], returnRow[i]);
                                    }
                                    var post = new DynamicEntity(values);
                                    dynPosts.Add(post);
                                }
                            }
                            break;
                        case ">":
                            {
                                if (int.TryParse(returnRow[conditionIndex], out int K)
                                    && int.TryParse(whereClause, out int J)
                                    && K > J)
                                {
                                    foreach (int i in selectIndex)
                                    {
                                        values.Add(fields[i], returnRow[i]);
                                    }
                                    var post = new DynamicEntity(values);
                                    dynPosts.Add(post);
                                }
                            }
                            break;
                    }
                    
                }
            }

            return dynPosts;

        }

    

        public string[] csvParserDefaultSet(TextFieldParser csvParser) {
            csvParser.CommentTokens = new string[] { "#" };
            csvParser.SetDelimiters(new string[] { "," });
            csvParser.HasFieldsEnclosedInQuotes = true;
            return csvParser.ReadFields();
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "id: "+id;
                
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
