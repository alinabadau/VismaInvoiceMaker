using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VismaInvoiceMaker.Models
{
    public class InvoiceDetails
    {
        public Guid InvoiceDetailsId { get; set; }

        public Invoice Invoice { get; set; }

        [UIHint("ClientArticles")]
        public Article Article { get; set; }

        [Range(-100000, 100000, ErrorMessage = "Quantity must be between 1 and 100000")]
        [UIHint("NumericInput")]
        public int Qty { get; set; }

        [Display(Name = "Created")]
        public DateTime TimeStamp { get; set; }
        public decimal Price { get; set; }

        public decimal VAT { get; set; }

        #region Calculated fields

        public decimal Total
        {
            get
            {
                return Qty * Price;
            }
            set { }
        }


        public decimal VatAmount
        {
            get
            {
                return TotalPlusVat - Total;
            }
            set { }
        }


        public decimal TotalPlusVat
        {
            get
            {
                return Total * (1 + VAT / 100);
            }
            set { }
        }


        #endregion
    }
}
