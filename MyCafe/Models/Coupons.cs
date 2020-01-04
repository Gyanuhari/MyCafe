using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCafe.Models
{
    public class Coupons
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required,Display(Name ="Coupon Type")]

        public string CouponType { get; set; }
        public enum ECouponType
        {
            [Display(Name ="Discount In Percentage")]
            Percent=0,
            [Display(Name ="Discount In Dollar")]
            Dollar=1,
        }

        [Required]
        public double Discount { get; set; }

        [Range(1,double.MaxValue,ErrorMessage ="Please enter value greater than 1.")]
        [Required, Display(Name ="Minimum Amount")]
        public double MinimumAmount { get; set; }

        public byte[] Picture { get; set; }
        public bool IsActive { get; set; }
    }
}
