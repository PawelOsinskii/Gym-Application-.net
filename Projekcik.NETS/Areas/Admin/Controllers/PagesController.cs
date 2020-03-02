
using Projekcik.NETS.Models.Data;
using Projekcik.NETS.Views.Pages;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Projekcik.NETS.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            // deklaracja listy pagewievmodel
            List<PageVM> pagesList;

            
            using (Db db = new Db())
            {// inicjalizacja danymi z bazy danych listy
                pagesList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();


            }
                //zzwracamy strony do widoku
            return View(pagesList);
        }
        //Get: Admin/pages/addpages
        public ActionResult AddPage()
        {
            return View();
        }
    }
}