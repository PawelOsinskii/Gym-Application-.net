using System.Web.Mvc;

namespace Projekcik.NETS.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class DashboardController : Controller
    {
        // GET: Admin/Dashboard
        public ActionResult Index()
        {
            return View();
        }
    }
}