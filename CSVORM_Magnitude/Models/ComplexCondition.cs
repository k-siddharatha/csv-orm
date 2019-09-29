using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSVORM_Magnitude.Models
{
    public enum AndOrOr { AND, OR };
    public class ComplexCondition : IRowMatch
    {
        public List<AndOrOr> AndOrOr;
        public List<SimpleCondition> condtions;
        public void rowFinder(string[] row, string[] fields, List<int> selectIndex, SimpleCondition condition, ref List<DynamicEntity> dynRows)
        {
            throw new NotImplementedException();
        }
    }
}