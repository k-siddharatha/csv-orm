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
    public class QueryController : ApiController
    {

        // select id, name, salary from test.csv where (salary < 4000000) OR (id = 1) OR (name = sid_1)
        // GET api/values
        public IEnumerable<DynamicEntity> Get(string csvTable)
        {
            // @input values
            var conditionClauses = "(salary < 4000000) OR (id = 1) OR (name = sid_1)";
            string[] select = { "id", "name", "salary" };

            // find the file in a folder location inside App_Data
            var path = HttpContext.Current.Server.MapPath(@"~\App_Data\" + csvTable + ".csv");
            // helping methods are written in helper class
            QueryHelper helper = new QueryHelper();

            // Creating dynamic rows of Dynamic Class Entity DynamicObject
            List<DynamicEntity> dynRows = new List<DynamicEntity>();


            //string[] orIndex = conditionClauses.IndexOf("OR");
            //string[] andIndex = conditionClauses.IndexOf("AND");
            string[] conditionClause;
            string[] complexCondition = conditionClauses.Split(new string[] { "AND", "OR" }, StringSplitOptions.RemoveEmptyEntries);


            //complex condition keeps list of simple conditions
            ComplexCondition complex = new ComplexCondition();

            //may have more than one and or ?? rethink
            complex.AndOrOr = new List<AndOrOr>();

            complex.AndOrOr.Add(conditionClauses.Contains("AND") ? AndOrOr.AND : AndOrOr.OR);
            complex.condtions = new List<SimpleCondition>();


            foreach (string condition in complexCondition)
            {
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
                        foreach (var s in select)
                        {
                            selectList.Add(Array.IndexOf(fields, s));
                        }

                        helper.rowFinder(returnRow, fields, selectList, condition, ref dynRows);
                    }
                }
            }

            return dynRows;

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
