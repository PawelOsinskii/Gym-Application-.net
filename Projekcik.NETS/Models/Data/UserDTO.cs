using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Projekcik.NETS.Models.ViewModels.Account;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projekcik.NETS.Models.Data
{
    [Table("tblUser")]
    public class UserDTO
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAdress { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        
        public System.Nullable<DateTime> TimeFinish { get; set; }

        public static bool hasMemberShip(UserDTO user)
        {
            if (user.TimeFinish == null)
                return false;
            else
                return DateTime.Compare((DateTime)user.TimeFinish, DateTime.Now) >= 0;
        }

        private static string hashPassword(string username, string password)
        {
            string saltString = "sol123123123" + username;
            byte[] salt = System.Text.Encoding.ASCII.GetBytes(saltString);
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
                ));

        }
        public static bool verifyPassword(LoginUserVM user, UserDTO userdto)
        {

            return UserDTO.hashPassword(user.UserName, user.Password) == userdto.Password;
        }
        public static void hashPass(UserVM user, UserDTO userdto)
        {

            userdto.Password = UserDTO.hashPassword(user.UserName, user.Password);
        }


    }
}