using Projekcik.NETS.Models.Data;
using System;
using System.ComponentModel.DataAnnotations;

namespace Projekcik.NETS.Models.ViewModels.Account
{
    public class UserProfileVM
    {
        public UserProfileVM()
        {

        }
        public UserProfileVM(UserDTO dto)
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
        [Display(Name = "Hasło")]
        public string Password { get; set; }
        [Display(Name = "Potwierdź hasło")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "obecne hasło")]
        public string OldPassword { get; set; }

        public string TimeFinish { get; set; }
    }
}
