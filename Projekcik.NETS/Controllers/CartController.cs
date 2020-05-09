using Projekcik.NETS.Models.Data;
using Projekcik.NETS.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace Projekcik.NETS.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {

            //inicjalizacja koszyka
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();  //rzut na liste elementow a jesli nie ma to tworzy nową

            // sprawdzamy czy koszyk jest pusty
            if (cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "Twój koszyk jest pusty";
                return View();
            }

            //obliczenie wartosci podsumuwującej koszyka i przekazanie do ViewBag
            decimal total = 0m;
            foreach (var item in cart)
            {
                total += item.Total;
            }
            ViewBag.GrandTotal = total;


            return View(cart);
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
                model.Quantity = quantity;
                model.Price = price;
            }
            else
            {
                quantity = 0;
                price = 0m;

            }

            return PartialView(model);
        }

        public ActionResult AddToCartPartial(int id)
        {
            // Inicjalizacja CartVM List
            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            // Inicjalizacja cartVM
            CartVM model = new CartVM();

            using (Db db = new Db())
            {
                // pobieramy produkt
                ProductDTO product = db.Products.Find(id);

                // sprawdzamy czy ten produkt jest juz w koszyku 
                var productInCart = cart.FirstOrDefault(x => x.ProductId == id);

                // w zaleznosci od tego czy produkt jest w koszyku go dodajemy lub zwiekszamy ilosc
                if (productInCart == null)
                {
                    cart.Add(new CartVM()
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = 1,
                        Price = product.Price,
                        Image = product.ImageName
                    });
                }
                else
                {
                    productInCart.Quantity++;
                }
            }

            //pobieramy calkowite wartosc ilosci i ceny i dodajemy do modelu
            int qty = 0;
            decimal price = 0m;

            foreach (var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }

            model.Quantity = qty;
            model.Price = price;

            // zapis w sesii
            Session["cart"] = cart;

            return PartialView(model);
        }

       public JsonResult IncrementProduct(int productId)
        {
            // Inicjalizacja listy CartVM
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            // pobieramy cartVM
            CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

            // zwikszamy ilosc produktu
            model.Quantity++;

            // przygotowanie danych do JSONA
            var result = new { qty = model.Quantity, price = model.Price };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public JsonResult DecrementProduct(int productId)
        {
            // Inicjalizacja listy CartVM
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            // pobieramy cartVM
            CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

            // zmniejszamy ilosc produktu
            if(model.Quantity > 1 )
                 model.Quantity--;
            else
            {
                model.Quantity = 0;
                cart.Remove(model);
            }

            // przygotowanie danych do JSONA
            var result = new { qty = model.Quantity, price = model.Price };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public void RemoveProduct(int productId)
        {
            // Inicjalizacja listy CartVM
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            // pobieramy cartVM
            CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

            // zmniejszamy ilosc produktu
            
           
                model.Quantity = 0;
                cart.Remove(model);
        

          

           
        }

        public ActionResult PaypalPartial()
        {
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            return PartialView(cart);
        }
        [HttpPost]
        public void PlaceOrder()
        {
            //pobieramy zawartośc koszyka ze zmiennej sesji
            List<CartVM> cart = Session["cart"] as List<CartVM>;
            //pobieramy nazwe użytkownika 

            string UserName = User.Identity.Name;
            // deklarujemy numer zamówienia
            int orderId = 0;
            
            using(Db db = new Db())
            {
                // zainicjalizowac orderDTO
                OrderDTO orderDto = new OrderDTO();

                //pobieramy userId
                var user = db.User.FirstOrDefault(x => x.UserName == UserName);
                int userId = user.Id;
                //ustawienie dto  i zapis 
                orderDto.UserId = userId;
                orderDto.CreatedAt = System.DateTime.Now;
                db.Orders.Add(orderDto);
                db.SaveChanges();
                //pobieramy id zapisanego zamówienia 
                orderId = orderDto.Id;


                //inicjalizacja orderdetailsdto


                OrderDetailsDTO detailsDto = new OrderDetailsDTO();
                foreach(var item in cart)
                {
                    
                    detailsDto.OrderId = orderId;
                    detailsDto.UserId = userId;
                    detailsDto.ProductId = item.ProductId;
                    detailsDto.Quantity = item.Quantity;

                    db.OrderDetails.Add(detailsDto);

                    db.SaveChanges();
                }
                
            }

            // wysyłać email do administratora o złożonym zamówieniu
            var client = new SmtpClient("smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("4437f0ed5b200f", "530c65931f5a99"),
                EnableSsl = true
            };
            client.Send("admin@example.com", "admin@example.com", "Nowe zamówienie", "Masz nowe zamówienie! o id: "+ orderId+ " o dacie " + DateTime.Now);


            // reset Session
            Session["cart"] = null;
        }
        
    }
}