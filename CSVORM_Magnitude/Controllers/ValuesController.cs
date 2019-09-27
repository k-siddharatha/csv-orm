using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Http;
using Microsoft.CSharp.RuntimeBinder;
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
        
        // select name from test.csv where salary > 30
        // GET api/values
        public IEnumerable<DynamicEntity> Get(string csvTable)
        {
            List<DynamicEntity> dynPosts = new List<DynamicEntity>();
            var path = HttpContext.Current.Server.MapPath(@"~\App_Data\" + csvTable + ".csv");
            var conditionClauses = "(salary < 4000000) OR (employee = TRUE)";
            string[] conditionClause;
            string[] complexCondition = conditionClauses.Split(new string[] { "AND", "OR" }, StringSplitOptions.RemoveEmptyEntries);

            ComplexCondition complex = new ComplexCondition();
            complex.AndOrOr = conditionClauses.Contains("AND") ? AndOrOr.AND : AndOrOr.OR;

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

        public void rowFinder(string[] row,string[] fields, SimpleConditions condition, ref List<DynamicEntity> dynPosts)
        {
            string[] returnRow = row;
            int[] selectIndex = { Array.IndexOf(fields, "emp_id") ,Array.IndexOf(fields, "name"), Array.IndexOf(fields, "salary") };
            int conditionIndex = Array.IndexOf(fields, condition.colName);
            string whereClause = condition.value;
            switch (condition.opera)
            {
                case "=":
                    {
                        if (whereClause.Equals(returnRow[conditionIndex]))
                        {
                            addToList(selectIndex, fields, returnRow, ref dynPosts);
                        }
                    }
                    break;
                case "!=":
                    {
                        if (!whereClause.Equals(returnRow[conditionIndex]))
                        {
                            addToList(selectIndex, fields, returnRow, ref dynPosts);
                        }
                    }
                    break;
                case "<":
                    {
                        if (int.TryParse(returnRow[conditionIndex], out int K)
                            && int.TryParse(whereClause, out int J)
                            && K < J)
                        {
                            addToList(selectIndex, fields, returnRow, ref dynPosts);
                        }
                    }
                    break;
                case ">":
                    {
                        if (int.TryParse(returnRow[conditionIndex], out int K)
                            && int.TryParse(whereClause, out int J)
                            && K > J)
                        {
                            addToList(selectIndex, fields, returnRow, ref dynPosts);
                            
                        }
                    }
                    break;
                case ">=":
                    {
                        if (int.TryParse(returnRow[conditionIndex], out int K)
                            && int.TryParse(whereClause, out int J)
                            && K >= J)
                        {
                            addToList(selectIndex, fields, returnRow, ref dynPosts);
                        }
                    }
                    break;
                case "<=":
                    {
                        if (int.TryParse(returnRow[conditionIndex], out int K)
                            && int.TryParse(whereClause, out int J)
                            && K <= J)
                        {
                          addToList(selectIndex, fields, returnRow, ref dynPosts);
                        }
                    }
                    break;
            }
        }
        private static object GetProperty(dynamic target, string name)
        {
            var site =
                CallSite<Func<CallSite, dynamic, object>>
                  .Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, name, target.GetType(),
                        new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
            return site.Target(site, target);
        }

        public int LookUpAlt(List<DynamicEntity> lst, string propName, object value)
        {
            if (lst.Count == 0) {
                return 0;
            }
            var values = lst.FindAll(i => GetProperty(i, propName).Equals(value));
            return (values).Count;
        }
        public object LookUp(List<dynamic> lst, string propName, object value)
        {
            return lst.FindAll(i => GetProperty(i, propName).Equals(value));
        }
        public void addToList(int[] selectIndex, string[] fields, string[] returnRow,ref List<DynamicEntity> dynPosts)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();

            if (LookUpAlt(dynPosts,"emp_id",returnRow[0]) == 0)
            {
                foreach (int i in selectIndex)
                {
                    values.Add(fields[i], returnRow[i]);
                }
                dynPosts.Add(new DynamicEntity(values));
            }
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
