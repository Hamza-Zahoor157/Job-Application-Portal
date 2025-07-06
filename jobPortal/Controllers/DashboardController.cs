using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobPortal.Data;
using System.Linq;
using System.Threading.Tasks;

namespace jobPortal.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var topPositions = await _context.JobPositions
                .Include(jp => jp.Applicants)
                .OrderByDescending(jp => jp.Applicants.Count)
                .Take(5)
                .ToListAsync();
            return View(topPositions);
        }
    }
} 