using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCafe.Models.OrderViewModels
{
    public class OrderVM
    {
        public List<OrderDetails> OrderDetailsList { get; set; }
        public List<OrderHeader> OrderHeadersList { get; set; }
    }
}
