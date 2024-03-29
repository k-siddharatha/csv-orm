﻿using CSVORM_Magnitude.Models;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace CSVORM_Magnitude.Controllers.Helper
{
    public class QueryHelper
    {
        
        public DynamicEntity rowFinder(string[] row, string[] fields, List<int> selectIndex, SimpleCondition condition)
        {
            string[] returnRow = row;
            DynamicEntity dynReturn = null;
            int conditionIndex = Array.IndexOf(fields, condition.colName);
            string whereClause = condition.value;
            switch (condition.opera)
            {
                case "=":
                    {
                        if (whereClause.Equals(returnRow[conditionIndex]))
                        {
                            dynReturn = conditionMatchRows(selectIndex, fields, returnRow);
                        }
                    }
                    break;
                case "!=":
                    {
                        if (!whereClause.Equals(returnRow[conditionIndex]))
                        {
                            dynReturn = conditionMatchRows(selectIndex, fields, returnRow);
                        }
                    }
                    break;
                case "<":
                    {
                        if (int.TryParse(returnRow[conditionIndex], out int K)
                            && int.TryParse(whereClause, out int J)
                            && K < J)
                        {
                            dynReturn = conditionMatchRows(selectIndex, fields, returnRow);
                        }
                    }
                    break;
                case ">":
                    {
                        if (int.TryParse(returnRow[conditionIndex], out int K)
                            && int.TryParse(whereClause, out int J)
                            && K > J)
                        {
                            dynReturn = conditionMatchRows(selectIndex, fields, returnRow);

                        }
                    }
                    break;
                case ">=":
                    {
                        if (int.TryParse(returnRow[conditionIndex], out int K)
                            && int.TryParse(whereClause, out int J)
                            && K >= J)
                        {
                            dynReturn = conditionMatchRows(selectIndex, fields, returnRow);
                        }
                    }
                    break;
                case "<=":
                    {
                        if (int.TryParse(returnRow[conditionIndex], out int K)
                            && int.TryParse(whereClause, out int J)
                            && K <= J)
                        {
                            dynReturn = conditionMatchRows(selectIndex, fields, returnRow);
                        }
                    }
                    break;
            }
            return dynReturn;
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
            if (lst.Count == 0)
            {
                return 0;
            }

            var values = lst.FindAll(i => GetProperty(i, propName).Equals(value));
            return (values).Count;
        }
        public object LookUp(List<DynamicEntity> lst, string propName, object value)
        {
            return lst.FindAll(i => GetProperty(i, propName).Equals(value));
        }

        public object LookUp(DynamicEntity de, string propName)
        {
            
            return GetProperty(de, propName);
        }
        public void addToList(List<int> selectIndex, string[] fields, string[] returnRow, ref List<DynamicEntity> dynRows)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            if (LookUpAlt(dynRows, "id", returnRow[0]) == 0)
            {
                foreach (int i in selectIndex)
                {
                    values.Add(fields[i], returnRow[i]);
                }
                dynRows.Add(new DynamicEntity(values));
            }
        }

        public DynamicEntity conditionMatchRows(List<int> selectIndex, string[] fields, string[] returnRow) {

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
    }
}