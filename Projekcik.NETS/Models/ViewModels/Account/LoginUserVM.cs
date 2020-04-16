using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Projekcik.NETS.Models.ViewModels.Account
{
    public class LoginUserVM
    {
        [Required]
        [Display(Name="Nazwa Użytkownika")]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "Hasło")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}