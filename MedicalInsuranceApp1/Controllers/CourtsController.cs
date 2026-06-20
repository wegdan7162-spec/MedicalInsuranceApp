using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Models.Entities;
using MedicalInsuranceApp1.Infrastrcture;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
    [RequirePermission(AppModules.Courts)]
    public class CourtsController : Controller
    {
        private readonly AppDbContext _db;
        public CourtsController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var courts = await _db.Courts.Where(x => (x.Del == null || x.Del == false)).OrderBy(x => x.CourtName).ToListAsync();
            return View(courts);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string CourtName)
        {
            if (string.IsNullOrWhiteSpace(CourtName))
            {
                TempData["Error"] = "اسم المحكمة مطلوب";
                return View();
            }

            _db.Courts.Add(new Court { CourtName = CourtName.Trim() });
            await _db.SaveChangesAsync();
            TempData["Success"] = $"تم إضافة المحكمة '{CourtName}' بنجاح";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var court = await _db.Courts.FindAsync(id);
            if (court == null) return NotFound();
            return View(court);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string CourtName)
        {
            if (string.IsNullOrWhiteSpace(CourtName))
            {
                TempData["Error"] = "اسم المحكمة مطلوب";
                return RedirectToAction("Edit", new { id });
            }

            var court = await _db.Courts.FindAsync(id);
            if (court == null) return NotFound();

            court.CourtName = CourtName.Trim();
            await _db.SaveChangesAsync();
            TempData["Success"] = $"تم تحديث المحكمة إلى '{CourtName}' بنجاح";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? reason)
        {
            var court = await _db.Courts.FindAsync(id);
            if (court != null)
            {
                court.Del = true;
                court.DeletedAt = DateTime.Now;
                court.DeletedBy = User.Identity?.Name;
                court.DeleteReason = reason;
                await _db.SaveChangesAsync();
            }
            TempData["Success"] = "تم حذف المحكمة بنجاح";
            return RedirectToAction("Index");
        }
    }
}