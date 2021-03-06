﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VismaInvoiceMaker.Models
{
    public class Customer
    {

        public Guid CustomerId { get; set; }

        [Required(ErrorMessage = "Name required")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Company number")]
        [Required(ErrorMessage = "Company number required")]
        public string CompanyNumber { get; set; }

        [Required(ErrorMessage = "Address required")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Zip code required")]
        [Display(Name = "Zip Code")]
        public string CP { get; set; }

        [Required(ErrorMessage = "City required")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required(ErrorMessage = "Contact person required")]
        [Display(Name = "Contact person")]
        public string ContactPerson { get; set; }

        [Required(ErrorMessage = "Telephone required")]
        [Display(Name = "Telephone")]
        public string Phone1 { get; set; }

        [Display(Name = "Mobile")]
        public string Phone2 { get; set; }

        public string Fax { get; set; }

        [Required(ErrorMessage = "Email required")]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*",
            ErrorMessage = "Wrong email format")]
        public string Email { get; set; }

        public string Notes { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; }
    }
}
