using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCafe.Models.OrderViewModels
{
    public class OrderListVM
    {
        public List<OrderDetails> OrderDetailsList { get; set; }
        public List<OrderHeader> OrderHeadersList { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
