using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.Domain;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class LoanRequestsController : Controller
    {
        private readonly AppDbContext _context;

        public LoanRequestsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/LoanRequests
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.LoanRequests.Include(l => l.Company);
            
            return View(await appDbContext.ToListAsync());
        }

        // GET: Admin/LoanRequests/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loanRequest = await _context.LoanRequests
                .Include(l => l.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loanRequest == null)
            {
                return NotFound();
            }

            return View(loanRequest);
        }

        // GET: Admin/LoanRequests/Create
        public IActionResult Create()
        {
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "CompanyName");
            return View();
        }

        // POST: Admin/LoanRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CompanyId,Amount,Interest,Period,Active,Comment,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt,Id")] LoanRequest loanRequest)
        {
            if (ModelState.IsValid)
            {
                loanRequest.Id = Guid.NewGuid();
                _context.Add(loanRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "CompanyName", loanRequest.CompanyId);
            return View(loanRequest);
        }

        // GET: Admin/LoanRequests/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loanRequest = await _context.LoanRequests.FindAsync(id);
            if (loanRequest == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "CompanyName", loanRequest.CompanyId);
            return View(loanRequest);
        }

        // POST: Admin/LoanRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CompanyId,Amount,Interest,Period,Active,Comment,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt,Id")] LoanRequest loanRequest)
        {
            if (id != loanRequest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loanRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoanRequestExists(loanRequest.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "CompanyName", loanRequest.CompanyId);
            return View(loanRequest);
        }

        // GET: Admin/LoanRequests/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loanRequest = await _context.LoanRequests
                .Include(l => l.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loanRequest == null)
            {
                return NotFound();
            }

            return View(loanRequest);
        }

        // POST: Admin/LoanRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var loanRequest = await _context.LoanRequests.FindAsync(id);
            if (loanRequest != null)
            {
                _context.LoanRequests.Remove(loanRequest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoanRequestExists(Guid id)
        {
            return _context.LoanRequests.Any(e => e.Id == id);
        }
    }
}
