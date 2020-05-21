using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Projekcik.NETS.Models.Data;
using Projekcik.NETS.Models.ViewModels.Account;
using Projekcik.NETS.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;

namespace Projekcik.NETS.Controllers
{
    public class OrderUnit
    {
        [JsonPropertyName("amount")]
        public OrderAmount Amount { get; set; }
    }

    public class OrderAmount
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }

    public class OrderParameters
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("purchase_units")]
        public List<OrderUnit> PurchaseUnits { get; set; }
    }

    public class AccesToken
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }
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
                    HasKarnet = UserDTO.hasMemberShip(dto)


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



        [HttpPost]
        async public Task<bool> Subscribe(string orderID)
        {
            string UserName = User.Identity.Name;

            HttpClient client = new HttpClient();

            var values = new Dictionary<string, string>
            {
                {"grant_type", "client_credentials" }
            };
            var content = new FormUrlEncodedContent(values);

            var byteArray = new System.Text.UTF8Encoding().GetBytes("AeDjKrLsusKVxAL41eu2ek1emSQmilxNbdtMJ9yEMMabWWNPIBrDGq_fjxpC-yBWAq1Wk6ZMW5PipcoT:ECmcbxwOFJ2IqGzv5NKIxyoRx2HgE_tJhoPv3F4zoUJS3AR49Eiu8PIQ9YFrryUut4PYlugtLexmVkxI");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var response = await client.PostAsync("https://api.sandbox.paypal.com/v1/oauth2/token", content);
            string responseString = await response.Content.ReadAsStringAsync();


            AccesToken accessToken = JsonSerializer.Deserialize<AccesToken>(responseString);


            HttpClient client2 = new HttpClient();

            client2.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken.AccessToken);
            var response2 = await client2.GetAsync("https://api.sandbox.paypal.com/v2/checkout/orders/" + orderID);

            string responseString2 = await response2.Content.ReadAsStringAsync();
            OrderParameters orderParameters = JsonSerializer.Deserialize<OrderParameters>(responseString2);


            if (!orderParameters.Status.Equals("COMPLETED"))
                return false;
            int days = 0;
            if (orderParameters.PurchaseUnits[0].Amount.Value.Equals("99.00")) 
                days = 30;
            if (orderParameters.PurchaseUnits[0].Amount.Value.Equals("179.00"))
                days = 60;
            if (orderParameters.PurchaseUnits[0].Amount.Value.Equals("250.00"))
                days = 90;

            using (Db db = new Db())
            {
                var user = db.User.FirstOrDefault(x => x.UserName == UserName);
                int userId = user.Id;
                //ustawienie dto  i zapis 
                if (!user.TimeFinish.HasValue)
                    user.TimeFinish = DateTime.Now.AddDays(days);
                else
                    user.TimeFinish = ((DateTime)user.TimeFinish).AddDays(days);


                db.SaveChanges();
            }

            return true;
        }
    }
}