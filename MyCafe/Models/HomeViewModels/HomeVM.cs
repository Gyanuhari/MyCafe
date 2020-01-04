using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCafe.Models.HomeViewModels
{
    public class HomeVM
    {
        public IEnumerable<MenuItem> MenuItems { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Coupons> Coupons { get; set; }

        public string StatusMessage { get; set; }
    }
}
