using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyCafe.Models
{
    public class CartMenuItem
    {
        public int Id { get; set; }

        [Required]
        public int MenuItemId { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        [NotMapped]
        [ForeignKey("MenuItemId")]
        public MenuItem MenuItem { get; set; }

        [NotMapped]
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        [NotMapped]
        public string StatusMessage { get; set; }

        public int MenuCount { get; set; } = 1;
    }
}
