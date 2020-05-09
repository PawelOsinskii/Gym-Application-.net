using Projekcik.NETS.Models.Data;
using System;

namespace Projekcik.NETS.Models.ViewModels.Shop
{

    public class OrderVM
    {
        public OrderVM()
        {

        }
        public OrderVM(OrderDTO dto)
        {
            OrderId = dto.Id;
            UserId = dto.UserId;
            CreateAdT = dto.CreatedAt;

        }
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime CreateAdT { get; set; }
    }
}