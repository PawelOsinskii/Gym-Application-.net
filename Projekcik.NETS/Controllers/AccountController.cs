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
            return View();
        }

        // GET: Account/create-account
        [ActionName("create-account")]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }
    }
}