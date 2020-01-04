using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCafe.Models.CategoryAndSubCategoryViewModels
{
    public class CategoryAndSubCategoryVM
    {
        public SubCategory SubCategory { get; set; }
        //For displaying categories in dropdown
        public IEnumerable<Category> CategoryList { get; set; }
        
        //For displaying list of name of categories
        public List<string> SubCategoryList { get; set; }

        [Display(Name="New SubCategory")]
        public bool IsNew { get; set; }

        public string StatusMessage { get; set; }
    }
}
