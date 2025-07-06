using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using JobPortal.Data;
using JobPortal.Models;

namespace jobPortal.Controllers
{
    public class JobPositionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JobPositionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: JobPositions
        [Authorize(Policy = "HRorAdminOnly")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.JobPositions.ToListAsync());
        }

        // GET: JobPositions/Details/5
        [Authorize(Policy = "HRorAdminOnly")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobPosition = await _context.JobPositions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobPosition == null)
            {
                return NotFound();
            }

            return View(jobPosition);
        }

        // GET: JobPositions/Create
        [Authorize(Policy = "HRorAdminOnly")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: JobPositions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Policy = "HRorAdminOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Salary")] JobPosition jobPosition)
        {
            // Debug: Log the model state
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["ValidationErrors"] = string.Join(", ", errors);
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(jobPosition);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["DatabaseError"] = ex.Message;
                    return View(jobPosition);
                }
            }
            return View(jobPosition);
        }

        // GET: JobPositions/Edit/5
        [Authorize(Policy = "HRorAdminOnly")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobPosition = await _context.JobPositions.FindAsync(id);
            if (jobPosition == null)
            {
                return NotFound();
            }
            return View(jobPosition);
        }

        // POST: JobPositions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Policy = "HRorAdminOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Salary")] JobPosition jobPosition)
        {
            if (id != jobPosition.Id)
            {
                return NotFound();
            }

            // Debug: Log the model state
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["ValidationErrors"] = string.Join(", ", errors);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jobPosition);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobPositionExists(jobPosition.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    TempData["DatabaseError"] = ex.Message;
                    return View(jobPosition);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(jobPosition);
        }

        // GET: JobPositions/Delete/5
        [Authorize(Policy = "HRorAdminOnly")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobPosition = await _context.JobPositions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobPosition == null)
            {
                return NotFound();
            }

            return View(jobPosition);
        }

        // POST: JobPositions/Delete/5

        [Authorize(Policy = "HRorAdminOnly")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jobPosition = await _context.JobPositions.FindAsync(id);
            if (jobPosition != null)
            {
                _context.JobPositions.Remove(jobPosition);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobPositionExists(int id)
        {
            return _context.JobPositions.Any(e => e.Id == id);
        }
    }
}
