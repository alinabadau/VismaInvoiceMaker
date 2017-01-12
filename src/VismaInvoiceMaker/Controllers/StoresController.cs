using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using VismaInvoiceMaker.Data;
using VismaInvoiceMaker.Models;

namespace VismaInvoiceMaker.Controllers
{
    public class StoresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StoresController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Stores
        public async Task<IActionResult> Index()
        {
            return View(await _context.Store.ToListAsync());
        }

        public JsonResult GetStores([DataSourceRequest] DataSourceRequest request)
        {
            return Json(_context.Store.ToDataSourceResult(request), new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new DefaultContractResolver()
            });
        }

        // GET: Stores/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var store = await _context.Store.SingleOrDefaultAsync(m => m.StoreId == id);
            if (store == null)
            {
                return NotFound();
            }

            return View(store);
        }

        // GET: Stores/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Stores/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StoreId,Address,CP,City,ContactPerson,Email,Fax,Name,Notes,Phone1,Phone2")] Store store)
        {
            if (ModelState.IsValid)
            {
                store.StoreId = Guid.NewGuid();
                _context.Add(store);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(store);
        }

        // GET: Stores/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var store = await _context.Store.SingleOrDefaultAsync(m => m.StoreId == id);
            if (store == null)
            {
                return NotFound();
            }
            return View(store);
        }

        // POST: Stores/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("StoreId,Address,CP,City,ContactPerson,Email,Fax,Name,Notes,Phone1,Phone2")] Store store)
        {
            if (id != store.StoreId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(store);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StoreExists(store.StoreId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(store);
        }

        // GET: Stores/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var store = await _context.Store.SingleOrDefaultAsync(m => m.StoreId == id);
            if (store == null)
            {
                return NotFound();
            }

            return View(store);
        }

        // POST: Stores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var store = await _context.Store.SingleOrDefaultAsync(m => m.StoreId == id);
            _context.Store.Remove(store);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool StoreExists(Guid id)
        {
            return _context.Store.Any(e => e.StoreId == id);
        }
    }
}
