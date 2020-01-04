using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCafe.Models.OrderViewModels
{
    //For Order Details In OrderHistory
    public class OrderDetailsVM
    {
        public OrderHeader OrderHeader { get; set; }
        public List<OrderDetails> OrderDetails { get; set; }
    }
}
