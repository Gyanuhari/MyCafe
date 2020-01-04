using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyCafe.Models
{
    public class MenuItem
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        [Range(1,int.MaxValue, ErrorMessage ="Price should be greater than $1.00")]
        public double Price { get; set; }

        public string Spicyness { get; set; }
        public enum ESpicy
        {
            [Display(Name ="No Spice")]
            NA =0,
            [Display(Name ="Little Spice")]
            Spicy =1,
            [Display(Name ="More Spice")]
            VerySpicy =2
        }

        [Display(Name ="Category")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }


        [Display(Name = "SubCategory")]
        public int SubCategoryId { get; set; }

        [ForeignKey("SubCategoryId")]
        public virtual SubCategory SubCategory { get; set; }
    }
}
