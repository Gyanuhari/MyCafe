using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCafe.Models.OrderViewModels
{
    public class OrderDownloadVM
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
