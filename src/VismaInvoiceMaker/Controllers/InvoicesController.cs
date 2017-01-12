using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using VismaInvoiceMaker.Data;
using VismaInvoiceMaker.Models;

namespace VismaInvoiceMaker.Controllers
{
    public class InvoicesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InvoicesController> _logger;
        private IConfiguration _config;

        public InvoicesController(ApplicationDbContext context, ILogger<InvoicesController> logger, IConfiguration config)
        {
            _context = context;
            _logger = logger;
            _config = config;
        }

        // GET: Invoices
        public async Task<IActionResult> Index()
        {
            return View(await _context.Invoice.ToListAsync());
        }

        [HttpGet]
        public JsonResult GetInvoices()
        {
            try
            {
                var model =
                    _context.Invoice.Include(i => i.Customer)
                        .Include(i => i.InvoiceDetails)
                        .ThenInclude(p => p.Article)
                        .ToList();
                return Json(model, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    ContractResolver = new DefaultContractResolver()
                });
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }

            return null;
        }

        public void DeleteInvoice(Guid invoiceId)
        {
            var invoice = _context.Invoice.Include(i => i.InvoiceDetails).FirstOrDefault(i => i.InvoiceId == invoiceId);
            if (invoice == null) return;
            _context.InvoiceDetails.RemoveRange(invoice.InvoiceDetails);
            _context.Invoice.Remove(invoice);

            _context.SaveChanges();
        }

        public ActionResult EditInvoice(Guid id)
        {
            if (id == Guid.Empty)
            {
                var max = _context.Invoice.ToList().Max(f => f.InvoiceNumber);

                return View(new Invoice
                {
                    InvoiceId = Guid.Empty,
                    InvoiceNumber = (max ?? 0) + 1
                });
            }
            else
            {
                var model =
                    _context.Invoice.Include(i => i.Customer).Include(i => i.Store)
                        .Include(i => i.InvoiceDetails)
                        .ThenInclude(p => p.Article)
                        .FirstOrDefault(i => i.InvoiceId == id);
                foreach (var detail in model.InvoiceDetails)
                {
                    detail.Invoice = null;

                }
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult SaveInvoice([FromBody] Invoice invoice)
        {
            try
            {
                if (invoice.Customer == null)
                    throw new Exception("Please select a Customer.");

                if (invoice.Store == null)
                    throw new Exception("Please select a Store.");

                var cleanList = invoice.InvoiceDetails.ToList();
                cleanList.RemoveAll(article => article.Article == null);
                invoice.InvoiceDetails = cleanList;

                if (invoice.InvoiceDetails == null || invoice.InvoiceDetails.Count == 0)
                    throw new Exception("Please select at least one article.");

                DeleteInvoice(invoice.InvoiceId);
                invoice.User = User.Identity.Name;
                _context.Customer.Attach(invoice.Customer);
                _context.Store.Attach(invoice.Store);
                _context.Article.AttachRange(invoice.InvoiceDetails.Select(s => s.Article));

                _context.Invoice.Add(invoice);
                _context.SaveChanges();

                return Json(new { success = true, id = invoice.InvoiceId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult Print(Guid id)
        {
            ViewBag.Print = true;
            var companyInfo = _config.GetSection("CompanyInfo").GetChildren();
            ViewBag.MyCompany = companyInfo.FirstOrDefault(k => k.Key == "MyCompanyName")?.Value;
            ViewBag.MyCompanyID = companyInfo.FirstOrDefault(k => k.Key == "MyCompanyID")?.Value;
            ViewBag.MyCompanyAddress = companyInfo.FirstOrDefault(k => k.Key == "MyCompanyAddress")?.Value;
            ViewBag.MyCompanyPhone = companyInfo.FirstOrDefault(k => k.Key == "MyCompanyPhone")?.Value;
            ViewBag.MyEmail = companyInfo.FirstOrDefault(k => k.Key == "MyEmail")?.Value;
            ViewBag.MyBankAccount = companyInfo.FirstOrDefault(k => k.Key == "MyBankAccount")?.Value;

            Invoice invoice = _context.Invoice.Include(i => i.Customer).Include(i => i.InvoiceDetails).ThenInclude(p => p.Article).FirstOrDefault(i => i.InvoiceId == id);
            return View(invoice);
        }

    }
}
