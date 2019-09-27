using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSVORM_Magnitude.Models
{
    public class SimpleCondition : IRowMatch
    {
        public string colName;
        public string value;
        public string opera;
        public void isRowMatch(string[] returnRow, string[] conditions)
        {
            throw new NotImplementedException();
        }
    }
}