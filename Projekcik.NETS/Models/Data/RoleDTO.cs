using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Projekcik.NETS.Models.Data
{
    [Table("tblRole")]
    public class RoleDTO
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}