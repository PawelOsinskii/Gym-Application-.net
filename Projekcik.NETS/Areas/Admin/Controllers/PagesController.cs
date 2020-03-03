
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
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            //sprawdzanie walidacji formularza
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (Db db = new Db())
            {
                string slug;
                //inicjalizacja pagedto
                PageDTO dto = new PageDTO();
                
                
                //gdy slug jest pusty, to przypisujemy tytuł
                if(string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }
                //zapobiegamy dodania takiej samej nazwy strony
                if ((db.Pages.Any(x => x.Title == model.Title)) || db.Pages.Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "Ten tytuł już istnieje");
                    return View(model);
                }
                dto.Title = model.Title;
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 1000;


                //zapis dto
                db.Pages.Add(dto);
                db.SaveChanges();
            }

            TempData["SM"] = "Dodałeś nową stronę";
            return RedirectToAction("AddPage");
        }

    }
}