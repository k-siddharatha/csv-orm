using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSVORM_Magnitude.Models
{
    public interface IRowMatch
    {
        void rowFinder(string[] row, string[] fields, List<int> selectIndex, SimpleCondition condition, ref List<DynamicEntity> dynRows);

    }
}