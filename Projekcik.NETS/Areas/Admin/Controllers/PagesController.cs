
using Projekcik.NETS.Models.Data;
using Projekcik.NETS.Views.Pages;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Projekcik.NETS.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
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
        //GET Admin/Pages/EditPages
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            //deklaracja viewModel
            PageVM model;

            using (Db db = new Db())
            {
                //pobieramy strone o przekazanym id
                PageDTO dto = db.Pages.Find(id);
                //sprawdzamy czy taka strona istenieje
                if(dto == null)
                {
                    return Content("Strona nie istnieje");
                }
                //mamy w pagevm konstruktor ktory dostaje dto i przypisuje caly model
                model = new PageVM(dto);
            }
                return View(model);
        }
        //GET Admin/Pages/EditPages
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (Db db = new Db())
            {
                //pobranie id 
                int id = model.Id;
                string slug = "home";

                //pobranie strony do edycji
                PageDTO dto = db.Pages.Find(id);

                dto.Title = model.Title;
                if(model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ","-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }  
                }
                //sprawdzamy unikalność strony adresu
                if(db.Pages.Where(x=> x.Id != id).Any(x => x.Title == model.Title) || db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "Strona już istnieje");
                }
                dto.Body = model.Body;
                dto.Slug = slug;
                dto.HasSidebar = model.HasSidebar;

                // zapis edytowanej strony do bazy danych

                db.SaveChanges();             
            }
            // ustawienie tempdata, komunikatu
            TempData["SM"] = "Wedytowałeś stronę";

            //przekierowanie do strony  REDIRECT
            return RedirectToAction("EditPage");
        }
        [HttpGet]
        //GET Admin/Pages/Details/id
        public ActionResult Details(int id)
        {
            //deklaracja PAGEVM
            PageVM model;
            using(Db db = new Db())
            {
                //pobranie strony o ID
                PageDTO dto = db.Pages.Find(id);

                //sprawdzenie czy strona istnieje
                if(dto == null)
                {
                    return Content("Strona o podanym ID nie istnieje");
                }
                //inicjalizacja strony
                model = new PageVM(dto);

            }

            return View(model);
        }
        [HttpGet]
        //GET Admin/Pages/Delete/id
        public ActionResult Delete(int id)
        {
            using (Db db = new Db())
            {
                // pobieramy strone
                PageDTO dto = db.Pages.Find(id);

                //usuwanie strony z bazy
                db.Pages.Remove(dto);
                db.SaveChanges();
            }
            //przekierowanie do index
                return RedirectToAction("Index");
        }
        //Post Admin/Pages/ReorderPages
        [HttpPost]
        public ActionResult ReorderPages(int[] id)
        {
            using (Db db = new Db())
            {
                int count = 1;
                PageDTO dto;

                // sortowanie stron, zapis na bazie 
                foreach ( var pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;
                    db.SaveChanges();
                    count++;
                }
            }
            return View();
        }
        //Get Admin/Pages/Editsidebar
        [HttpGet]
        public ActionResult EditSidebar()
        {
            // deklaracja vm
            SidebarVM model;
            using (Db db = new Db())
            {
                //pobieramy SidebarDTO
                SidebarDTO dto = db.Sidebar.Find(1);
                //inicjalizacja modelu
                model = new SidebarVM(dto);

            }
            return View(model);
        }
        //Post Admin/Pages/
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using (Db db = new Db())
            {
                SidebarDTO dto = db.Sidebar.Find(1);

                //modyfikacja sidebar
                dto.Body = model.Body;
                db.SaveChanges();

            }
            //Ustawiamy komunikat o modyfikacji
            TempData["SM"] = "zmodyfikowałeś pasek boczny";
            return RedirectToAction("EditSidebar");
        }

    }
}