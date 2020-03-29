using Projekcik.NETS.Models.Data;
using Projekcik.NETS.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Projekcik.NETS.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages");
        }


        public ActionResult CategoryMenuPartial()
        {
            // deklarujemy CategoryVM list
            List<CategoryVM> categoryVMList;

            // inicjalizacja listy
            using (Db db = new Db())
            {
                categoryVMList = db.Categories
                                   .ToArray()
                                   .OrderBy(x => x.Sorting)
                                   .Select(x => new CategoryVM(x))
                                   .ToList();
            }

            // zwracamy partial z lista
            return PartialView(categoryVMList);
        }
    }
}