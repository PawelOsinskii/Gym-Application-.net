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
        public Boolean Karnet { get; set; }
        public DateTime TimeFinish { get; set; }


        
    }
}