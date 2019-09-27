using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSVORM_Magnitude.Models
{
    public interface IRowMatch
    {
        void isRowMatch(string[] returnRow, string[] conditions);
    }
}