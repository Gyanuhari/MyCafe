using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyCafe.Models
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }

        //For Someone who is going to pickup the order
        [Required]
        public string Name { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required, Display(Name = "Zip Code")]
        public string ZipCode { get; set; }

        //To keep the record of the one who made the order
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }


        [Required, Display(Name="Pickup Time")]
        public DateTime PickupTime { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required, Display(Name="Order Total")]
        public double OrderTotal { get; set; }

        //If Coupon is used, if admin edits the coupons we can still view the coupon used
        public string CouponCode { get; set; }

        //There is no point if someone edits the Coupons
        //[Required]
        //public int CouponId { get; set; }

        //[ForeignKey("CouponId")]
        //public virtual Coupons Coupon { get; set; }

        public string Status { get; set; }

        public string Comments { get; set; }
    }
}
