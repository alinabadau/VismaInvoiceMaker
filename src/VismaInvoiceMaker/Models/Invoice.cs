using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VismaInvoiceMaker.Models
{
    public sealed class Invoice
    {
        public Invoice()
        {
            InvoiceDetails = new List<InvoiceDetails>();
        }

        public Guid InvoiceId { get; set; }

        [Display(Name = "Invoice Number")]
        public int? InvoiceNumber { get; set; }

        public InvoiceState InvoiceState { get; set; }

        public string InvoiceStateDisplay => InvoiceState.ToString();

        public Customer Customer { get; set; }

        public Store Store { get; set; }

        [Display(Name = "Notes")]
        public string Notes { get; set; }

        [Display(Name = "Created")]
        public DateTime TimeStamp { get; set; }

        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        [Display(Name = "Advance Payment (%)")]
        [Range(0.00, 100.0, ErrorMessage = "Value must be a % between 0 and 100")]
        public decimal AdvancePaymentTax { get; set; }

        public bool Paid { get; set; }

        public string User { get; set; }

        public ICollection<InvoiceDetails> InvoiceDetails { get; set; }

        #region Calculated fields

        [Display(Name = "VAT Amount")]
        public decimal VATAmount
        {
            get
            {
                return TotalWithVAT - NetTotal;
            }
            set { }
        }

        public decimal NetTotal
        {
            get
            {
                return InvoiceDetails?.Sum(i => i.Total) ?? 0;
            }
            set { }
        }

        public decimal AdvancePaymentTaxAmount
        {
            get
            {
                return InvoiceDetails == null ? 0 : TotalWithVAT * (AdvancePaymentTax / 100);
            }
            set { }
        }

        [Display(Name = "Total with VAT")]
        public decimal TotalWithVAT
        {
            get
            {
                return InvoiceDetails?.Sum(i => i.TotalPlusVat) ?? 0;
            }
            set { }
        }

        [Display(Name = "Total to pay")]
        public decimal TotalToPay
        {
            get
            {
                return TotalWithVAT - AdvancePaymentTaxAmount;
            }
            set { }
        }

        #endregion
    }

    public enum InvoiceState
    {
        Draft = 0,
        Released = 1
    }
}
