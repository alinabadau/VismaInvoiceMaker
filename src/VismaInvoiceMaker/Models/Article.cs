using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VismaInvoiceMaker.Models
{
    public class Article
    {
        public Guid ArticleId { get; set; }

        [Required(ErrorMessage = "Name required")]
        public string Name { get; set; }

        [Display(Name = "Notes")]
        public string Notes { get; set; }

        [Range(0.01, 999999999, ErrorMessage = "Price must be between 0.01 and 999999999")]
        [Required(ErrorMessage = "Price required")]
        public decimal Price { get; set; }

        [Range(0.00, 100, ErrorMessage = "VAT must be a % between 0 and 100")]
        [UIHint("NumericInput")]
        public decimal VAT { get; set; }
    }
}
