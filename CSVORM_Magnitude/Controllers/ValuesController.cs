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
using CSVORM_Magnitude.Models;
using CSVORM_Magnitude.Controllers.Helper;

namespace CSVORM_Magnitude.Controllers
{
    public class ValuesController : ApiController
    {
        
        // select name from test.csv where salary > 30
        // GET api/values
        public IEnumerable<DynamicEntity> Get(string csvTable)
        {
            List<DynamicEntity> dynPosts = new List<DynamicEntity>();
            var path = HttpContext.Current.Server.MapPath(@"~\App_Data\" + csvTable + ".csv");
            var conditionClauses = "(salary < 4000000) OR (emp_id = 1) OR (name = sid_1)";
            string[] select = { "emp_id", "name", "salary" };

            string[] conditionClause;
            string[] complexCondition = conditionClauses.Split(new string[] { "AND", "OR" }, StringSplitOptions.RemoveEmptyEntries);

            ComplexCondition complex = new ComplexCondition();
            complex.AndOrOr = new List<AndOrOr>();

            complex.AndOrOr.Add(conditionClauses.Contains("AND") ? AndOrOr.AND : AndOrOr.OR);
            complex.condtions = new List<SimpleCondition>();

            ValuesHelper helper = new ValuesHelper();
            foreach (string condition in complexCondition) {
                conditionClause = condition.Replace('(', ' ').Replace(')', ' ').Trim().Split();
                SimpleCondition simpleConditions = new SimpleCondition()
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
                    string[] fields = helper.csvParserDefaultSet(csvParser);
                    
                    while (!csvParser.EndOfData)
                    {
                        string[] returnRow = csvParser.ReadFields();

                        List<int> selectList = new List<int>();
                        foreach (var s in select) {
                            selectList.Add(Array.IndexOf(fields, s));
                        }
                        
                        helper.rowFinder(returnRow, fields, selectList, condition, ref dynPosts);                       
                    }
                }
            }
            
            return dynPosts;

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
