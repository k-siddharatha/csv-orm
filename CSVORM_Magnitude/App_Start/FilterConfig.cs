using System.Web;
using System.Web.Mvc;

namespace CSVORM_Magnitude
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
