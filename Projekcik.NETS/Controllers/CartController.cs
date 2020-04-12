﻿using Projekcik.NETS.Models.ViewModels.Cart;
using System.Collections.Generic;
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
            foreach(var item in cart)
            {
                total += item.Price;
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