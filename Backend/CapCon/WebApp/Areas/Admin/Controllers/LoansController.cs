using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Contracts.DAL;
using App.Contracts.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.DAL.EF;
using App.DAL.EF.Repositories;
using App.Domain;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class LoansController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILoanRepository _repo;
        private readonly IAppUnitOfWork _uow;
        private readonly Mapper _mapper;


        public LoansController(AppDbContext context, IAppUnitOfWork uow, Mapper mapper)
        {
            _context = context;
            _uow = uow;
            _mapper = mapper;
            _repo = new LoanRepository(context, _mapper);
        }

        // GET: Admin/Loans
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Loans.Include(l => l.AppUser).Include(l => l.Company);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Admin/Loans/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loans
                .Include(l => l.AppUser)
                .Include(l => l.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }

        // GET: Admin/Loans/Create
        public IActionResult Create()
        {
            ViewData["AppUserId"] = new SelectList(_context.AppUsers, "Id", "Id");
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "CompanyName");
            return View();
        }

        // POST: Admin/Loans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CompanyId,AppUserId,Amount,Interest,Period,StartDate,EndDate,Comment,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt,Id")] Loan loan)
        {
            if (ModelState.IsValid)
            {
                loan.Id = Guid.NewGuid();
                _context.Add(loan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AppUserId"] = new SelectList(_context.AppUsers, "Id", "Id", loan.AppUserId);
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "CompanyName", loan.CompanyId);
            return View(loan);
        }

        // GET: Admin/Loans/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loans.FindAsync(id);
            if (loan == null)
            {
                return NotFound();
            }
            ViewData["AppUserId"] = new SelectList(_context.AppUsers, "Id", "Id", loan.AppUserId);
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "CompanyName", loan.CompanyId);
            return View(loan);
        }

        // POST: Admin/Loans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CompanyId,AppUserId,Amount,Interest,Period,StartDate,EndDate,Comment,CreatedBy,CreatedAt,UpdatedBy,UpdatedAt,Id")] Loan loan)
        {
            if (id != loan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoanExists(loan.Id))
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
            ViewData["AppUserId"] = new SelectList(_context.AppUsers, "Id", "Id", loan.AppUserId);
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "CompanyName", loan.CompanyId);
            return View(loan);
        }

        // GET: Admin/Loans/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loans
                .Include(l => l.AppUser)
                .Include(l => l.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }

        // POST: Admin/Loans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan != null)
            {
                _context.Loans.Remove(loan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoanExists(Guid id)
        {
            return _context.Loans.Any(e => e.Id == id);
        }
    }
}
