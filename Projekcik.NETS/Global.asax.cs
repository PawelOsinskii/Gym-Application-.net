using Projekcik.NETS.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Projekcik.NETS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_AuthenticateRequest()
        {
            //pobieramy nazwe użytkownika
            if (User == null)
                return;
            string username = Context.User.Identity.Name;

            string[] roles = null;
            using(Db db = new Db())
            {
                //pobieramy dane użytkkownika z bazy aby pobrać role
                UserDTO dto = db.User.FirstOrDefault(x => x.UserName == username);
                roles = db.UserRoles.Where(x => x.UserId == dto.Id).Select(x => x.Role.Name).ToArray();

            }
            // tworzymy IPrincipalObiekt
            IIdentity userIdentity = new GenericIdentity(username);
            IPrincipal newUserObj = new GenericPrincipal(userIdentity, roles);

            //uaktualnić context
            Context.User = newUserObj;
        }
    }
}
