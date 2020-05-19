using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Projekcik.NETS.Models.Data;
using Projekcik.NETS.Models.ViewModels.Account;
using Projekcik.NETS.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace Projekcik.NETS.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return Redirect("~/account/login");
        }

        // GET: Account/Login

        public ActionResult Login()
        {

            //sprawdzanie czy użytkownik jest zalogowany
            string userName = User.Identity.Name;
            if (!string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("user-profile");
            }

            //zwracamy widok logowania
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public ActionResult Login(LoginUserVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            bool isValid = false;



            using (Db db = new Db())
            {
                try
                {
                    UserDTO dto = db.User.First(x => x.UserName == model.UserName);
                    isValid = UserDTO.verifyPassword(model, dto);
                }
                catch (System.InvalidOperationException)
                {
                    isValid = false;
                }


                //sprawdzamy login 



            }
            if (!isValid)
            {
                ModelState.AddModelError("", "Nieprawidłowa nazwa użytkownika lub zapomniałeś hasła");
                return View(model);
            }
            else
            {
                FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                return Redirect(FormsAuthentication.GetRedirectUrl(model.UserName, model.RememberMe));
            }

        }

        // GET: Account/create-account
        [HttpGet]
        [ActionName("create-account")]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }
        // Post: Account/create-account
        [HttpPost]
        [ActionName("create-account")]
        public ActionResult CreateAccount(UserVM model)
        {
            //sprawdzenie Model.State
            if (!ModelState.IsValid)
            {
                return View("CreateAccount", model);
            }
            //sprawdzenie hasła czy pasują
            if (!model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError("", "Hasła do siebie nie pasują");
                return View("CreateAccount", model);
            }
            //sprawdzenie czy nazwa użytkownika jest unikalna
            using (Db db = new Db())
            {
                if (db.User.Any(x => x.UserName.Equals(model.UserName)))
                {
                    ModelState.AddModelError("", "Nazwa użytkownika musi być unikalna");
                    return View("CreateAccount", model);
                }
                //utworzenie użytkownika

                UserDTO userDTO = new UserDTO()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAdress = model.EmailAdress,
                    UserName = model.UserName,

                };
                UserDTO.hashPass(model, userDTO);
                db.User.Add(userDTO);
                db.SaveChanges();

                //dodanie roli dla użytkownika

                UserRoleDTO userRoleDTO = new UserRoleDTO()
                {
                    UserId = userDTO.Id,
                    RoleId = 1
                };
                //dodanie roli
                db.UserRoles.Add(userRoleDTO);
                db.SaveChanges();
                //tempdata


            }
            TempData["SM"] = "Utworzyłeś konto, jeżeli chcesz korzystać w stuprocentach ze swojego kontaa i brac udział w naszych treningach, " +
                "to polecamy kupić karnet";
            return Redirect("~/account/login");
        }

        // GET: /account/logout
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            return Redirect("~/account/login");
        }

        [Authorize]
        public ActionResult UserNavpartial()
        {
            //pobieramy username 
            string username = User.Identity.Name;
            // deklarujemy model 
            UserNavPartialVM model;
            using (Db db = new Db())
            {
                //pobieramy użytkownika
                UserDTO dto = db.User.FirstOrDefault(x => x.UserName == username);
                model = new UserNavPartialVM()
                {
                    UserName = dto.UserName,
                    HasKarnet = dto.Karnet,
                    MamKarnet = "Posiadasz Karnet",
                    NieMamKarnetu = "Nie posiadasz karnetu"


                };
            }
            return PartialView(model);
        }


        //GET: /account/user-profile
        [Authorize]
        [ActionName("user-profile")]
        public ActionResult UserProfile()
        {
            //pobieramy nazwe użytkownika
            string username = User.Identity.Name;

            //deklaruejmy VM

            UserProfileVM model;

            using (Db db = new Db())
            {
                //pobieramy użytkownika
                UserDTO dto = db.User.FirstOrDefault(x => x.UserName == username);
                model = new UserProfileVM(dto);

            }

            return View("UserProfile", model);
        }
        [HttpPost]
        [ActionName("user-profile")]
        public ActionResult UserProfile(UserProfileVM model)
        {
            // sprawdzamyy state
            if (ModelState.IsValid)
            {
                return View("UserProfile", model);
            }
            using (Db db = new Db())
            {


            }
            return View();
        }

        //GET: /account/orders
        [Authorize]
        public ActionResult Orders()
        {
            //inicjalziacja listy zamówień dla użytkownika
            List<OrderForUserVM> ordersForUser = new List<OrderForUserVM>();
            using (Db db = new Db())
            {
                //pobieramy userId 
                UserDTO user = db.User.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                int userId = user.Id;

                //pobieramy zamowienia dla uzytkownika 
                List<OrderVM> orders = db.Orders.Where(x => x.UserId == userId).ToArray().Select(x => new OrderVM(x)).ToList();
                foreach (var order in orders)
                {
                    //inicjalizacja slownika produktów
                    Dictionary<string, int> productsAndQty = new Dictionary<string, int>();
                    decimal total = 0;

                    // pobieramy szczegoly zamowienia 

                    List<OrderDetailsDTO> orderDetailsDTO = db.OrderDetails.Where(x => x.OrderId == order.OrderId).ToList();

                    foreach (var item in orderDetailsDTO)
                    {
                        //pobieramy produkt 
                        ProductDTO product = db.Products.Where(x => x.Id == item.ProductId).FirstOrDefault();
                        decimal price = product.Price;
                        string name = product.Name;

                        productsAndQty.Add(name, item.Quantity);
                        total += item.Quantity * price;

                    }
                    ordersForUser.Add(new OrderForUserVM()
                    {
                        OrderNumber = order.OrderId,
                        Total = total,
                        ProductsAndQty = productsAndQty,
                        CreatedAt = order.CreateAdT
                    });
                }

            }




            return View(ordersForUser);
        }


        //get /account/calculators
        public ActionResult Calculators()
        {
            return View();
        }
    }
}