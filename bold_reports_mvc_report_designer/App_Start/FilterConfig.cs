using System.Web;
using System.Web.Mvc;

namespace bold_reports_mvc_report_designer
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
