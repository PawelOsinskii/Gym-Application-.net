using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Projekcik.NETS.Areas.Admin.Models.ViewModels.Shop
{
    public class OrderForAdminVM
    {
        public int OrderNumber { get; set; }
        public string UserName { get; set; }
        public decimal Total { get; set; }
        public Dictionary<string,int> ProductsAndQnty { get; set; }
        public DateTime CreeatedAt { get; set; }
    }
}