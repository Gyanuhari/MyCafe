using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyCafe.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }


        [Required]
        public int OrderHeaderId { get; set; }

        [ForeignKey("OrderHeaderId")]
        public OrderHeader OrderHeader { get; set; }


        [Required]
        public int MenuItemId { get; set; }

        [ForeignKey("MenuItemId")]
        public virtual MenuItem MenuItem { get; set; }

        //If incase someone edits the Name and Price the record in orderdetails woun't change
        [Required]
        public string MenuName { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public int MenuCount { get; set; }
    }
}
