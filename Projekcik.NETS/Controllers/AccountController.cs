using Projekcik.NETS.Models.Data;
using Projekcik.NETS.Models.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            using(Db db = new Db())
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
                    // todo tutaj trzeba dodać kodowanie hasła
                    Password = model.Password
                };
                db.User.Add(userDTO);
                db.SaveChanges();

                //dodanie roli dla użytkownika

                UserRoleDTO userRoleDTO = new UserRoleDTO()
                {
                    UserId = userDTO.Id,
                    RoleId = 2
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
    }
}