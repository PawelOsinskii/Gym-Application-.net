﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projekcik.NETS.Models.Data
{
    [Table("tblProduct")]
    public class ProductDTO
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string slug { get; set; }
        public int Description { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public string ImageName { get; set; }

        [ForeignKey("CategoryId")]
        public virtual CategoryDTO Category { get; set; }

    }
}