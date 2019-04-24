using CascBasic.Classes;
using System.Web;
using System.Web.Mvc;

namespace CascBasic
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthorizeAttribute());
            filters.Add(new SessionIsRoleAttribute());
            //filters.Add(new RoleMenuAccess());
        }
    }
}
