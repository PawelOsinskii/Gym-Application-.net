using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Projekcik.NETS.Models.ViewModels.Account
{
    public class UserNavPartialVM
    {
        public string UserName { get; set; }
        public bool HasKarnet { get; set; }
        public string MamKarnet { get; set; }
        public string NieMamKarnetu { get; set; }
    }
}