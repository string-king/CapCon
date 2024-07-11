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

    public class UserCompaniesController : Controller
    {
        private readonly AppDbContext _context;

        public UserCompaniesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/UserCompanies
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.UserCompanies.Include(u => u.AppUser).Include(u => u.Company);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Admin/UserCompanies/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userCompany = await _context.UserCompanies
                .Include(u => u.AppUser)
                .Include(u => u.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userCompany == null)
            {
                return NotFound();
            }

            return View(userCompany);
        }

        // GET: Admin/UserCompanies/Create
        public IActionResult Create()
        {
            ViewData["AppUserId"] = new SelectList(_context.AppUsers, "Id", "Id");
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "CompanyName");
            return View();
        }

        // POST: Admin/UserCompanies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppUserId,CompanyId,Id")] UserCompany userCompany)
        {
            if (ModelState.IsValid)
            {
                userCompany.Id = Guid.NewGuid();
                _context.Add(userCompany);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AppUserId"] = new SelectList(_context.AppUsers, "Id", "Id", userCompany.AppUserId);
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "CompanyName", userCompany.CompanyId);
            return View(userCompany);
        }

        // GET: Admin/UserCompanies/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userCompany = await _context.UserCompanies.FindAsync(id);
            if (userCompany == null)
            {
                return NotFound();
            }
            ViewData["AppUserId"] = new SelectList(_context.AppUsers, "Id", "Id", userCompany.AppUserId);
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "CompanyName", userCompany.CompanyId);
            return View(userCompany);
        }

        // POST: Admin/UserCompanies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("AppUserId,CompanyId,Id")] UserCompany userCompany)
        {
            if (id != userCompany.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userCompany);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserCompanyExists(userCompany.Id))
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
            ViewData["AppUserId"] = new SelectList(_context.AppUsers, "Id", "Id", userCompany.AppUserId);
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "CompanyName", userCompany.CompanyId);
            return View(userCompany);
        }

        // GET: Admin/UserCompanies/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userCompany = await _context.UserCompanies
                .Include(u => u.AppUser)
                .Include(u => u.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userCompany == null)
            {
                return NotFound();
            }

            return View(userCompany);
        }

        // POST: Admin/UserCompanies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var userCompany = await _context.UserCompanies.FindAsync(id);
            if (userCompany != null)
            {
                _context.UserCompanies.Remove(userCompany);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserCompanyExists(Guid id)
        {
            return _context.UserCompanies.Any(e => e.Id == id);
        }
    }
}
