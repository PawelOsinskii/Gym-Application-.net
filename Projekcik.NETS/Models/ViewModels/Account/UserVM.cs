using Projekcik.NETS.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Projekcik.NETS.Models.ViewModels.Account
{
    
    public class UserVM
    {
        public UserVM()
        {
                
        }
        public UserVM(UserDTO dto)
        {
            Id = dto.Id;
            FirstName = dto.FirstName;
            LastName = dto.LastName;
            EmailAdress = dto.EmailAdress;
            UserName = dto.UserName;
            Password = dto.Password;
            if (!UserDTO.hasMemberShip(dto))
            {
                TimeFinish = "brak karnetu";
            }
            else
            {
                TimeFinish = dto.TimeFinish.ToString();
            }

        }

        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAdress { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
        public Boolean Karnet { get; set; }
        public string TimeFinish { get; set; }
    }
}