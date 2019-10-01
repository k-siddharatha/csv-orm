using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Http;
using Microsoft.VisualBasic.FileIO;
using CSVORM_Magnitude.Models;
using CSVORM_Magnitude.Controllers.Helper;

namespace CSVORM_Magnitude.Controllers
{
    
    public class QueryController : ApiController
    {

        // preframed condtions 
        private static string conditionInput = "(salary < 4000000) OR (id = 1) OR (name = sid-1)";
        private static string[] selectInput = { "id", "name", "salary" };

        private static string conditionInput1 = "(age < 25) AND (contract = FALSE) AND (name = sid-1)";
        private static string[] selectInput1 = { "id","name","age", "salary" };

        // GET api/values
        public IEnumerable<DynamicEntity> Get(string csvTable, string conditionType = null, string selectType = null)
        {

            // condtions
            var conditionClauses = (conditionType == null) || (conditionType.Equals("0")) ? conditionInput : conditionType.Equals("1") ? conditionInput1 : conditionType;
            string[] select = ((selectType == null) || (selectType.Equals("0")) )? selectInput : selectType.Equals("1") ? selectInput1 : selectType.Split();
            
            // Creating dynamic rows of Dynamic Class Entity DynamicObject
            List<DynamicEntity> dynRows = new List<DynamicEntity>();
            List<DynamicEntity> dynRowsReturn = new List<DynamicEntity>();
            // find the file in a folder location inside App_Data
            var path = HttpContext.Current.Server.MapPath(@"~\App_Data\" + csvTable + ".csv");
            if (!File.Exists(path))
            {
                return dynRowsReturn;
            }
            
            // helping methods are written in helper class
            QueryHelper helper = new QueryHelper();
            string[] conditionClause;
            string[] complexCondition = conditionClauses.Split(new string[] { "AND", "OR" }, StringSplitOptions.RemoveEmptyEntries);
            //complex condition keeps list of simple conditions
            ComplexCondition complex = new ComplexCondition();
            //may have more than one and or at one level
            complex.AndOrOr = new List<AndOrOr>();
            int count = 0;
            foreach (string s in conditionClauses.Split())
            {
                if (s.Equals("AND"))
                {
                    complex.AndOrOr.Add(AndOrOr.AND);
                }
                else if (s.Equals("OR")){
                    complex.AndOrOr.Add(AndOrOr.OR);
                }
            }
            complex.condtions = new List<SimpleCondition>();
            
            // filling conditions to complex conditions
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

            //simple condition check one by one
            foreach (var condition in complex.condtions)
            {
                List<DynamicEntity> dynRowsPerCondition = new List<DynamicEntity>();

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
                        var entry = helper.rowFinder(returnRow, fields, selectList, condition);

                        if (entry != null)
                        {
                            dynRowsPerCondition.Add(entry);
                        }

                    }


                }

                // only one condition
                if (complex.condtions.Count == 1)
                {
                    return dynRowsPerCondition;
                }
                // one level of complex queries all or / all and

                if (dynRows.Count == 0)
                {
                    dynRows.AddRange(dynRowsPerCondition);
                }
                else if (complex.AndOrOr[count] == AndOrOr.OR)
                {
                    //LEFT JOIN
                    foreach (var a in dynRowsPerCondition)
                    {
                        if (helper.LookUpAlt(dynRows, "id", helper.LookUp(a, "id")) == 0)
                        {
                            dynRows.Add(a);
                        }
                    }
                    dynRowsReturn = dynRows;
                    count++;
                }
                else if (complex.AndOrOr[count] == AndOrOr.AND)
                {
                    // INNER EQUE JOIN

                    foreach (var a in dynRowsPerCondition)
                    {
                        if (helper.LookUpAlt(dynRows, "id", helper.LookUp(a, "id")) == 1
                            && helper.LookUpAlt(dynRowsReturn, "id", helper.LookUp(a, "id")) == 0)
                        {
                            dynRowsReturn.Add(a);
                        }
                    }
                    dynRows = dynRowsReturn;
                    count++;
                }
                
            }
            return dynRowsReturn;
        }
    }
}
