using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCafe.Models.CartViewModels
{
    public class CartViewModel
    {
        public List<CartMenuItem> CartMenuList { get; set; }
        public OrderHeader OrderHeader { get; set; }
        public List<Coupons> CouponsList { get; set; }
    }
}
