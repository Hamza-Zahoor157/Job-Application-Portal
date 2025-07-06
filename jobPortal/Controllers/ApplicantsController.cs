using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using JobPortal.Data;
using JobPortal.Models;
using Microsoft.AspNetCore.Identity;

namespace jobPortal.Controllers
{
    public class ApplicantsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicantsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Applicants
        [Authorize(Policy = "ApplicantOnly")]
        public async Task<IActionResult> Index()
        {
            var userEmail = User.Identity.Name;
            var applicationDbContext = _context.Applicants.Include(a => a.JobPosition).Where(a => a.Email == userEmail);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Applicants/Details/5
        [Authorize(Policy = "ApplicantOnly")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicant = await _context.Applicants
                .Include(a => a.JobPosition)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicant == null)
            {
                return NotFound();
            }
            if (applicant.Email != User.Identity.Name)
            {
                return Forbid();
            }
            return View(applicant);
        }

        // GET: Applicants/Create
        [Authorize(Policy = "ApplicantOnly")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["JobPositionId"] = new SelectList(_context.JobPositions, "Id", "Title");
            return View();
        }

        // POST: Applicants/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [Authorize(Policy = "ApplicantOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int JobPositionId, IFormFile ResumeFile)
        {
            // Debug: Check if we're even reaching this method
            TempData["Debug"] = "POST method reached. JobPositionId: " + JobPositionId + 
                               ", ResumeFile: " + (ResumeFile?.FileName ?? "null");
            
            var user = await _userManager.GetUserAsync(User);
            
            // Create new applicant object
            var applicant = new Applicant
            {
                JobPositionId = JobPositionId,
                ResumeFile = ResumeFile,
                Email = user.Email,
                FullName = user.FullName
            };

            // Basic validation
            if (JobPositionId <= 0)
            {
                TempData["UploadError"] = "Please select a job position.";
                ViewData["JobPositionId"] = new SelectList(_context.JobPositions, "Id", "Title", JobPositionId);
                return View(applicant);
            }

            if (ResumeFile == null || ResumeFile.Length == 0)
            {
                TempData["UploadError"] = "Please select a resume file.";
                ViewData["JobPositionId"] = new SelectList(_context.JobPositions, "Id", "Title", JobPositionId);
                return View(applicant);
            }



            try
            {
                var allowedExtensions = new[] { ".pdf", ".docx", ".jpg", ".png" };
                var extension = Path.GetExtension(applicant.ResumeFile.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    TempData["UploadError"] = "Only PDF, DOCX, JPG, and PNG files are allowed.";
                    ViewData["JobPositionId"] = new SelectList(_context.JobPositions, "Id", "Title", applicant.JobPositionId);
                    return View(applicant);
                }

                // Check file size (10MB limit)
                if (applicant.ResumeFile.Length > 10 * 1024 * 1024)
                {
                    TempData["UploadError"] = "File size must be less than 10MB.";
                    ViewData["JobPositionId"] = new SelectList(_context.JobPositions, "Id", "Title", applicant.JobPositionId);
                    return View(applicant);
                }

                // Save file to wwwroot/Resumes
                var resumesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Resumes");
                if (!Directory.Exists(resumesFolder))
                {
                    Directory.CreateDirectory(resumesFolder);
                }

                var fileName = Guid.NewGuid().ToString() + extension;
                var filePath = Path.Combine(resumesFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await applicant.ResumeFile.CopyToAsync(stream);
                }

                // Verify file was actually created
                if (!System.IO.File.Exists(filePath))
                {
                    TempData["UploadError"] = "Failed to save the uploaded file.";
                    ViewData["JobPositionId"] = new SelectList(_context.JobPositions, "Id", "Title", applicant.JobPositionId);
                    return View(applicant);
                }

                // Save file path to DB
                applicant.ResumeUrl = "/Resumes/" + fileName;

                _context.Applicants.Add(applicant);
                await _context.SaveChangesAsync();

                TempData["UploadSuccess"] = "Application submitted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["UploadError"] = "Error: " + ex.Message;
                ViewData["JobPositionId"] = new SelectList(_context.JobPositions, "Id", "Title", applicant.JobPositionId);
                return View(applicant);
            }
        }


        // GET: Applicants/Edit/5
        [Authorize(Policy = "ApplicantOnly")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicant = await _context.Applicants.FindAsync(id);
            if (applicant == null)
            {
                return NotFound();
            }
            if (applicant.Email != User.Identity.Name)
            {
                return Forbid();
            }
            ViewData["JobPositionId"] = new SelectList(_context.JobPositions, "Id", "Title", applicant.JobPositionId);
            return View(applicant);
        }

        // POST: Applicants/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "ApplicantOnly")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,JobPositionId")] Applicant applicant, IFormFile ResumeFile)
        {
            var user = await _userManager.GetUserAsync(User);
            var dbApplicant = await _context.Applicants.FindAsync(id);
            if (dbApplicant == null)
                return NotFound();
            if (dbApplicant.Email != user.Email)
                return Forbid();

            dbApplicant.JobPositionId = applicant.JobPositionId;
            dbApplicant.FullName = user.FullName;
            dbApplicant.Email = user.Email;

            // Resume logic
            if (ResumeFile != null && ResumeFile.Length > 0)
            {
                var allowedExtensions = new[] { ".pdf", ".docx", ".jpg", ".png" };
                var extension = Path.GetExtension(ResumeFile.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                {
                    TempData["UploadError"] = "Only PDF, DOCX, JPG, and PNG files are allowed.";
                    ViewData["JobPositionId"] = new SelectList(_context.JobPositions, "Id", "Title", applicant.JobPositionId);
                    return View(dbApplicant);
                }
                if (ResumeFile.Length > 10 * 1024 * 1024)
                {
                    TempData["UploadError"] = "File size must be less than 10MB.";
                    ViewData["JobPositionId"] = new SelectList(_context.JobPositions, "Id", "Title", applicant.JobPositionId);
                    return View(dbApplicant);
                }
                var resumesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Resumes");
                if (!Directory.Exists(resumesFolder))
                    Directory.CreateDirectory(resumesFolder);
                var fileName = Guid.NewGuid().ToString() + extension;
                var filePath = Path.Combine(resumesFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ResumeFile.CopyToAsync(stream);
                }
                dbApplicant.ResumeUrl = "/Resumes/" + fileName;
            }
            // If no resume file uploaded and no existing resume, require it
            if (dbApplicant.ResumeUrl == null)
            {
                TempData["UploadError"] = "Resume file is required.";
                ViewData["JobPositionId"] = new SelectList(_context.JobPositions, "Id", "Title", applicant.JobPositionId);
                return View(dbApplicant);
            }
            try
            {
                _context.Update(dbApplicant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApplicantExists(applicant.Id))
                    return NotFound();
                else
                    throw;
            }
        }

        // GET: Applicants/Delete/5
        [Authorize(Policy = "ApplicantOnly")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicant = await _context.Applicants
                .Include(a => a.JobPosition)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicant == null)
            {
                return NotFound();
            }
            if (applicant.Email != User.Identity.Name)
            {
                return Forbid();
            }
            return View(applicant);
        }

        // POST: Applicants/Delete/5
        [Authorize(Policy = "ApplicantOnly")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var applicant = await _context.Applicants.FindAsync(id);
            if (applicant == null)
            {
                return NotFound();
            }
            if (applicant.Email != User.Identity.Name)
            {
                return Forbid();
            }
            _context.Applicants.Remove(applicant);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApplicantExists(int id)
        {
            return _context.Applicants.Any(e => e.Id == id);
        }
    }
}
