using Projekcik.NETS.Models.ViewModels.Cart;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Projekcik.NETS.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult CartPartial()
        {
            //inicjalizacja CartVM
            CartVM model = new CartVM();

            //inicjalizacja ilosci i ceny
            int quantity = 0;
            decimal price = 0;

            //sprawdzamy czy mamy koszyk zapisane w sesji
            if (Session["cart"] != null)
            {
                //pobieranie wartości z sesji
                var list = (List<CartVM>)Session["cart"];
                foreach (var item in list)
                {
                    quantity += item.Quantity;
                    price += item.Quantity * item.Price;
                } 
            }
            else
            {
                quantity = 0;
                price = 0m;

            }
            
            return PartialView(model);
        }

    }
}