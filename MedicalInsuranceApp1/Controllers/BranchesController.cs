using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Models.Entities;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
    public class BranchesController : Controller
    {
        private readonly AppDbContext _db;
        public BranchesController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var branches = await _db.Branches
                .Where(x => (x.Del == null || x.Del == false)).OrderBy(x => x.BranchName)
                .ToListAsync();
            return View(branches);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string BranchName)
        {
            if (string.IsNullOrWhiteSpace(BranchName))
            {
                TempData["Error"] = "اسم الفرع مطلوب";
                return View();
            }

            _db.Branches.Add(new Branch { BranchName = BranchName.Trim(), Del = false });
            await _db.SaveChangesAsync();
            TempData["Success"] = $"تم إضافة الفرع '{BranchName}' بنجاح";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var branch = await _db.Branches.FindAsync(id);
            if (branch == null) return NotFound();
            return View(branch);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string BranchName)
        {
            if (string.IsNullOrWhiteSpace(BranchName))
            {
                TempData["Error"] = "اسم الفرع مطلوب";
                return RedirectToAction("Edit", new { id });
            }

            var branch = await _db.Branches.FindAsync(id);
            if (branch == null) return NotFound();

            branch.BranchName = BranchName.Trim();
            await _db.SaveChangesAsync();
            TempData["Success"] = $"تم تحديث الفرع إلى '{BranchName}' بنجاح";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? reason)
        {
            var branch = await _db.Branches.FindAsync(id);
            if (branch != null)
            {
                branch.Del = true;
                branch.DeletedAt = DateTime.Now;
                branch.DeletedBy = User.Identity?.Name;
                branch.DeleteReason = reason;
                await _db.SaveChangesAsync();
            }
            TempData["Success"] = "تم حذف الفرع بنجاح";
            return RedirectToAction("Index");
        }
    }
}