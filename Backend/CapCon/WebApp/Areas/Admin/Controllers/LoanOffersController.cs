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

    public class LoanOffersController : Controller
    {
        private readonly AppDbContext _context;

        public LoanOffersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/LoanOffers
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.LoanOffers.Include(l => l.AppUser).Include(l => l.LoanRequest);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Admin/LoanOffers/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loanOffer = await _context.LoanOffers
                .Include(l => l.AppUser)
                .Include(l => l.LoanRequest)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loanOffer == null)
            {
                return NotFound();
            }

            return View(loanOffer);
        }

        // GET: Admin/LoanOffers/Create
        public IActionResult Create()
        {
            ViewData["AppUserId"] = new SelectList(_context.AppUsers, "Id", "Id");
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "CompanyName");
            ViewData["LoanRequestId"] = new SelectList(_context.LoanRequests, "Id", "CreatedBy");
            return View();
        }

        // POST: Admin/LoanOffers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppUserId,CompanyId,LoanRequestId,Amount,Interest,Period,Active,Comment,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt,Id")] LoanOffer loanOffer)
        {
            if (ModelState.IsValid)
            {
                loanOffer.Id = Guid.NewGuid();
                _context.Add(loanOffer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AppUserId"] = new SelectList(_context.AppUsers, "Id", "Id", loanOffer.AppUserId);
            ViewData["LoanRequestId"] = new SelectList(_context.LoanRequests, "Id", "CreatedBy", loanOffer.LoanRequestId);
            return View(loanOffer);
        }

        // GET: Admin/LoanOffers/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loanOffer = await _context.LoanOffers.FindAsync(id);
            if (loanOffer == null)
            {
                return NotFound();
            }
            ViewData["AppUserId"] = new SelectList(_context.AppUsers, "Id", "Id", loanOffer.AppUserId);
            ViewData["LoanRequestId"] = new SelectList(_context.LoanRequests, "Id", "CreatedBy", loanOffer.LoanRequestId);
            return View(loanOffer);
        }

        // POST: Admin/LoanOffers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("AppUserId,CompanyId,LoanRequestId,Amount,Interest,Period,Active,Comment,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt,Id")] LoanOffer loanOffer)
        {
            if (id != loanOffer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loanOffer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoanOfferExists(loanOffer.Id))
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
            ViewData["AppUserId"] = new SelectList(_context.AppUsers, "Id", "Id", loanOffer.AppUserId);
            ViewData["LoanRequestId"] = new SelectList(_context.LoanRequests, "Id", "CreatedBy", loanOffer.LoanRequestId);
            return View(loanOffer);
        }

        // GET: Admin/LoanOffers/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loanOffer = await _context.LoanOffers
                .Include(l => l.AppUser)
                .Include(l => l.LoanRequest)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loanOffer == null)
            {
                return NotFound();
            }

            return View(loanOffer);
        }

        // POST: Admin/LoanOffers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var loanOffer = await _context.LoanOffers.FindAsync(id);
            if (loanOffer != null)
            {
                _context.LoanOffers.Remove(loanOffer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoanOfferExists(Guid id)
        {
            return _context.LoanOffers.Any(e => e.Id == id);
        }
    }
}
