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
    public enum AndOrOr { AND,OR};
    public class SimpleConditions : IRowMatch
    {
        public string colName;
        public string value;
        public string opera;
        public void isRowMatch(string[] returnRow, string[] conditions)
        {
            throw new NotImplementedException();
        }
    }

    public class ComplexCondition : IRowMatch
    {
        public AndOrOr AndOrOr;
        public List<SimpleConditions> condtions;
        public void isRowMatch(string[] returnRow, string[] conditions)
        {
            throw new NotImplementedException();
        }
    }

    public interface IRowMatch
    {
        void isRowMatch(string[] returnRow, string[] conditions);
    }


    public class ValuesController : ApiController
    {
        HashSet<DynamicEntity> dynPosts = new HashSet<DynamicEntity>();

        // select name from test.csv where salary > 30
        // GET api/values
        public IEnumerable<DynamicEntity> Get(string csvTable)
        {
            var path = HttpContext.Current.Server.MapPath(@"~\App_Data\" + csvTable + ".csv");
            var conditionClauses = "(salary < 4000000) OR (employee = TRUE)";
            string[] conditionClause;
            string[] complexCondition = conditionClauses.Split(new string[] { "AND", "OR" }, StringSplitOptions.RemoveEmptyEntries);

            ComplexCondition complex = new ComplexCondition();
            if (conditionClauses.Contains("AND"))
            {
                complex.AndOrOr = AndOrOr.AND;
            } else {
                complex.AndOrOr = AndOrOr.OR;
            }
            complex.condtions = new List<SimpleConditions>();

            foreach (string condition in complexCondition) {
                conditionClause = condition.Replace('(', ' ').Replace(')', ' ').Trim().Split();
                SimpleConditions simpleConditions = new SimpleConditions()
                {
                    colName = conditionClause[0],
                    opera = conditionClause[1],
                    value = conditionClause[2]
                };
                complex.condtions.Add(simpleConditions);
            }


            //simple condition
            foreach (var condition in complex.condtions)
            {
                using (TextFieldParser csvParser = new TextFieldParser(path))
                {
                    string[] fields = csvParserDefaultSet(csvParser);
                    
                    while (!csvParser.EndOfData)
                    {
                        string[] returnRow = csvParser.ReadFields();
                        rowFinder(returnRow, fields, condition, ref dynPosts);                       
                    }
                }
            }
            return dynPosts;

        }

        public void rowFinder(string[] row,string[] fields, SimpleConditions condition, ref HashSet<DynamicEntity> dynPosts)
        {
            string[] returnRow = row;
            int[] selectIndex = { Array.IndexOf(fields, "name"), Array.IndexOf(fields, "salary") };

            int conditionIndex = Array.IndexOf(fields, condition.colName);
            string whereClause = condition.value;
            switch (condition.opera)
            {
                case "=":
                    {
                        if (whereClause.Equals(returnRow[conditionIndex]))
                        {
                            var entry = addToList(selectIndex, fields, returnRow);
                            if (entry != null)
                            {
                                dynPosts.Add(entry);
                            }

                        }
                    }
                    break;
                case "!=":
                    {
                        if (!whereClause.Equals(returnRow[conditionIndex]))
                        {
                            var entry = addToList(selectIndex, fields, returnRow);
                            if (entry != null)
                            {
                                dynPosts.Add(entry);
                            }

                        }
                    }
                    break;
                case "<":
                    {
                        if (int.TryParse(returnRow[conditionIndex], out int K)
                            && int.TryParse(whereClause, out int J)
                            && K < J)
                        {
                            var entry = addToList(selectIndex, fields, returnRow);
                            if (entry != null)
                            {
                                dynPosts.Add(entry);
                            }
                        }
                    }
                    break;
                case ">":
                    {
                        if (int.TryParse(returnRow[conditionIndex], out int K)
                            && int.TryParse(whereClause, out int J)
                            && K > J)
                        {
                            var entry = addToList(selectIndex, fields, returnRow);
                            if (entry != null)
                            {
                                dynPosts.Add(entry);
                            }
                        }
                    }
                    break;
                case ">=":
                    {
                        if (int.TryParse(returnRow[conditionIndex], out int K)
                            && int.TryParse(whereClause, out int J)
                            && K >= J)
                        {
                            var entry = addToList(selectIndex, fields, returnRow);
                            if (entry != null)
                            {
                                dynPosts.Add(entry);
                            }
                        }
                    }
                    break;
                case "<=":
                    {
                        if (int.TryParse(returnRow[conditionIndex], out int K)
                            && int.TryParse(whereClause, out int J)
                            && K <= J)
                        {
                            var entry = addToList(selectIndex, fields, returnRow);
                            if (entry != null)
                            {
                                dynPosts.Add(entry);
                            }
                        }
                    }
                    break;
            }
        }

        public DynamicEntity addToList(int[] selectIndex, string[] fields, string[] returnRow)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();

            foreach (int i in selectIndex)
            {
                values.Add(fields[i], returnRow[i]);
            }
            return new DynamicEntity(values);

        }

        public string[] csvParserDefaultSet(TextFieldParser csvParser)
        {
            csvParser.CommentTokens = new string[] { "#" };
            csvParser.SetDelimiters(new string[] { "," });
            csvParser.HasFieldsEnclosedInQuotes = true;
            return csvParser.ReadFields();
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "id: " + id;

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
