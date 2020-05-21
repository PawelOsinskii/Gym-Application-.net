using Projekcik.NETS.Areas.Admin.Views.Shop;
using Projekcik.NETS.Models;
using Projekcik.NETS.Models.Data;
using Projekcik.NETS.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
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

        public ActionResult Category(string name)
        {
            // deklaracja productVMList
            List<ProductVM> productVMList;

            using (Db db = new Db())
            {
                // pobranie id kategorii
                CategoryDTO categoryDTO = db.Categories.Where(x => x.Slug == name).FirstOrDefault();
                int catId = categoryDTO.Id;

                // inicjalizacja listy produktów
                productVMList = db.Products
                                  .ToArray()
                                  .Where(x => x.CategoryId == catId)
                                  .Select(x => new ProductVM(x)).ToList();

                // pobieramy nazwe kategori
               
                if ((db.Products.Where(x => x.CategoryId == catId).FirstOrDefault()==null))
                {
                    ViewBag.CategoryName = categoryDTO.Name;
                }
                else
                {
                    var productCat = db.Products.Where(x => x.CategoryId == catId).FirstOrDefault();


                    ViewBag.CategoryName = categoryDTO.Name;
                }
            }

            // zwracamy widok z lista produktów z danej kategorii
            return View(productVMList);
        }


        //GET: /shop/product-szczegoly/name
        [ActionName("product-details")]
        public ActionResult ProductDetails(string name)
        {
            //deklaracja productVM i product DTO

            ProductVM model;
            ProductDTO dto;
            //inicjalizacja product.Id
            int id = 0;
            using(Db db = new Db())
            {
                //sprawdzamy czy produkt istnieje
                if(!db.Products.Any(x=> x.slug.Equals(name)))
                    {
                    return RedirectToAction("Index", "Shop");
                }
                //inicjalizacja prodcut dto
                dto = db.Products.Where(x => x.slug == name).FirstOrDefault();

                id = dto.Id;
                //model
                model = new ProductVM(dto);

            }
            //pobieramy galerie zdjęć dla wybranego produktu
            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs")).Select(fn => Path.GetFileName(fn));

            return View("ProductDetails", model);
        }

        public ActionResult Calculators()
        {
            return View();
        }
        public ActionResult BuyCarnet()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


    }
}